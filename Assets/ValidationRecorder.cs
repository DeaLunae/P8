using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ValidationRecorder : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvents;
    [SerializeField] private bool debugMode;
    LoggingManager _lm;

    private void Start()
    {
        _lm = FindObjectOfType<LoggingManager>();
        _lm.SetEmail("NA");
        gestureRecorderEvents.EndOfListReached += SaveAllData;
    }
    
    public void SaveAllData()
    {
        _lm.Log("Validation", GenerateValidationData());
        string root = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\P8GestureData";
        GetOrCreateDirectory($"{root}");
        if (debugMode)
        {
            GetOrCreateDirectory($"{root}\\Debug");
            _lm.SetSavePath($"{root}\\Debug");
        }
        else
        {
            var currLargSubDirNr = new DirectoryInfo($"{root}").GetDirectories().Where(e => int.TryParse(e.Name, out _))
                .Select(e => int.Parse(e.Name)).OrderByDescending(e => e).ToArray();
            DirectoryInfo partDirInf = GetOrCreateDirectory(currLargSubDirNr.Length == 0 ? $"{root}\\{0}" : $"{root}\\{int.Parse(currLargSubDirNr.First().ToString())+1}");
            _lm.SetSavePath(partDirInf.FullName);
        }
        _lm.SaveAllLogs(true,TargetType.CSV);
        gestureRecorderEvents.EndOfListReached-= SaveAllData;
    }

    private Dictionary<string, object> GenerateValidationData()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        foreach (var gesture in gestureRecorderEvents.GestureList)
        {
            data.Add($"{gesture.Name}.{gesture.Handedness}", gesture.UserSessionResponse.ToString());
        }
        return data;
    }

    private DirectoryInfo GetOrCreateDirectory(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        return directoryInfo;
    }

    private void OnDestroy()
    {
        gestureRecorderEvents.EndOfListReached -= SaveAllData;
    }
}
