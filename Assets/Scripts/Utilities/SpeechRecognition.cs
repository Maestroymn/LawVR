﻿using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Utilities
{
    public class SpeechRecognition : MonoBehaviour
    {
        string PythonScriptPath;
        string PythonExePath;
        string WorkingDirectory;
        char DirSeperatorChar;
        Thread PythonThread;
        // Start is called before the first frame update
        void Start()
        {
            DirSeperatorChar = Path.DirectorySeparatorChar;
            UnityEngine.Debug.Log("lets do something");

            PythonScriptPath = @Application.dataPath + DirSeperatorChar + "Python"+ DirSeperatorChar + "listen_user_test.py";
            WorkingDirectory = @Application.dataPath + DirSeperatorChar + "Python";

            PythonExePath = @Application.dataPath + DirSeperatorChar + "Python" + DirSeperatorChar + "Scripts" + DirSeperatorChar+  "python.exe";
            PythonThread = new Thread(RunPythonListenerScript);
            PythonThread.Start();
            using (StreamWriter sw = File.CreateText("WriteLines2.txt"))
            {
                sw.WriteLine(PythonScriptPath + "\n" + PythonExePath);
                sw.Close();
            }
        }


        public void RunPythonListenerScript()
        {
            UnityEngine.Debug.Log( string.Format("\"{0}\" ", PythonScriptPath));
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = PythonExePath;
            start.Arguments = $"\"{PythonScriptPath}\"";
            start.WorkingDirectory = WorkingDirectory;
        
            start.UseShellExecute = false;// Do not use OS shell
            start.CreateNoWindow = true; // We don't need new window
            start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
            start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)


            using (Process process = Process.Start(start))
            {

                UnityEngine.Debug.Log("python thread started");
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                    string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                    UnityEngine.Debug.Log("exception " + stderr);
                    UnityEngine.Debug.Log("result " + result);
                }
                
            }


            UnityEngine.Debug.Log("lets do something1");

      
        }

    }
}
