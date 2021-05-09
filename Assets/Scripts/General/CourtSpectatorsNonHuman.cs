using UnityEngine;

namespace General
{
    public enum Status
    {
        NervousSitting,
        NormalSitting,
    }
    public class CourtSpectatorsNonHuman : MonoBehaviour
    {
        public Status Status;
        public Animator Animator;
        private static readonly int Nervous = Animator.StringToHash("nervous");
        private static readonly int NormalSit = Animator.StringToHash("normal_sit");

        public void Start()
        {
            SetStartingStatus();
        }

        private void SetStartingStatus()
        {
            switch (Status)
            {
                case Status.NervousSitting:
                    Animator.SetBool(Nervous,true);
                    break;
                case Status.NormalSitting:
                    Animator.SetBool(NormalSit,true);
                    break;
            }
        }
    }
}
