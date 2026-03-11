using UnityEngine;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button openSettingsButton;
    [SerializeField] private Button closeSettingsButton;
    [SerializeField] private Toggle debugStatsToggle;

    [Header("UI Elements")]
    [SerializeField] private GameObject settingsPanelParent;

    [Header("Debug Stat Texts")]
    [SerializeField] private GameObject playerDebugValues;
    [SerializeField] private GameObject rollDebugValues;

    void Start()
    {
        openSettingsButton.onClick.AddListener(OpenSettings);
        closeSettingsButton.onClick.AddListener(CloseSettings);

        CloseSettings();
        
        debugStatsToggle.onValueChanged.AddListener(ToggleDebugValues);

        ToggleDebugValues(false);
    }

    void OpenSettings()
    {
        settingsPanelParent.SetActive(true);
    }

    void CloseSettings()
    {
        settingsPanelParent.SetActive(false);
    }

    void ToggleDebugValues(bool value)
    {
        playerDebugValues.SetActive(value);
        rollDebugValues.SetActive(value);
    }
}
