using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListHandler : MonoBehaviour
{
    [Header("Player List")]
    public List<Player> playerList = new List<Player>();
    public List<Color> playerColors = new List<Color>();

    [Header("Buttons")]
    [SerializeField] private Button openAddPlayerPanelButton;

    [Header("Script References")]
    [SerializeField] private PlayerAddingHandler playerAddingHandler;
    [SerializeField] private OrderHandler orderHandler;

    [Header("Prefabs")]
    [SerializeField] private GameObject playerEntryPrefab;

    [Header("UI References")]
    [SerializeField] private Transform playerListContent;
    [SerializeField] private TMP_Text tipText;
    [SerializeField] private Animator wheelAnimator;

    [Header("Values")]
    [SerializeField] private string notEnoughPlayersText;
    [SerializeField] private string enoughPlayersText;

    [Header("Flags")]
    public bool playerListLoaded = false;

    private List<Color> unusedPlayerColors = new List<Color>();

    private void Start()
    {
        LoadPlayerList();

        openAddPlayerPanelButton.onClick.AddListener(playerAddingHandler.OpenPanel);
    }

    public void AddPlayerToLists(string playerString)
    {
        Player newPlayer = new Player();
        newPlayer.playerName = playerString;
        newPlayer.baseWeight = 1;
        newPlayer.modifiedWeight = 1;
        newPlayer.playerColor = GetRandomColor();

        unusedPlayerColors.Remove(newPlayer.playerColor);
        playerList.Add(newPlayer);

        GameObject newEntry = Instantiate(playerEntryPrefab, playerListContent);
        VisualPlayerEntry entryScript = newEntry.GetComponent<VisualPlayerEntry>();

        entryScript.SetupEntry(newPlayer, this);

        Debug.Log("Added player = " +  playerString);
        PrintPlayerList();
    }

    public void RemovePlayerFromLists(string playerString)
    {
        Player playerToRemove = playerList.Find(p => p.playerName == playerString);

        if (playerToRemove != null)
        {
            playerList.Remove(playerToRemove);
            unusedPlayerColors.Add(playerToRemove.playerColor);

            Debug.Log("Removed player = " + playerString);
        }
        else
        {
            Debug.LogError("Player Removal failed, no player found by the name " +  playerString);
        }

        PlayerPrefs.DeleteAll();
        PrintPlayerList();
    }

    private void PrintPlayerList()
    {
        string playersListed = "";

        for (int i = 0; i < playerList.Count; i++)
        {
            Player currentPlayer = playerList[i];

            playersListed += currentPlayer.playerName;

            if (i < playerList.Count - 1)
            {
                playersListed += ", ";
            }
        }

        UpdateSpinButtonState();

        Debug.Log("Current Players = " + playersListed);
        SavePlayerList();
    }

    private void UpdateSpinButtonState()
    {
        bool enoughPlayers = false;

        if (playerList.Count >= 2)
        {
            enoughPlayers = true;
            tipText.text = enoughPlayersText;
        }
        else
        {
            tipText.text = notEnoughPlayersText;
        }

        orderHandler.spinButton.interactable = enoughPlayers;
        wheelAnimator.SetBool("canPlay", enoughPlayers);
    }

    private void SavePlayerList()
    {
        int playerIndex = 0;
        string prefsKey = "";

        foreach (Player player in playerList)
        {
            prefsKey = "player" + playerIndex;
            PlayerPrefs.SetString(prefsKey, player.playerName);
            playerIndex++;

            Debug.Log(prefsKey + " saved");
        }

        Debug.Log("Player list saved");
    }

    private void LoadPlayerList()
    {
        foreach (Color color in playerColors)
        {
            unusedPlayerColors.Add(color);
        }

        if (PlayerPrefs.HasKey("player0"))
        {
            bool morePlayers = true;
            int playerIndex = 0;
            string playerToAdd = "";

            while (morePlayers)
            {
                if (PlayerPrefs.HasKey("player" +  playerIndex))
                {
                    playerToAdd = PlayerPrefs.GetString("player" + playerIndex);
                    AddPlayerToLists(playerToAdd);
                    playerIndex++;
                }
                else
                {
                    morePlayers = false;
                }
            }

            Debug.Log("Previous player list successfully loaded");
        }
        else
        {
            Debug.Log("No previous player list to load");
        }

        playerListLoaded = true;
        UpdateSpinButtonState();
    }

    private Color GetRandomColor()
    {
        return unusedPlayerColors[Random.Range(0, unusedPlayerColors.Count)];
    }

}

public class Player
{
    public string playerName;
    public Color playerColor;
    public float baseWeight;
    public float modifiedWeight;
}