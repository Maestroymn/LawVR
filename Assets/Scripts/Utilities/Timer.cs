using System;
using TMPro;
using UnityEngine;

namespace Utilities
{
    public class Timer : MonoBehaviour
    {
        public event Action OnTimesUp;
        public float timeRemaining,timeLimit;
        public bool timerIsRunning = false;
        public TMP_Text timeText;
        public Animator Animator;
        private static readonly int Ticking = Animator.StringToHash("Ticking");

        public void HandleTimer(bool isActive)
        {
            Animator.SetBool(Ticking, isActive);
            timeRemaining = timeLimit;
            timerIsRunning = isActive;
            if(!isActive)
                timeText.text = "WAIT";
        }
        
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
                timeRemaining = 0;
                timerIsRunning = false;
                Animator.SetBool(Ticking,false);
                timeText.text = "WAIT";
                OnTimesUp?.Invoke();
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