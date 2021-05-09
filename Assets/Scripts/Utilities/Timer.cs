using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class Timer : MonoBehaviour
    {
        public float timeRemaining = 60;
        public bool timerIsRunning = false;
        public Text timeText;

        void Update()
        {
            if (!timerIsRunning) return;
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                //Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }

        private void DisplayTime(float timeToDisplay)
        {
            timeToDisplay += 1;

            float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);

            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}