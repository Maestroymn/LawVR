using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GeneralUIBehaviourScripts
{
    public class CaseDetailPanelBehaviour : MonoBehaviour
    { 
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Button _closeButton;
        private void OnEnable()
        {
            OpenUpPanel();
        }

        public void OpenUpPanel()
        {
            gameObject.SetActive(true);
            _rectTransform.LeanScale(Vector3.one, .5f).setOnComplete(() =>
            {
                _closeButton.interactable = true;
            });
        }
        
        public void CloseDetailPanel()
        {
            _closeButton.interactable = false;
            _rectTransform.LeanScale(Vector3.zero, .5f);
        }
    }
}
