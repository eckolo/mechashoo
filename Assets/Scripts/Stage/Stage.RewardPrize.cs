using System;
using UnityEngine;

public abstract partial class Stage : Methods
{
    abstract class RewardPrize<Reward> where Reward : Methods
    {
        [SerializeField]
        private RewardTermType termType = default(RewardTermType);
        public RewardTerm termData => GetTerm(termType);
        public abstract Reward entity { get; }
    }
}
