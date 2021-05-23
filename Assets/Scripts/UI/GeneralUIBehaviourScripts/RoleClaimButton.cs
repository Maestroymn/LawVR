using Data;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class RoleClaimButton : MonoBehaviourPunCallbacks
    {
        [SerializeField] private PlayerListingsMenu _playerListingsMenu;
        [SerializeField] private Image _background, _icon;
        public bool HaveExtraIcon;

        [ConditionalShowInInspector("HaveExtraIcon", true)] [SerializeField]
        private GameObject _extraIconParent;
        public bool IsValid;
        public enum RoleType
        {
            Judge,
            Defendant,
            Plaintiff,
            Spectator
        }

        public void Initialize()
        {
            switch (Role)
            {
                case RoleType.Defendant:
                    IsValid = true;
                    break;
                case RoleType.Judge:
                    if ((bool) PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__AI_JUDGE__])
                    {
                        MakeRoleInvalid();
                        if (_extraIconParent != null)
                        {
                            _extraIconParent.SetActive(true);
                        }
                    }
                    else
                    {
                        IsValid = true;
                    }
                    break;
                case RoleType.Plaintiff:
                    IsValid = true;
                    break;
                case RoleType.Spectator:
                    IsValid = true;
                    break;
            }
        }

        public void MakeRoleInvalid()
        {
            IsValid = false;
            var tmpColor = _background.color;
            tmpColor.a = .5f;
            _background.color = tmpColor;
            tmpColor = _icon.color;
            tmpColor.a = .5f;
            _icon.color = tmpColor;
        }

        public RoleType Role;
        public void OnClick_RoleClaim()
        {
            if (!IsValid) return;
            switch (Role)
            {
                case RoleType.Defendant:
                    _playerListingsMenu.CheckRoleStatusAndSet(DataKeyValues.__DEFENDANT__);
                    break;
                case RoleType.Judge:
                    IsValid = false;
                    _playerListingsMenu.CheckRoleStatusAndSet(DataKeyValues.__JUDGE__);
                    break;
                case RoleType.Plaintiff:
                    _playerListingsMenu.CheckRoleStatusAndSet(DataKeyValues.__PLAINTIFF__);
                    break;
                case RoleType.Spectator:
                    _playerListingsMenu.CheckRoleStatusAndSet(DataKeyValues.__SPECTATOR__);
                    break;
            }
        }

    }
}
