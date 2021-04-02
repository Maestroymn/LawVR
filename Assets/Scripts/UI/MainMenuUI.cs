using UnityEngine;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public void HideMainMenu()
        {
            gameObject.SetActive(false);    
        }
        
        public void Show()
        {
            gameObject.SetActive(true);    
        }
        
    }
}
