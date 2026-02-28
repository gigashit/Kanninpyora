using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderHandler : MonoBehaviour
{
    public List<OrderTypeObject> orderTypes = new List<OrderTypeObject>();
    public List<DrinkOrder> drinkOrders = new List<DrinkOrder>();

    [Header("Script References")]
    [SerializeField] private PlayerListHandler playerListHandler;

    [Header("Values")]
    [SerializeField] private float weightModifier;

    private Player previousPlayer;
    private DrinkOrder previousOrder;

    private void Start()
    {
        LoadAllDrinkOrders();
        ResetAllWeights();
    }

    private void ResetAllWeights()
    {
        foreach (OrderTypeObject type in orderTypes)
        {
            type.modifiedWeight = type.baseWeight;
        }

        foreach (DrinkOrder order in drinkOrders)
        {
            order.modifiedWeight = order.baseWeight;
        }
    }

    public DrinkOrder GetRandomOrder()
    {
        OrderTypeObject selectedOrderType = null;

        float totalWeights = 0f;
        float cumulativeWeights = 0;

        // First roll for order type

        foreach (OrderTypeObject type in orderTypes)
        {
            totalWeights += type.modifiedWeight;
        }

        float roll = Random.Range(0, totalWeights);

        foreach (OrderTypeObject type in orderTypes)
        {
            cumulativeWeights += type.modifiedWeight;

            if (roll < cumulativeWeights)
            {
                selectedOrderType = type;
                break;
            }
        }

        // Reset rolled type weight, increase other weights

        selectedOrderType.modifiedWeight = selectedOrderType.baseWeight;

        foreach (OrderTypeObject type in orderTypes)
        {
            if (type != selectedOrderType || type.orderType != OrderType.Random || type.orderType != OrderType.Juo || type.orderType != OrderType.Jaa)
            {
                type.modifiedWeight += type.baseWeight * weightModifier;
            }
        }


        // Then roll for specific order

        List<DrinkOrder> orders = new List<DrinkOrder>(drinkOrders);
        DrinkOrder selectedOrder = null;

        orders = drinkOrders.FindAll(t => t.orderType == selectedOrderType.orderType);

        totalWeights = 0f;
        cumulativeWeights = 0f;

        if (orders.Count > 1)
        {
            if (previousOrder != null && previousOrder.orderType == selectedOrderType.orderType)
            {
                orders.Remove(previousOrder);
            }

            foreach (DrinkOrder order in orders)
            {
                totalWeights += order.modifiedWeight;
            }

            roll = Random.Range(0, totalWeights);

            foreach (DrinkOrder order in orders)
            {
                cumulativeWeights += order.modifiedWeight;

                if (roll < cumulativeWeights)
                {
                    selectedOrder = order;
                    break;
                }
            }

            // Reset weights for current order, increase weights for others of the same type

            selectedOrder.modifiedWeight = selectedOrder.baseWeight;

            foreach(DrinkOrder order in orders)
            {
                if (order != selectedOrder)
                {
                    order.modifiedWeight += order.baseWeight * weightModifier;
                }
            }
        }
        else
        {
            selectedOrder = orders[0];
        }

        previousOrder = selectedOrder;
        return selectedOrder;
    }

    public int GetRandomSips(int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    public Player GetRandomPlayer()
    {
        List<Player> reducedList = new List<Player>(playerListHandler.playerList);

        float totalWeights = 0;
        float cumulativeWeight = 0;
        Player selectedPlayer = null;

        if (previousPlayer  != null)
        {
            reducedList.Remove(previousPlayer);
        }

        if (reducedList.Count == 0)
            return null;

        foreach (Player player in reducedList)
        {
            totalWeights += player.modifiedWeight;
        }

        float roll = Random.Range(0, totalWeights);

        foreach(Player player in reducedList)
        {
            cumulativeWeight += player.modifiedWeight;

            if (cumulativeWeight > roll)
            {
                selectedPlayer = player;
                break;
            }
        }

        Player realPlayer = playerListHandler.playerList.Find(p => p.playerName == selectedPlayer.playerName);
        realPlayer.modifiedWeight = realPlayer.baseWeight;

        foreach (Player player in playerListHandler.playerList)
        {
            if (player.playerName != selectedPlayer.playerName)
            {
                player.modifiedWeight += weightModifier;
            }
        }

        previousPlayer = selectedPlayer;
        return selectedPlayer;
    }

    public void LoadAllDrinkOrders()
    {
        drinkOrders = Resources.LoadAll<DrinkOrder>("DrinkOrders").ToList();
    }
}
