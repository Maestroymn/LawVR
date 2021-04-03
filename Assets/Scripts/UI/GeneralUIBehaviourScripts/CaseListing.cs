﻿using TMPro;
using UI.GeneralUIBehaviourScripts;
using UnityEngine;

namespace UI
{
    public class CaseListing : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        [SerializeField] private CaseDetailPanelBehaviour _caseDetail;
        
        public void SetCaseName(string caseName)
        {
            _textMeshProUGUI.text = caseName;
        }

        public void ShowCaseDetail()
        {
            _caseDetail.OpenUpPanel();
        }
    }
}
