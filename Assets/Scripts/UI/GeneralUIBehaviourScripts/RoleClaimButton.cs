using UnityEngine;

namespace UI
{
    public class RoleClaimButton : MonoBehaviour
    {
        [SerializeField] private PlayerListingsMenu _playerListingsMenu;
        public enum RoleType
        {
            Judge,
            Defendant,
            Plaintiff,
            Spectator
        }

        public RoleType Role;
        public void OnClick_RoleClaim()
        {
            switch (Role)
            {
                case RoleType.Defendant:
                    _playerListingsMenu.CheckRoleStatusAndSet("Defendant");
                    break;
                case RoleType.Judge:
                    _playerListingsMenu.CheckRoleStatusAndSet("Judge");
                    break;
                case RoleType.Plaintiff:
                    _playerListingsMenu.CheckRoleStatusAndSet("Plaintiff");
                    break;
                case RoleType.Spectator:
                    _playerListingsMenu.CheckRoleStatusAndSet("Spectator");
                    break;
            }
        }
        
    }
}
