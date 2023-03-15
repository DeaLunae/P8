using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using static Gesture.GestureName;

[CreateAssetMenu]
public class GestureList : ScriptableObject
{
    public List<Gesture> list;
    
    [ContextMenu("Clear list")]
    public void ClearList()
    {
        list.Clear();
    }
    
    [ContextMenu("Fill with all letters")]
    public void FillWithAllTwoHandedLetters()
    {
        list.Clear();
        var letters = ((Gesture.GestureName[])Enum.GetValues(typeof(Gesture.GestureName))).Skip(1);
        foreach (var letter in letters)
        {
            list.Add(new Gesture(letter, Handedness.Right));
            list.Add(new Gesture(letter, Handedness.Left));
        }
    }
    
    [ContextMenu("Fill with all letters only right hand")]
    public void FillWithAllRightHandedLetters()
    {
        list.Clear();
        var letters = ((Gesture.GestureName[])Enum.GetValues(typeof(Gesture.GestureName))).Skip(1);
        foreach (var letter in letters)
        {
            list.Add(new Gesture(letter, Handedness.Right));
        }
    }
    
    [ContextMenu("Fill with all letters only left hand")]
    public void FillWithAllLeftHandedLetters()
    {
        list.Clear();
        var letters = ((Gesture.GestureName[])Enum.GetValues(typeof(Gesture.GestureName))).Skip(1);
        foreach (var letter in letters)
        {
            list.Add(new Gesture(letter, Handedness.Left));
        }
    }
    
    [ContextMenu("Fill with all letters only both hands for some")]
    public void FillWithAllLettersBothHandsForSome()
    {
        list.Clear();
        var lettersForLeftHand = new List<Gesture.GestureName>{J, Z, Ø, Å, Nummer10};
        var letters = ((Gesture.GestureName[])Enum.GetValues(typeof(Gesture.GestureName))).Skip(1);
        foreach (var letter in letters)
        {
            list.Add(new Gesture(letter, Handedness.Right));
            if (lettersForLeftHand.Contains(letter))
            {
                list.Add(new Gesture(letter, Handedness.Left));
            }
        }
    }
    
    [ContextMenu("Fill with 10 versions of all dynamic letters")]
    public void FillWith10VersionsOfAllDynamicLetters()
    {
        list.Clear();
        var letters = new List<Gesture.GestureName>{J, Z, Ø, Å, Nummer10};
        foreach (var letter in letters)
        {
            for (int i = 0; i < 10; i++)
            {
                list.Add(new Gesture(letter, Handedness.Right));
            }
        }
    }
}
