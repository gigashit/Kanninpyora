using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisualPlayerEntry : MonoBehaviour
{
    [SerializeField] private Button removeButton;
    [SerializeField] private TMP_Text playerNameText;

    public string currentPlayerName;
    private PlayerListHandler playerListHandler;

    private void Start()
    {
        removeButton.onClick.AddListener(RemoveEntry);
    }

    public void SetupEntry(Player player, PlayerListHandler handler)
    {
        currentPlayerName = player.playerName;
        playerNameText.text = player.playerName;
        playerNameText.color = player.playerColor;
        playerListHandler = handler;
    }

    private void RemoveEntry()
    {
        playerListHandler.RemovePlayerFromLists(currentPlayerName);
        Destroy(gameObject);
    }
}
