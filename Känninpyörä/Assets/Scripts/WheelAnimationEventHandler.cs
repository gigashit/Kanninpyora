using UnityEngine;

public class WheelAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private OrderHandler orderHandler;

    public void SpinAnimationEnd()
    {
        orderHandler.ShowPopup();
        Debug.Log("Spin animation ended");
    }
}
