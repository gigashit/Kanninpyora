using UnityEngine;

[CreateAssetMenu(fileName = "OrderTypeObject", menuName = "Scriptable Objects/OrderTypeObject")]
public class OrderTypeObject : ScriptableObject
{
    public OrderType orderType;
    public Sprite orderTypeIcon;
    public Color orderTypeColor;
    public float baseWeight;
    public float modifiedWeight;
    public int minimumAnimationIndex;
    public int maximumAnimationIndex;
}
