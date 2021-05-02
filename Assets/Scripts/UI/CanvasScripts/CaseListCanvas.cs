using System.Collections.Generic;
using Data;
using DatabaseScripts;
using Photon.Pun;
using UI.GeneralUIBehaviourScripts;
using UnityEngine;

namespace UI.CanvasScripts
{
    public class CaseListCanvas : MonoBehaviour
    {
        [HideInInspector] public CaseListing SelectedCase;
        [SerializeField] private CaseListing _caseListingPrefab;
        [SerializeField] private CaseDetailPanelBehaviour _caseDetailPrefab;
        [SerializeField] private Transform _caseContent;
        private List<CaseListing> _caseListings=new List<CaseListing>();
        private List<CourtCase> _courtCases;
        
        public void Initialize()
        {
            _courtCases = DatabaseConnection.GetCourtCases();
            _courtCases?.ForEach(court=>
            {
                if(!_caseListings.Find(x=>x.CourtCase.CaseID==court.CaseID))
                {
                    InstantiateCaseListing(court);
                }
            });
        }

        public void InstantiateCaseListing(CourtCase courtCase)
        {
            CaseListing caseListing;
            caseListing = Instantiate(_caseListingPrefab);
            caseListing.CourtCase = courtCase;
            caseListing.OnSelected += SetSelectedCase;
            caseListing.transform.SetParent(_caseContent,false);
            caseListing.SetCaseName(courtCase.CaseName);
            CaseDetailPanelBehaviour detail = Instantiate(_caseDetailPrefab, _caseContent.parent, false);
            detail.SetInfo(courtCase);
            caseListing.SetCaseDetail(detail);
        }

        private void SetSelectedCase(CaseListing selectedCase)
        {
            if (SelectedCase)
            {
                SelectedCase.Image.color=Color.white;
            }
            SelectedCase = selectedCase;
            PhotonNetwork.CurrentRoom.CustomProperties[DataKeyValues.__CASE_ID__] = SelectedCase.CourtCase.CaseID;
        }
    }
}
