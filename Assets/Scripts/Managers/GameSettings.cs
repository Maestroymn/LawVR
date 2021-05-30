using AdvancedCustomizableSystem;
using UnityEngine;

namespace Managers
{
    public enum Gender
    {
        Male,
        Female
    }
    [CreateAssetMenu(menuName = "Manager/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private string _gameVersion = "0.0.0";
        public string GameVersion => _gameVersion;
        public string NickName;
        public string Password;
        public string Mail;
        public Gender Gender;
        public string UserID;
        public CharacterCustomizationSetup CharacterCustomizationSetup;
        public string PublicSelectedRoomName;
    }
}
