using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

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
    [SerializeField] private Image wheelRotatingGlow;

    [Header("Script References")]
    [SerializeField] private PlayerListHandler playerListHandler;
    [SerializeField] private DebugStatHandler debugStatHandler;

    [Header("Values")]
    [SerializeField] private float weightModifier;

    private Player previousPlayer;
    private DrinkOrder previousOrder;
    private OrderTypeObject rolledOrderType;
    private DrinkOrder rolledOrder;
    [HideInInspector] public int roundCount = 0;

    [Header("Listat")]
    [SerializeField] private List<string> randomWords = new List<string>();
    [SerializeField] private List<ColorString> randomColors = new List<ColorString>();
    [SerializeField] private List<string> categories = new List<string>();
    [SerializeField] private List<string> ilmansuunnat = new List<string>();
    [SerializeField] private List<string> kuukaudet = new List<string>();
    [SerializeField] private List<string> hahmot = new List<string>();

    private Color dimmerColor = new Color(0f, 0f, 0f, 0.6f);
    private Color transparentColor = new Color(0f, 0f, 0f, 0f);
    private Color rotateGlowColor = new Color(1f, 1f, 0.83f, 1f);

    private Color originalTitleColor;

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

        originalTitleColor = titleText.color;
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

        int roll = UnityEngine.Random.Range(rolledOrderType.minimumAnimationIndex, rolledOrderType.maximumAnimationIndex + 1);
        string trigger = "wheelSpin" + roll;
        
        wheelAnimator.SetTrigger(trigger);

        wheelRotatingGlow.color = transparentColor;
    }

    public void ShowPopup()
    {
        popupAnimator.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        popupPanel.SetActive(true);

        titleBackgroundImage.color = rolledOrderType.orderTypeColor;
        leftIcon.sprite = rolledOrderType.orderTypeIcon;
        rightIcon.sprite = rolledOrderType.orderTypeIcon;
        titleText.text = rolledOrder.orderTitle;

        if (rolledOrderType.orderType == OrderType.Vesiputous) { titleText.color = Color.red; }
        else { titleText.color = originalTitleColor; }

        bodyText.text = GenerateDrinkOrderText(rolledOrder.orderBodyText);

        popupAnimator.SetTrigger("Bounce");

        Invoke(nameof(EnableContinueButton), 3f);
    }

    public string GenerateDrinkOrderText(string template)
    {
        string result = template;

        result = Regex.Replace(result, @"\[pelaaja\]", match =>
        {
            Player p = GetRandomPlayer();
            string hexColor = ColorUtility.ToHtmlStringRGB(p.playerColor);

            return $"<b><color=#{hexColor}>{p.playerName}</color></b>";
        });

        result = Regex.Replace(result, @"\[(\d+)-(\d+)\]", match =>
        {
            int min = int.Parse(match.Groups[1].Value);
            int max = int.Parse(match.Groups[2].Value);

            int sips = GetRandomSips(min, max);

            return $"<b><color=#FFFFFF>{sips}</color></b>";
        });

        result = Regex.Replace(result, @"\[sana\]", match =>
        {
            return $"<b><color=#C8C8C8>{GetRandomWord()}</color></b>";
        });

        result = Regex.Replace(result, @"\[väri\]", match =>
        {
            return $"<b>{GetRandomColor()}</b>";
        });

        result = Regex.Replace(result, @"\[kategoria\]", match =>
        {
            return $"<b><color=#C8C8C8>{GetRandomCategory()}</color></b>";
        });

        result = Regex.Replace(result, @"\[ilmansuunta\]", match =>
        {
            return $"<b><color=#C8C8C8>{GetRandomIlmansuunta()}</color></b>";
        });

        result = Regex.Replace(result, @"\[kuukaudet\]", match =>
        {
            return $"<b><color=#C8C8C8>{GetRandomKuukausi()}</color></b>";
        });

        result = Regex.Replace(result, @"\[hahmot\]", match =>
        {
            return $"<b><color=#C8C8C8>{GetRandomHahmo()}</color></b>";
        });

        return result;
    }

    private void EnableContinueButton()
    {
        popupPanelBackgroundButton.interactable = true;
        continueButton.gameObject.SetActive(true);
    }

    private void ResetPopup()
    {
        popupAnimator.SetTrigger("Hide");

        spinButton.interactable = true;

        wheelBackground.raycastTarget = false;
        wheelBackground.color = transparentColor;

        popupPanelBackgroundButton.interactable = false;
        continueButton.gameObject.SetActive(false);
        popupPanel.SetActive(false);

        wheelAnimator.SetTrigger("resetWheel");
        wheelRotatingGlow.color = rotateGlowColor;
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
            order.hasAppeared = false;
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

            taukoType = orderTypes.Find(t => t.orderType == OrderType.Vesiputous);
            reducedOrderTypes.Remove(taukoType);
        }

        foreach (OrderTypeObject type in reducedOrderTypes)
        {
            totalWeights += type.modifiedWeight;
        }

        float roll = UnityEngine.Random.Range(0, totalWeights);

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

        debugStatHandler.UpdateRollStats(rolledOrderType);

        // Reset rolled type weight, increase other weights

        selectedOrderType.modifiedWeight = selectedOrderType.baseWeight;

        foreach (OrderTypeObject type in orderTypes)
        {
            if (type != selectedOrderType || type.orderType != OrderType.Juo || type.orderType != OrderType.Jaa)
            {
                type.modifiedWeight += type.baseWeight * weightModifier;
            }
        }


        // Then roll for specific order

        List<DrinkOrder> orders = new List<DrinkOrder>(drinkOrders);
        DrinkOrder selectedOrder = null;

        orders = drinkOrders.FindAll(t => t.orderType == selectedOrderType.orderType);

        List<DrinkOrder> ordersToRemove = new List<DrinkOrder>(orders);

        foreach (DrinkOrder order in orders)
        {
            if (order.isOneTime && order.hasAppeared)
            {
                ordersToRemove.Remove(order);
                Debug.Log("Order " + order.orderTitle + " has already appeared, removing from rolling list.");
            }
        }

        orders = ordersToRemove;

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

            roll = UnityEngine.Random.Range(0, totalWeights);

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

        if (selectedOrder.isOneTime)
        {
            selectedOrder.hasAppeared = true;
        }

        return selectedOrder;
    }

    private string GetRandomWord()
    {
        return randomWords[UnityEngine.Random.Range(0, randomWords.Count)];
    }

    private string GetRandomCategory()
    {
        return categories[UnityEngine.Random.Range(0, categories.Count)];
    }

    private string GetRandomIlmansuunta()
    {
        return ilmansuunnat[UnityEngine.Random.Range(0, ilmansuunnat.Count)];
    }

    private string GetRandomKuukausi()
    {
        return kuukaudet[UnityEngine.Random.Range(0, kuukaudet.Count)];
    }

    private string GetRandomHahmo()
    {
        return hahmot[UnityEngine.Random.Range(0, hahmot.Count)];
    }

    private string GetRandomColor()
    {
        ColorString colorString = randomColors[UnityEngine.Random.Range(0, randomColors.Count)];

        string colorHex = ColorUtility.ToHtmlStringRGB(colorString.colorColor);

        return $"<color=#{colorHex}>{colorString.colorName}</color>";
    }

    public int GetRandomSips(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
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

        float roll = UnityEngine.Random.Range(0, totalWeights);

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
        debugStatHandler.UpdatePlayerStats(selectedPlayer);
        return selectedPlayer;
    }

    public void LoadAllDrinkOrders()
    {
        drinkOrders = Resources.LoadAll<DrinkOrder>("DrinkOrders").ToList();
    }
}

[Serializable] public class ColorString
{
    public string colorName;
    public Color colorColor;
}
