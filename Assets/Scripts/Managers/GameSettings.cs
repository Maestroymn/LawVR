using UnityEngine;
using Utilities;

namespace Managers
{
    [CreateAssetMenu(menuName = "Manager/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private string _gameVersion = "0.0.0";
        public string GameVersion => _gameVersion;
        [SerializeField] private string _nickName = "User";
        public string NickName
        {
            get
            {
                int value = Random.Range(0, 9999);
                return _nickName + value;
            }
        }
    }
}
