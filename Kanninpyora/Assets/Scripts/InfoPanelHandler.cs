using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelHandler : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button openInfoPanelButton;
    [SerializeField] private Button closeInfoPanelButton;

    [Header("UI Elements")]
    [SerializeField] private List<TMP_Text> percentageTexts = new List<TMP_Text>();

    [Header("Order Types")]
    [SerializeField] private List<OrderTypeObject> orderTypeObjects = new List<OrderTypeObject>();

    private void Start()
    {
        openInfoPanelButton.onClick.AddListener(OpenInfoPanel);
        closeInfoPanelButton.onClick.AddListener(CloseInfoPanel);

        CloseInfoPanel();
    }

    private void OpenInfoPanel()
    {
        closeInfoPanelButton.gameObject.SetActive(true);
        CalculatePercentages();
    }

    private void CalculatePercentages()
    {
        float totalWeight = 0f;

        foreach (OrderTypeObject orderType in orderTypeObjects)
        {
            totalWeight += orderType.modifiedWeight;
        }

        for (int i = 0; i < percentageTexts.Count; i++)
        {
            float percentage = (orderTypeObjects[i].modifiedWeight / totalWeight) * 100f;

            if (percentage < 10f)
            {
                percentage = (Mathf.Round(percentage * 10.0f) * 0.1f);
                percentageTexts[i].fontSize = 50;
            }
            else
            {
                percentage = Mathf.RoundToInt(percentage);
                percentageTexts[i].fontSize = 60;
            }

            percentageTexts[i].text = percentage + "<size=70%>%";
        }
    }

    private void CloseInfoPanel()
    {
        closeInfoPanelButton.gameObject.SetActive(false);
    }

}
