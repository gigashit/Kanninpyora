using UnityEngine;

[CreateAssetMenu(fileName = "DrinkOrder", menuName = "Scriptable Objects/DrinkOrder")]
public class DrinkOrder : ScriptableObject
{
    public string orderTitle;
    public OrderType orderType;
    [TextArea(5, 10)]
    public string orderBodyText;
    public float baseWeight;
    public float modifiedWeight;
}

public enum OrderType
{
    Juo,
    Jaa,
    Duel,
    Salama,
    Tauko,
    Vesiputous,
    Random
}
