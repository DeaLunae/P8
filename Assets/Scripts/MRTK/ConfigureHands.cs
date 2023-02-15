using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
public class ConfigureHands : MonoBehaviour
{
    [SerializeField] private PointerBehavior handRayPointerBehavior;
    [SerializeField] private PointerBehavior gazePointerBehavior;
    [SerializeField] private PointerBehavior handGrabPointerBehavior;

    private void Start()
    {
        PointerUtils.SetHandRayPointerBehavior(handRayPointerBehavior);
        PointerUtils.SetGazePointerBehavior(gazePointerBehavior);
        PointerUtils.SetHandGrabPointerBehavior(handGrabPointerBehavior);
    }
}