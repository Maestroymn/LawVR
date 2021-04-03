using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GeneralUIBehaviourScripts
{
    public class ConnectionQuality : MonoBehaviour
    {
        [SerializeField] private Image _connectionIcon;
        [SerializeField] private Color _good, _medium, _bad;
        [SerializeField] private TextMeshProUGUI _pingText;
        private int _ping;
        private void Update()
        {
            if (PhotonNetwork.IsConnected)
            {
                UpdateConnectionStatus(PhotonNetwork.GetPing());
            }
        }

        private void UpdateConnectionStatus(int newPing)
        {
            if (_ping == newPing) return;
            _ping = newPing;
            _pingText.text = _ping.ToString();
            if (_ping <= 80 && _ping > 0)
            {
                _connectionIcon.color = _good;
            }else if (_ping > 80 && _ping < 100)
            {
                _connectionIcon.color = _medium;
            }else if (_ping >= 100)
            {
                _connectionIcon.color = _bad;
            }
        }
    }
}
