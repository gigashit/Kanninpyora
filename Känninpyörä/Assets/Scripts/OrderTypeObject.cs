using UnityEngine;

[CreateAssetMenu(fileName = "OrderTypeObject", menuName = "Scriptable Objects/OrderTypeObject")]
public class OrderTypeObject : ScriptableObject
{
    public OrderType orderType;
    public float baseWeight;
    public float modifiedWeight;
}
