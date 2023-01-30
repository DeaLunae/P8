using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;


public enum TargetType
{
    MySql,
    CSV
}

public class LoggingManager : MonoBehaviour
{
    // sampleLog[COLUMN NAME][COLUMN NO.] = [OBJECT] (fx a float, int, string, bool)
    private Dictionary<string, LogStore> logsList = new();
    [Tooltip("If set to true, the logging process will be done over time, resulting in faster saving time.\n" +
             "If set to false, the logging process will use less ressources, but the logs will take more time to be saved")]
    [SerializeField]
    private bool logStringOverTime = true;

    [SerializeField] private bool DebugMode;
    [SerializeField] private string FolderName;
    

    [Tooltip("If save path is empty, it defaults to My Documents.")]
    [SerializeField]
    private string savePath = "";

    
    [SerializeField]
    private string filePrefix = "log";

    [SerializeField]
    private string fileExtension = ".csv";

    private string filePath;
    private char fieldSeperator = ';';
    private string sessionID = "";
    private string deviceID = "";
    private string filestamp;

    private List<TargetType> targetsEnabled;
    private Dictionary<string, Dictionary<TargetType, bool>> originsSavedPerLog;

    // Start is called before the first frame update
    void Awake()
    {
        targetsEnabled = new List<TargetType>();
        //Initializes the list of activated targets
        targetsEnabled.Add(TargetType.CSV);
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

        if (savePath == "")
        {
            savePath = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        }
    }

    public void SetSavePath(string path)
    {
        this.savePath = path;
    }

    private void GenerateLogString(string collectionLabel, Action callback)
    {
        var context = System.Threading.SynchronizationContext.Current;
        new Thread(() =>
        {
            logsList[collectionLabel].ExportAll<string>();
            //runs the callback in the main Thread
            context.Post(_ =>
            {
                callback();
            }, null);

        }).Start();
    }




    public void CreateLog(string collectionLabel, List<string> headers = null)
    {
        if (logsList.ContainsKey(collectionLabel))
        {
            Debug.LogWarning(collectionLabel + " already exists");
            return;
        }
        LogStore logStore = new LogStore(collectionLabel, logStringOverTime, headers:headers);
        logsList.Add(collectionLabel, logStore);
    }


    public void Log(string collectionLabel, Dictionary<string, object> logData)
    {
        //checks if the log was created and creates it if not
        if (logsList.TryGetValue(collectionLabel, out LogStore logStore))
        {
            AddToLogstore(logStore, logData);
        }
        //this will be executed only once if the log has not been created.
        else
        {
            LogStore newLogStore = new LogStore(collectionLabel, logStringOverTime);
            AddToLogstore(newLogStore, logData);
            logsList.Add(collectionLabel, newLogStore);
        }
    }

    private void AddToLogstore(LogStore logStore, Dictionary<string, object> logData)
    {
        foreach (KeyValuePair<string, object> pair in logData)
        {
            logStore.Add(pair.Key, pair.Value);
        }
        if (logStore.LogType == LogType.LogEachRow)
        {
            logStore.EndRow();
        }
    }

    public void Log(string collectionLabel, string columnLabel, object value)
    {
        //checks if the log was created and creates it if not
        if (logsList.TryGetValue(collectionLabel, out LogStore logStore))
        {
            AddToLogstore(logStore, columnLabel, value);
        }
        //this will be executed only once if the log has not been created.
        else
        {
            LogStore newLogStore = new LogStore(collectionLabel, logStringOverTime);
            logsList.Add(collectionLabel, newLogStore);
            AddToLogstore(newLogStore, columnLabel, value);
        }
    }

    private void AddToLogstore(LogStore logStore, string columnLabel, object value)
    {
        logStore.Add(columnLabel, value);
        if (logStore.LogType == LogType.LogEachRow)
        {
            logStore.EndRow();
        }
    }





    public void ClearAllLogs()
    {
        foreach (KeyValuePair<string, LogStore> pair in logsList)
        {
            pair.Value.Clear();
        }
    }

    public void ClearLog(string collectionLabel)
    {
        if (logsList.ContainsKey(collectionLabel))
        {
            logsList[collectionLabel].Clear();
        }
        else
        {
            Debug.LogError("Collection " + collectionLabel + " does not exist.");
        }
    }

