using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Scripting.Python;
using System.Threading;




public class deleteme : MonoBehaviour
{
    string PythonScriptPath ;
    Thread PythonThread;
    // Start is called before the first frame update
    void Start()
    {
        PythonScriptPath = Application.dataPath + "/Python/listen_user_test.py";
        PythonThread = new Thread(RunPythonListenerScript);
        PythonThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RunPythonListenerScript()
    {
        
        PythonRunner.RunFile(PythonScriptPath);

        Debug.Log("Python işi bitti");
        PythonThread.Abort();
    }

}
