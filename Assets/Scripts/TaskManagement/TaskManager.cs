using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Devkit.Modularis.References;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private InteractableTaskReference _currentTaskDescription;
    [SerializeField] private IntReference _condition;
    [SerializeField,ReadOnly] private int currentIndex = 0;
    [SerializeField] private bool _debugMode;
    [SerializeField] private List<InteractableTaskReference> tasks;
    [SerializeField] private AudioClip TaskCompleteClip;
    
    private LoggingManager _loggingManager;
    private AudioSource _audioSource;
    private Stopwatch _stopwatch;
    private readonly string _root = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\P7TaskData";
    private void Start()
    {
        _stopwatch = Stopwatch.StartNew();
        _loggingManager = FindObjectOfType<LoggingManager>();
        _loggingManager.SetEmail("NA");
        tasks[currentIndex].RegisterCallback(OnEventCompleted);
        _audioSource = GetComponent<AudioSource>();
        Invoke(nameof(DelayedStart),0.1f);
    }

    private void DelayedStart()
    {
        _currentTaskDescription.Value = tasks[currentIndex];
    }

    private bool hasSaved = false;
    private void OnEventCompleted(InteractableTask interactableTask)
    {
        LogEvent();
        _stopwatch.Restart();
        _audioSource.Play();
        tasks[currentIndex].UnregisterCallback(OnEventCompleted);
        currentIndex++;
        
        if (currentIndex >= tasks.Count)
        {
            if (!hasSaved)
            {
                Save();
                hasSaved = true;
            }
            //Application.Quit();
            return;
        }
        _currentTaskDescription.Value = tasks[currentIndex];
        tasks[currentIndex].RegisterCallback(OnEventCompleted);
    }

    private void LogEvent()
    {
        var output = new Dictionary<string, object>();
        output.Add("TaskID",currentIndex);
        output.Add("Condition", _condition.Value == 0 ? "Hands" : "Controllers");
        output.Add("Task Description", tasks[currentIndex].Value.taskDescription);
        output.Add("Task Type", tasks[currentIndex].Value.taskType.ToString());
        output.Add("TaskCompletionTimeInMillis",_stopwatch.ElapsedMilliseconds);
        output.Add("Timestamp", Time.time);
        //output.Add("Amount of translate ends");
        output.Add("SessionID", "NA");
        output.Add("Email", "NA");
        output.Add("Real time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        _loggingManager.Log("Tasks", output);
    }

    private void Save()
    {
        if (_debugMode)
        {
            var directoryInfo = new DirectoryInfo($"{_root}\\Debug");
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            _loggingManager.SetSavePath($"{_root}\\Debug");
        }
        else
        {
            var currentLargestSubDirectoryNumber = new DirectoryInfo($"{_root}").GetDirectories().Where(e => int.TryParse(e.Name,out _)).OrderByDescending(e => e).ToArray();
            DirectoryInfo dirinfo;
            dirinfo = currentLargestSubDirectoryNumber.Length == 0 ? new DirectoryInfo($"{_root}\\{0}") : new DirectoryInfo($"{_root}\\{int.Parse(currentLargestSubDirectoryNumber[0].Name)+1}");
            if (!dirinfo.Exists)
            {
                dirinfo.Create();
            }
            _loggingManager.SetSavePath(dirinfo.FullName);
        }
        _loggingManager.SaveAllLogs(true);
    }
}


