using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DevConsoleManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI consoleText; // Reference to the UI Text component to display the logs
    [SerializeField] GameObject devConsoleObject; // Reference to the GameObject containing the consoleText
    public int maxLogCount = 100; // Maximum number of logs to display

    private Queue<string> logQueue = new Queue<string>();

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Format the log entry to include log type, message, and stack trace
        string logEntry = $"{type}: {logString}";
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert || type == LogType.Warning)
        {
            logEntry += $"\nStack Trace:\n{stackTrace}";
        }

        // Enqueue the log entry to the logQueue
        logQueue.Enqueue(logEntry);

        // Maintain the max log count by dequeuing the oldest entry if necessary
        if (logQueue.Count > maxLogCount)
        {
            logQueue.Dequeue();
        }

        // Update the console text with the latest logs
        consoleText.text = string.Join("\n", logQueue.ToArray());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote)) // Toggle console with the ` key
        {
            devConsoleObject.SetActive(!devConsoleObject.activeSelf);
        }
    }
}
