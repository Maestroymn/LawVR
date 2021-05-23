using Data;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CanvasScripts
{
    public class MainSettingsCanvas : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _microphones, _resolutions;
        [SerializeField] private Toggle _vrToggle;
        private bool _contains;
        public void Initialize()
        {
            _vrToggle.isOn = false;
            foreach (var device in Microphone.devices)
            {
                _contains = false;
                _microphones.options.ForEach(x =>
                {
                    if (x.text == device)
                    {
                        _contains = true;
                    }
                });
                if (!_contains)
                {
                    _microphones.options.Add(new TMP_Dropdown.OptionData(device));
                }
            }
        }

        public void ChangeResolution(int val)
        {
            var res = _resolutions.options[val];
            var splitted = res.text.Split('x');
            Screen.SetResolution(int.Parse(splitted[0]),int.Parse(splitted[1]),true);
        }

        public void SetVrEnable(bool val)
        {
            PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__VR_ENABLE__] = _vrToggle.isOn;
            print(PhotonNetwork.LocalPlayer.CustomProperties[DataKeyValues.__VR_ENABLE__]);
        }
    }
}
