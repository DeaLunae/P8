using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static OVRInput;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;

public class ControllerGestureHandler : MonoBehaviour
{
    [SerializeField] private Handedness hand;
    
    [SerializeField] private GestureIdReference _actionToOutput;
    [SerializeField, Range(0f, 1f)] private float selectActivationMinValue;
    [SerializeField, Range(0f, 1f)] private float hoverActivationMinValue;
    [SerializeField, Range(0f, 1f)] private float grabActivationMinValue;

    private RawButton currButtonPress = RawButton.None;
    private RawAxis1D currAxisPress = RawAxis1D.None;

    private void Update()
    {
        CheckIfTranslate();
        if (_actionToOutput.Value != Gesture.GestureID.Translate)
        {
            CheckIfHoverOrSelect();
        }
        if (hand == Handedness.Left)
        {
            OnButtonPressed(Gesture.GestureID.Scale, RawButton.X);
        }

        if (hand == Handedness.Right)
        {
            OnButtonPressed(Gesture.GestureID.Scale,RawButton.A);
        }
        //OnButtonPressed(Gesture.GestureID.Rotate, RawButton.X);
        
        if (CurrButtonReleased())
        {
            _actionToOutput.Value = Gesture.GestureID.None;
            currButtonPress = RawButton.None;
        }
    }

    private void CheckIfTranslate()
    {
        Controller controller = Controller.None;
        var value = hand switch
        {
            Handedness.Left => Get(RawAxis1D.LHandTrigger),
            Handedness.Right => Get(RawAxis1D.RHandTrigger),
            _ => 0f
        };
        if (value > grabActivationMinValue)
        {
            if (_actionToOutput.Value == Gesture.GestureID.Translate)
            {
                return;
            }
            
            _actionToOutput.Value = Gesture.GestureID.Translate;
            return;
        }
        if (!(value <= grabActivationMinValue)) return;
        if (_actionToOutput.Value == Gesture.GestureID.Translate)
        {
            _actionToOutput.Value = Gesture.GestureID.None;
        }
    }
    private void CheckIfHoverOrSelect()
    {
        Controller controller = Controller.None;
        var value = hand switch
        {
            Handedness.Left => Get(RawAxis1D.LIndexTrigger),
            Handedness.Right => Get(RawAxis1D.RIndexTrigger),
            _ => 0f
        };

        if (value > selectActivationMinValue)
        {
            if (_actionToOutput.Value == Gesture.GestureID.Select)
            {
                return;
            }

            _actionToOutput.Value = Gesture.GestureID.Select;
            return;
        }
        if (value > hoverActivationMinValue)
        {
            if (_actionToOutput.Value == Gesture.GestureID.Hover)
            {
                return;
            }
            _actionToOutput.Value = Gesture.GestureID.Hover;
            return;
        }
        if (!(value < hoverActivationMinValue)) return;
        if (_actionToOutput.Value == Gesture.GestureID.Hover || _actionToOutput.Value == Gesture.GestureID.Select)
        {
            _actionToOutput.Value = Gesture.GestureID.None;
        }
    }
    public bool CurrButtonReleased()
    {
        return currButtonPress != RawButton.None && !Get(currButtonPress);
    }
    public void OnButtonPressed(Gesture.GestureID action, RawButton button)
    {
        //we now know it's a button press
        if (currButtonPress == RawButton.None)
        {
            //no button press is currently being done
            if (GetDown(button) && _actionToOutput.Value != action )
            {
                _actionToOutput.Value = action;
                currButtonPress = button;
            }
        }else if (currButtonPress != button)
        {
            //we now know it's a button press, and that a button press is currently being done that isn't this button
            if (GetDown(button) && _actionToOutput.Value != action )
            {
                _actionToOutput.Value = action;
                currButtonPress = button;
            }
        }
    }
}
