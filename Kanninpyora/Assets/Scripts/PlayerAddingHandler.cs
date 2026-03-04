using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAddingHandler : MonoBehaviour
{
    [SerializeField] private Button addPlayerButton;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button backgroundButton;
    [SerializeField] private GameObject panel;
    [SerializeField] private PlayerListHandler playerListHandler;

    private string playerToBeAddedName;

    void Start()
    {
        panel.SetActive(false);

        inputField.onValueChanged.AddListener(delegate { UpdateButton(); });
        addPlayerButton.onClick.AddListener(AddPlayer);
        backgroundButton.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel()
    {
        ResetInputField();
        panel.SetActive(true);
        UpdateButton();
    }

    private void ClosePanel()
    {
        panel.SetActive(false);
    }

    private void ResetInputField()
    {
        inputField.text = string.Empty;
    }

    private void UpdateButton()
    {
        if (inputField.text !=  string.Empty)
        {
            addPlayerButton.interactable = true;
            playerToBeAddedName = inputField.text;
        }
        else
        {
            addPlayerButton.interactable = false;
        }
    }

    private void AddPlayer()
    {
        playerListHandler.AddPlayerToLists(playerToBeAddedName);

        playerToBeAddedName = string.Empty;
        ClosePanel();
    }

}
