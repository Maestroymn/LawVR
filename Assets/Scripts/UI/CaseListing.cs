using TMPro;
using UnityEngine;

namespace UI
{
    public class CaseListing : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;

        public void SetCaseName(string caseName)
        {
            _textMeshProUGUI.text = caseName;
        }
    }
}
