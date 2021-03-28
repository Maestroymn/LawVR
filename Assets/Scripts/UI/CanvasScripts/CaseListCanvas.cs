using UnityEngine;

namespace UI.CanvasScripts
{
    public class CaseListCanvas : MonoBehaviour
    {
        [SerializeField] private CaseListing _caseListingPrefab;
        [SerializeField] private Transform _leftContentParent, _rightContentParent;
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
            if (_currentCaseNumber % 2 == 0)
            {
                caseListing = Instantiate(_caseListingPrefab, _rightContentParent);
            }
            else
            {
                caseListing = Instantiate(_caseListingPrefab, _leftContentParent);
            }
            caseListing.SetCaseName("Case_"+_currentCaseNumber);
        }
    }
}
