using System.Collections.Generic;
using Photon.Pun;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Utilities;

namespace Managers
{
    [CreateAssetMenu(menuName = "Singleton/GameManager")]
    public class GameManager : SingletonScriptableObject<GameManager>
    {
        [SerializeField] private GameSettings _gameSettings;
        public static GameSettings GameSettings => Instance._gameSettings;

        [SerializeField] private List<NetworkedPrefab> _networkedPrefabs = new List<NetworkedPrefab>();
        
        public static GameObject NetworkInstantiate(GameObject obj, Vector3 position, Quaternion rotation)
        {
            GameObject result = null;
            Instance._networkedPrefabs.ForEach(networkPrefab =>
            {
                if (networkPrefab.Prefab == obj && networkPrefab.Path!=string.Empty)
                {
                    result = PhotonNetwork.Instantiate(networkPrefab.Path, position, rotation);
                }
                else
                {
                    Debug.LogWarning("Path is empty for gameobject: "+networkPrefab.Prefab);
                }
            });
            return result;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void PopulateNetworkedPrefabs()
        {
#if UNITY_EDITOR
            Instance._networkedPrefabs.Clear();
            GameObject[] results = Resources.LoadAll<GameObject>("");
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].GetComponent<PhotonView>() != null)
                {
                    string path = AssetDatabase.GetAssetPath(results[i]);
                    Instance._networkedPrefabs.Add(new NetworkedPrefab(results[i],path));
                }
            }
#endif
        }
    }
}
