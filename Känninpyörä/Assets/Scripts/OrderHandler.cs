using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderHandler : MonoBehaviour
{
    public List<OrderTypeObject> orderTypes = new List<OrderTypeObject>();
    public List<DrinkOrder> drinkOrders = new List<DrinkOrder>();

    [Header("UI References")]
    public Button spinButton;
    [SerializeField] private Image wheelBackground;
    [SerializeField] private Animator wheelAnimator;
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Button popupPanelBackgroundButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Animator popupAnimator;
    [SerializeField] private Image titleBackgroundImage;
    [SerializeField] private Image leftIcon;
    [SerializeField] private Image rightIcon;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;

    [Header("Script References")]
    [SerializeField] private PlayerListHandler playerListHandler;

    [Header("Values")]
    [SerializeField] private float weightModifier;

    private Player previousPlayer;
    private DrinkOrder previousOrder;
    private OrderTypeObject rolledOrderType;
    private DrinkOrder rolledOrder;
    [HideInInspector] public int roundCount = 0;

    private Color dimmerColor = new Color(0f, 0f, 0f, 0.6f);
    private Color transparentColor = new Color(0f, 0f, 0f, 0f);

    private void Start()
    {
        LoadAllDrinkOrders();
        ResetAllWeights();

        spinButton.onClick.AddListener(InitiateSpin);
        popupPanelBackgroundButton.onClick.AddListener(ResetPopup);
        continueButton.onClick.AddListener(ResetPopup);

        wheelBackground.raycastTarget = false;
        wheelBackground.color = transparentColor;

        popupPanel.SetActive(false);
        popupPanelBackgroundButton.interactable = false;
        continueButton.gameObject.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        ResetAllWeights();
    }

    private void InitiateSpin()
    {
        spinButton.interactable = false;

        roundCount++;

        rolledOrder = GetRandomOrder();

        wheelBackground.color = dimmerColor;
        wheelBackground.raycastTarget = true;

        // Animate wheel spin here based on the rolled order

        int roll = Random.Range(rolledOrderType.minimumAnimationIndex, rolledOrderType.maximumAnimationIndex + 1);
        string trigger = "wheelSpin" + roll;
        
        wheelAnimator.SetTrigger(trigger);
    }

    public void ShowPopup()
    {
        popupPanel.SetActive(true);

        titleBackgroundImage.color = rolledOrderType.orderTypeColor;
        leftIcon.sprite = rolledOrderType.orderTypeIcon;
        rightIcon.sprite = rolledOrderType.orderTypeIcon;
        titleText.text = rolledOrder.orderTitle;
        bodyText.text = rolledOrder.orderBodyText;

        popupAnimator.SetTrigger("Bounce");

        Invoke(nameof(EnableContinueButton), 3f);
    }

    private void EnableContinueButton()
    {
        popupPanelBackgroundButton.interactable = true;
        continueButton.gameObject.SetActive(true);
    }

    private void ResetPopup()
    {
        spinButton.interactable = true;

        wheelBackground.raycastTarget = false;
        wheelBackground.color = transparentColor;

        popupPanelBackgroundButton.interactable = false;
        continueButton.gameObject.SetActive(false);
        popupPanel.SetActive(false);

        wheelAnimator.SetTrigger("resetWheel");
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
        List<OrderTypeObject> reducedOrderTypes = new List<OrderTypeObject>(orderTypes);

        float totalWeights = 0f;
        float cumulativeWeights = 0;

        // First roll for order type

        if (roundCount < 8)
        {
            OrderTypeObject taukoType = orderTypes.Find(t => t.orderType == OrderType.Tauko);
            reducedOrderTypes.Remove(taukoType);
        }

        foreach (OrderTypeObject type in reducedOrderTypes)
        {
            totalWeights += type.modifiedWeight;
        }

        float roll = Random.Range(0, totalWeights);

        foreach (OrderTypeObject type in reducedOrderTypes)
        {
            cumulativeWeights += type.modifiedWeight;

            if (roll < cumulativeWeights)
            {
                selectedOrderType = type;
                rolledOrderType = type;
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
