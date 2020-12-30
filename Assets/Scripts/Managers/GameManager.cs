using UnityEngine;
using Utilities;

namespace Managers
{
    [CreateAssetMenu(menuName = "Singleton/GameManager")]
    public class GameManager : SingletonScriptableObject<GameManager>
    {
        [SerializeField] private GameSettings _gameSettings;
        public static GameSettings GameSettings => Instance._gameSettings;
    }
}
