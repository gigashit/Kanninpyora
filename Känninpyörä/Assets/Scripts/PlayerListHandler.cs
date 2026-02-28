using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListHandler : MonoBehaviour
{
    [Header("Player List")]
    public List<Player> playerList = new List<Player>();

    [Header("Buttons")]
    [SerializeField] private Button openAddPlayerPanelButton;

    [Header("Script References")]
    [SerializeField] private PlayerAddingHandler playerAddingHandler;

    [Header("Prefabs")]
    [SerializeField] private GameObject playerEntryPrefab;

    [Header("UI References")]
    [SerializeField] private Transform playerListContent;

    [Header("Flags")]
    public bool playerListLoaded = false;

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

        playerList.Add(newPlayer);

        GameObject newEntry = Instantiate(playerEntryPrefab, playerListContent);
        VisualPlayerEntry entryScript = newEntry.GetComponent<VisualPlayerEntry>();

        entryScript.SetupEntry(playerString, this);

        Debug.Log("Added player = " +  playerString);
        PrintPlayerList();
    }

    public void RemovePlayerFromLists(string playerString)
    {
        Player playerToRemove = playerList.Find(p => p.playerName == playerString);

        if (playerToRemove != null)
        {
            playerList.Remove(playerToRemove);

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

        Debug.Log("Current Players = " + playersListed);
        SavePlayerList();
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
    }
}

public class Player
{
    public string playerName;
    public float baseWeight;
    public float modifiedWeight;
}