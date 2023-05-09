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

    [ContextMenu("Add 2 copies of every gesture")]
    public void Add3OfEveryGesture()
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            list.Insert(i,new Gesture(list[i].Name, list[i].Handedness, list[i].staticGesture, list[i].demonstrationData));
            list.Insert(i,new Gesture(list[i].Name, list[i].Handedness, list[i].staticGesture, list[i].demonstrationData));
        }
    }

    
    [ContextMenu("Delete all demonstration poses except the last for Æ")]
    public void DeleteAllDemonstrationPosesExceptTheLastForÆ()
    {
        foreach (var gesture in list)
        {
            if (gesture.Name == Æ)
            {
                gesture.demonstrationData = new List<Gesture.PoseFrameData> { gesture.demonstrationData.Last() };
            }
        }
    }
    [ContextMenu("Fill with all letters")]
    public void FillWithAllTwoHandedLetters()
    {
        list.Clear();
        var letters = ((Gesture.GestureName[])Enum.GetValues(typeof(Gesture.GestureName))).Skip(1);
        
        foreach (var letter in letters)
        {
            list.Add(new Gesture(letter, Handedness.Right, IsStatic(letter)));
            list.Add(new Gesture(letter, Handedness.Left, IsStatic(letter)));
        }
    }

    public bool IsStatic(Gesture.GestureName gesture)
    {
        return gesture switch
        {
            J => false,
            Z => false,
            Ø => false,
            Å => false,
            Nummer10 => false,
            _ => true
        };
    }
    [ContextMenu("Fill with all letters only right hand")]
    public void FillWithAllRightHandedLetters()
    {
        list.Clear();
        var letters = ((Gesture.GestureName[])Enum.GetValues(typeof(Gesture.GestureName))).Skip(1);
        foreach (var letter in letters)
        {
            list.Add(new Gesture(letter, Handedness.Right, IsStatic(letter)));
        }
    }
    [ContextMenu("Remove all left handed letters")]
    public void RemoveAllLeftHandedLetters()
    {
        list.RemoveAll(x => x.Handedness == Handedness.Left);
    }
    
    [ContextMenu("Remove all right handed letters")]
    public void RemoveAllRightHandedLetters()
    {
        list.RemoveAll(x => x.Handedness == Handedness.Right);
    }
    
    [ContextMenu("Fill with all letters only left hand")]
    public void FillWithAllLeftHandedLetters()
    {
        list.Clear();
        var letters = ((Gesture.GestureName[])Enum.GetValues(typeof(Gesture.GestureName))).Skip(1);
        foreach (var letter in letters)
        {
            list.Add(new Gesture(letter, Handedness.Left, IsStatic(letter)));
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
            list.Add(new Gesture(letter, Handedness.Right, IsStatic(letter)));
            if (lettersForLeftHand.Contains(letter))
            {
                list.Add(new Gesture(letter, Handedness.Left, IsStatic(letter)));
            }
        }
    }
    [ContextMenu("Mirror current gestures")]
    public void MirrorCurrentGestures()
    {
        for (var i = 0; i < list.Count; i++)
        {
            list[i].Handedness = list[i].Handedness == Handedness.Right ? Handedness.Left : Handedness.Right;
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
                list.Add(new Gesture(letter, Handedness.Right, false));
            }
        }
    }
    
    [ContextMenu("RandomizeCurrentListDeterministically")]
    public void DeterministicRandomizeCurrentList()
    {
        var random = new System.Random(0);
        list = list.OrderBy(x => random.Next()).ToList();
    }
    

}
