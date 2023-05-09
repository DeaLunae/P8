using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LatinSquareRandomizer : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents _gestureRecorderEvents;
    
    // Start is called before the first frame update
    private void Awake()
    {
        if (Application.isEditor)
        {
            return;
        }
        _gestureRecorderEvents.GestureList = GenerateLatinSquare(_gestureRecorderEvents.GestureList, GetCurrentParticipantID());
    }
    [ContextMenu("PrintSequence")]
    public void PrintSequence()
    {
        string res = "";
        var sep = ",";
        var newline = "\n";
        for (int i = 0; i < 40; i++)
        {
            var seq = GenerateLatinSquare(_gestureRecorderEvents.GestureList, i);
            for (int j = 0; j < seq.Count; j++)
            {
                res += $"{seq[j].Name.ToString().Replace("Nummer","")}{(j == seq.Count - 1 ? newline : sep)}";
            }
        }
        Debug.Log(res);
    }
    public int GetCurrentParticipantID()
    {
        var currLargSubDirNr = new DirectoryInfo($"{root}").GetDirectories().Where(e => int.TryParse(e.Name, out _))
            .Select(e => int.Parse(e.Name)).OrderByDescending(e => e).ToArray();
        return currLargSubDirNr.Length == 0 ? 0 : int.Parse(currLargSubDirNr.First().ToString()) + 1;
    }
    
    string root = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\P8GestureData";

    // Based on "Bradley, J. V. Complete counterbalancing of immediate sequential effects in a Latin square design. J. Amer. Statist. Ass.,.1958, 53, 525-528. "
    public List<Gesture> GenerateLatinSquare(List<Gesture> gestures, int participantId)
    {
        var result = new List<Gesture>();
        for (int i = 0, j = 0, h = 0; i < gestures.Count; ++i) {
            int val = 0;
            if (i < 2 || i % 2 != 0) {
                val = j++;
            } else {
                val = gestures.Count - h - 1;
                ++h;
            }
            var idx = (val + participantId) % gestures.Count;
            result.Add(gestures[idx]);
        }
        if (gestures.Count % 2 != 0 && participantId % 2 != 0) {
            result.Reverse();
        }
        return result;
    }
}