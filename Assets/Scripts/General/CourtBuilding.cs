using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace General
{
    public class CourtBuilding : MonoBehaviour
    {
        public Transform DefendantTransform,PlaintiffTransform,JudgeTransform,SpectatorTransform;
        public Timer DefendantTimer, PlaintiffTimer;
        public InteractableCourtObject PlaintiffStartButton, DefendantStartButton;
        public List<InteractableCourtObject> InteractableCourtObjects;

        private void Awake()
        {
            InteractableCourtObjects = GetComponentsInChildren<InteractableCourtObject>().ToList();
        }

        public void InitTimers(float timeLimit)
        {
            DefendantTimer.timeLimit = timeLimit;
            DefendantTimer.timeText.text = "WAIT";
            PlaintiffTimer.timeText.text = "WAIT";
            PlaintiffTimer.timeLimit = timeLimit;
        }
    }
}
