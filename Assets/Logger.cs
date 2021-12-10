using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{
    public Text monitor = null;
    public InputField participant = null;

    static Text smonitor = null;
    static InputField sparticipant = null;

    static string path = "";

    private void Awake()
    {
        smonitor = monitor;
        sparticipant = participant;
    }

    public static void Log(string text)
    {
        path = Application.dataPath + "/recordings/" + sparticipant.text + ".txt";
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(Application.dataPath+"/recordings");
            StreamWriter sw = File.CreateText(path);
            sw.Write(string.Format("Time: {1}\nGravity: {0}", Events.curGrav, System.DateTime.Now));
            sw.Flush();
            sw.Close();
        }
        smonitor.text = text + "\n" + smonitor.text;
        File.AppendAllText(path, "\n"+text);
    }
}
