using TMPro;
using UnityEngine;

namespace UI.GeneralUIBehaviourScripts
{
    public enum SummaryType
    {
        BeforeSession,
        DuringSession,
    }
    public class CaseSummaryDetail : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _role,_simType,_startTime,_caseName,_turnCount,_turnDuration,_rolesTextField,_caseDetail;
        [SerializeField] private GameObject _readyButton, _closeButton;

        public void Initialize()
        {
            _closeButton.SetActive(false);
            _readyButton.SetActive(true);
            //Here set all the infos 
        }

    }
}
