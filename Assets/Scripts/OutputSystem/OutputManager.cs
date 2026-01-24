using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Purchasing.MiniJSON;
using System.IO;
using System.Text;

[Serializable]
public class Output {
    public string text;
    public string code;
    public string objectName;
}

public class OutputManager : Singleton<OutputManager>
{
    private HashSet<OutputHandler> outputHandlers = new HashSet<OutputHandler>();

    [TextArea(4,15)]
    [SerializeField] string testOutput;
    [SerializeField] KeyCode testButton;
    private string filePath;
    [SerializeField] string sceneName;

    void Start()
    {
        // Set the path: Application.persistentDataPath works across platforms
        // string timeStamp = System.DateTime.Now.ToString("HH-mm-ss");
        // filePath = Path.Combine(Application.dataPath + "/StudyLogData/", sceneName + "_"+timeStamp + ".csv");

        // // Optional: Write CSV headers
        // if (!File.Exists(filePath))
        // {
        //     WriteLine("Time,Prompt,Response");
        // }
    }

    private void Update() {
        if (Input.GetKeyDown(testButton) && testOutput != "") {
            ProcessResponse(testOutput);
        }
    }

    public void AddHandler(OutputHandler handler) {
        outputHandlers.Add(handler);
    }

    public void ProcessResponse(string response) {
        Debug.Log("response: \n" + response);
        Output output = JsonUtility.FromJson<Output>(ReplaceCurvedQuotes(response));
        // if the prompt stored in input manager starts with "select", set the "current selection" to the corresponding game object of response.objectName
        GameObject obj = TextTree.Instance.StringToGameobject(output.objectName);
        InputManager inputManager = FindObjectOfType<InputManager>();
        // LogData(inputManager.mostRecentMessage, response);
        // Disable logging for now to reduce file size
        Debug.Log("response: \n" + response);
        Debug.Log("parsedOutput: " + output.code);
        Debug.Log("line count: " + output.code.Count(c => c == '\n'));
        Debug.Log("handlerCount= " + outputHandlers.Count);
        foreach (OutputHandler outputHandler in outputHandlers) {
            outputHandler.HandleOutput(output);
        }
    }



    public static string ReplaceCurvedQuotes(string input)
    {
        string result = input.Replace("“", "\"").Replace("”", "\"");
        return result;
    }

    public void LogData(string prompt, string response)
    {
        string csvLine = string.Join(",", new[] {
            EscapeCsvField(System.DateTime.Now.ToString("HH:mm:ss")),
            EscapeCsvField(prompt),
            EscapeCsvField(response)
        });
        WriteLine(csvLine);
    }

    private void WriteLine(string line)
    {
        using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            sw.WriteLine(line);
        }
    }

    string EscapeCsvField(string field)
    {
        if (field.Contains("\""))
            field = field.Replace("\"", "\"\"");

        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            field = $"\"{field}\"";

        return field;
    }
}
