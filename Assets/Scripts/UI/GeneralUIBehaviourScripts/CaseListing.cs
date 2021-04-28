using System;
using TMPro;
using UI.GeneralUIBehaviourScripts;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CaseListing : MonoBehaviour
    {
        public event Action<CaseListing> OnSelected;
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        public RawImage Image;
        private CaseDetailPanelBehaviour _caseDetail;

        public void SetCaseDetail(CaseDetailPanelBehaviour caseDetail)
        {
            _caseDetail = caseDetail;
        }

        public void SetCaseName(string caseName)
        {
            _textMeshProUGUI.text = caseName;
        }

        public void ShowCaseDetail()
        {
            _caseDetail.OpenUpPanel();
        }
        
        public void OnCaseSelected()
        {
            Image.color=Color.green;
            OnSelected?.Invoke(this);
        }
    }
}