    public void DeleteLog(string collectionLabel)
    {
        if (logsList.ContainsKey(collectionLabel))
        {
            logsList.Remove(collectionLabel);
        }
        else
        {
            Debug.LogError("Collection " + collectionLabel + " does not exist.");
        }
    }


    public void DeleteAllLogs()
    {
        foreach (var keyValuePair in logsList)
        {
            logsList.Remove(keyValuePair.Key);
        }
    }

    public void SaveLog(string collectionLabel, bool clear)
    {
        if (logsList.ContainsKey(collectionLabel))
        {
 
            foreach (var targetType in targetsEnabled)
            {
                logsList[collectionLabel].AddSavingTarget(targetType);
            }

            //we generate the string and then we save the logs in the callback
            //by doing this, we are sure that the logs will be exported only once
            GenerateLogString(collectionLabel, () =>
            {
                Save(collectionLabel, clear);
            });
        }
        else
        {
            Debug.LogError("No Collection Called " + collectionLabel);
        }
    }

    public void SaveLog(string collectionLabel, bool clear, TargetType targetType)
    {
        if (logsList.ContainsKey(collectionLabel))
        {
            //we generate the string and then we save the logs in the callback
            //by doing this, we are sure that the logs will be exported only once
            GenerateLogString(collectionLabel, () =>
            {
                Save(collectionLabel, clear);
            });
        }
        else
        {
            Debug.LogError("No Collection Called " + collectionLabel);
        }
    }

    private void Save(string collectionLabel, bool clear)
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            SaveToCSV(collectionLabel, clear);
        }
    }


    public void SaveAllLogs(bool clear)
    {
        foreach (KeyValuePair<string, LogStore> pair in logsList)
        {
            SaveLog(pair.Key, clear);
        }
    }

    public void SaveAllLogs(bool clear,TargetType targetType)
    {
        foreach (KeyValuePair<string, LogStore> pair in logsList)
        {
            SaveLog(pair.Key, clear,targetType);
        }
    }

    private void SaveCallback(LogStore logStore, bool clear)
    {
        if (!clear) return;

        //checks if all the targets have been saved, if not returns
        if (targetsEnabled.Any(targetType => !logStore.TargetsSaved[targetType]))
        {
            return;
        }
        //All targets have been saved, we can clear the logs
        logStore.Clear();
        logStore.ResetTargetsSaved();
    }

    // Formats the logs to a CSV row format and saves them. Calls the CSV headers generation beforehand.
    // If a parameter doesn't have a value for a given row, uses the given value given previously (see 
    // UpdateHeadersAndDefaults).
    private void SaveToCSV(string label, bool clear)
    {
        if (logsList.TryGetValue(label, out LogStore logStore))
        {
            WriteToCSV writeToCsv = new WriteToCSV(logStore, savePath, filePrefix, fileExtension);
            writeToCsv.WriteAll(() =>
            {
                logStore.TargetsSaved[TargetType.CSV] = true;
                SaveCallback(logStore, clear);
            });
        }
        else
        {
            Debug.LogWarning("Trying to save to CSV " + label + " collection but it doesn't exist.");
        }
    }
    public void SaveCurrentParticipantData()
    {
        FolderName = string.IsNullOrEmpty(FolderName) ? "ProjectData" : FolderName;
        var internalPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\{FolderName}";
        var parentDir = new DirectoryInfo($"{internalPath}");
        if (!parentDir.Exists)
        {
            parentDir.Create();
            var directoryInfo = new DirectoryInfo($"{internalPath}\\_Debug");
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }
        if (DebugMode)
        {
            var directoryInfo = new DirectoryInfo($"{internalPath}\\_Debug");
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            SetSavePath(directoryInfo.FullName);
        }
        else
        {
            var currentLargestSubDirectoryNumber = new DirectoryInfo($"{internalPath}").GetDirectories()
                .Where(e => int.TryParse(e.Name, out _)).OrderByDescending(e => int.Parse(e.Name)).ToArray();
            var directoryInfo = currentLargestSubDirectoryNumber.Length == 0 ? 
                new DirectoryInfo($"{internalPath}\\{0}") : 
                new DirectoryInfo($"{internalPath}\\{int.Parse(currentLargestSubDirectoryNumber[0].Name) + 1}");
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            SetSavePath(directoryInfo.FullName);
        }
        SaveAllLogs(true);
    }
}