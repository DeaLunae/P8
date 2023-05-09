using System;
using System.Linq;
using TMPro;
using UnityEngine;
using static Gesture;

public class KeyboardCharacterPressInvoker : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;
    private TMP_Text _text;
    private void Start()
    {
        _text = GetComponentsInChildren<TMP_Text>().FirstOrDefault();
    }

    public void OnButtonPressed()
    {
        if (_text.text == "")
        {
            return;
        }
        gestureRecorderEvents.SetUserSessionResponse(ConvertTextToEnum(_text.text));
    }

    private GestureName ConvertTextToEnum(string text)
    {
        if (text == "")
        {
            return GestureName.None;
        }
        if (int.TryParse(text, out _))
        {
            return Enum.TryParse($"Nummer{text}", out GestureName resultingNumber)
                ? resultingNumber
                : GestureName.None;
        }
        return Enum.TryParse(text, out GestureName resultingLetter) ? resultingLetter : GestureName.None;
    }
}
