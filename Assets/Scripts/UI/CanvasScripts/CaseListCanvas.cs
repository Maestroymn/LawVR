using UI.GeneralUIBehaviourScripts;
using UnityEngine;

namespace UI.CanvasScripts
{
    public class CaseListCanvas : MonoBehaviour
    {
        [SerializeField] private CaseListing _caseListingPrefab;
        [SerializeField] private CaseDetailPanelBehaviour _caseDetailPrefab;
        [SerializeField] private Transform _caseContent;
        private int _currentCaseNumber;

        private void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                InstantiateCaseListing();
            }
        }

        public void InstantiateCaseListing()
        {
            CaseListing caseListing;
            _currentCaseNumber++;
            caseListing = Instantiate(_caseListingPrefab);
            caseListing.transform.SetParent(_caseContent,false);
            caseListing.SetCaseName("Case_"+_currentCaseNumber);
            CaseDetailPanelBehaviour detail = Instantiate(_caseDetailPrefab, _caseContent.parent, false);
            caseListing.SetCaseDetail(detail);
        }
        
    }
}
