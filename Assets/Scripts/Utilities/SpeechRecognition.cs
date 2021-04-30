﻿using DatabaseScripts;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Data;
using Managers;
using Photon.Pun;
using UnityEngine;

namespace Utilities
{
    public class SpeechRecognition : MonoBehaviour
    {

        string PythonScriptPath;
        string PythonExePath;
        string WorkingDirectory;
        char DirSep;
        Thread PythonThread;
        // Start is called before the first frame update
        void Start()
        {
            DirSep = Path.DirectorySeparatorChar;
            PythonScriptPath = @Application.dataPath + DirSep + "Python"+ DirSep + "listen_user_test.py";


            WorkingDirectory = @Application.dataPath + DirSep + "Python";
            PythonExePath = @Application.dataPath + DirSep + "Python" + DirSep + "Python27"+  DirSep+  "python.exe";


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
                    if(result.Contains("change python") || (result.Length==0 && stderr.Length==0))
                    {
                        UnityEngine.Debug.Log("exception " + stderr);
                        UnityEngine.Debug.Log("result " + result);
                        WorkingDirectory = "C:" + DirSep + "Python27";
                        PythonExePath = "C:" + DirSep + "Python27" + DirSep + "python.exe";
                        RunPythonListenerScript();
                    }else 
                    {
                        UnityEngine.Debug.Log("exception " + stderr);
                        UnityEngine.Debug.Log("result " + result);
                        string[] Words = result.Split('\n');
                        string Speech = "";
                        string StartTime = "";
                        string SpeechDuration ="";
                        
                        foreach (string Word in Words)
                        {
                            UnityEngine.Debug.Log(Word);
                            if (Word.StartsWith("Speech*"))
                            {

                                Speech = String.Join("\'\'",Word.Split('*')[1].Split('\''));
                                
                                UnityEngine.Debug.Log(Speech);

                            }
                            else if (Word.StartsWith("StartTime*"))
                            {
                                
                                StartTime = Word.Split('*')[1];
                                UnityEngine.Debug.Log(StartTime);

                            }
                            
                            else if (Word.StartsWith("Duration*"))
                            {
         
                                SpeechDuration = Word.Split('*')[1];

                            
                            }
                        }

                        DatabaseConnection.UploadSpeech("SESSION_ID",GameManager.GameSettings.NickName,PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__ROLE__].ToString(), Speech, StartTime, SpeechDuration);

                    }
                    
                    
                }  
            }     
        }

    }
}
