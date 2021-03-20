
#if COMPILE
using UnityEngine;
using UnityEditor;
using UnityEditor.Scripting.Python;
[CustomEditor(typeof(python_manager))]
    public class python_manager_editor : Editor
    {

        python_manager targetManager;

        private void OnEnable()
        {
            targetManager = (python_manager)target;
        }


        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Launch Python Script", GUILayout.Height(35)))
            {
                string path = Application.dataPath + "/Python/listen_user_test.py";
                PythonRunner.RunFile(path);
            
            }
            else if (GUILayout.Button("Stop Python Script", GUILayout.Height(35)))
            {

                Debug.Log(PythonRunner.GetConnectedClients());

            }
            
        }

    }

#endif

