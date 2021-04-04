using UnityEngine;

namespace Managers
{
    [CreateAssetMenu(menuName = "Manager/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private string _gameVersion = "0.0.0";
        public string GameVersion => _gameVersion;
        public string NickName;

    }
}
