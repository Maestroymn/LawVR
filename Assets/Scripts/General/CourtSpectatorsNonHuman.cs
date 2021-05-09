using UnityEngine;

namespace General
{
    public enum Status
    {
        NervousSitting,
        NormalSitting,
        Writing
    }
    public class CourtSpectatorsNonHuman : MonoBehaviour
    {
        public Status Status;
        public Animator Animator;
        private static readonly int Nervous = Animator.StringToHash("nervous");
        private static readonly int NormalSit = Animator.StringToHash("normal_sit");
        private static readonly int Writing = Animator.StringToHash("writing");

        public void Start()
        {
            SetStartingStatus();
        }

        private void SetStartingStatus()
        {
            Animator.enabled = true;
            switch (Status)
            {
                case Status.NervousSitting:
                    Animator.SetBool(Nervous,true);
                    break;
                case Status.NormalSitting:
                    Animator.SetBool(NormalSit,true);
                    break;
                case Status.Writing:
                    Animator.SetBool(Writing,true);
                    break;
            }
        }
    }
}
