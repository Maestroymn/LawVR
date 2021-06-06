﻿using Data;
using DatabaseScripts;
using Photon.Pun;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace AI
{
    public class AIJudgeGeneralBehaviour : MonoBehaviour
    {
        static string PythonScriptPath;
        static string PythonExePath;
        static string WorkingDirectory;
        static char DirSep;

        public static void AIJudgeDecisionCaller(string session_id, string speaker_role, string speaker_name)
        {
            DirSep = Path.DirectorySeparatorChar;
            PythonScriptPath = @Application.dataPath + DirSep + "Python" + DirSep + "LawVRJudge.py";


            WorkingDirectory = @Application.dataPath + DirSep + "Python";
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            PythonExePath = @Application.dataPath + DirSep + "Python" + DirSep + "Python38" + DirSep + "python.exe";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            WorkingDirectory = "/usr/bin";
            PythonExePath = WorkingDirectory + DirSep + "python";
#endif

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = PythonExePath;
            start.Arguments = $"\"{PythonScriptPath}\"  " + session_id + "  " + speaker_role + " " +speaker_name;
            UnityEngine.Debug.Log(session_id + "  " + speaker_role);
            start.WorkingDirectory = WorkingDirectory;

            start.UseShellExecute = false;// Do not use OS shell
            start.CreateNoWindow = true; // We don't need new window
            start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
            start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)

            using (Process process = Process.Start(start))
            {
                UnityEngine.Debug.Log("AI thread started");
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                    string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")

                    UnityEngine.Debug.Log("exception " + stderr);
                    UnityEngine.Debug.Log("result " + result);


                    UserFeedback Feedback = DatabaseConnection.RetrieveFeedback(session_id);
                    UnityEngine.Debug.Log(Feedback.NegativeKeywords+ " " + Feedback.PositiveKeywords+ " " + Feedback.Result + " " + Feedback.SessionID + " " + Feedback.UserName + " " + Feedback.UserRole +" " + Feedback.FeedbackID );
                    Feedback.ToString();

                }



            }


            DatabaseConnection.UpdateSessionLog(PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__SESSION_ID__].ToString(), DateTime.Now.ToString());
        }

    }
}
