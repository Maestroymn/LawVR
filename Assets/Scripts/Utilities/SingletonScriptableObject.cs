using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Utilities
{
    public abstract class SingletonScriptableObject<TYpe> : ScriptableObject where TYpe : ScriptableObject
    {
        private static TYpe _instance = null;
        public static TYpe Instance
        {
            get
            {
                if (_instance == null)
                {
                    TYpe[] results = Resources.FindObjectsOfTypeAll<TYpe>();
                    if (results.Length == 0)
                    {
                        Debug.LogError("SingletonScriptableObject: No result can be found for "+typeof(TYpe));
                        return null;
                    }

                    if (results.Length > 1)
                    {
                        Debug.LogError("SingletonScriptableObject: Multiple references for "+typeof(TYpe));
                        return null;
                    }
                    _instance = results[0];
                    _instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
                }
                return _instance;
            }
        }
    }
}
