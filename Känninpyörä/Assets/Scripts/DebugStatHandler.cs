using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugStatHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text playerValues;
    [SerializeField] private TMP_Text orderValues;

    private Dictionary<OrderType, int> orderCounts = new Dictionary<OrderType, int>();
    private int totalOrderCount = 0;

    private Dictionary<Player, int> playerTurns = new Dictionary<Player, int>();

    void Start()
    {
        InitializeOrderStats();
    }

    public void InitializePlayerStats(List<Player> ogPlayerList)
    {
        List<Player> players = new List<Player>(ogPlayerList);

        playerTurns.Clear();

        foreach(Player p in players)
        {
            playerTurns[p] = 0;
        }

        RefreshPlayerStatsText();
    }

    private void InitializeOrderStats()
    {
        foreach (OrderType type in System.Enum.GetValues(typeof(OrderType)))
            orderCounts[type] = 0;

        RefreshOrderStatsText();
    }

    public void UpdatePlayerStats(Player player)
    {
        playerTurns[player]++;

        RefreshPlayerStatsText();
    }

    private void RefreshPlayerStatsText()
    {
        string playerStats = "Pelaajien vuorot:";

        foreach (var pair in playerTurns)
        {
            playerStats += $"<br>{pair.Key.playerName} = {pair.Value}";
        }

        playerValues.text = playerStats;
    }

    public void UpdateRollStats(OrderTypeObject orderType)
    {
        totalOrderCount++;
        orderCounts[orderType.orderType]++;

        RefreshOrderStatsText();
    }

    private void RefreshOrderStatsText()
    {
        string orderStats = "Pyöräytystodennäköisyydet:<br>Pyöräytyksiä yhteensä = " + totalOrderCount;

        foreach (var pair in orderCounts)
        {
            float percentage = totalOrderCount == 0
                ? 0f
                : (float)pair.Value / totalOrderCount * 100f;

            orderStats += $"<br>{pair.Key} = {percentage:F1}%";
        }

        orderValues.text = orderStats;
    }
}
