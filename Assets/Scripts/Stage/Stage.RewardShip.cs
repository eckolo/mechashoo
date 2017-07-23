using System;
using UnityEngine;

public abstract partial class Stage : Methods
{
    /// <summary>
    /// 報酬機体クラス
    /// </summary>
    [Serializable]
    private class RewardShip : RewardPrize<Ship>
    {
        [SerializeField]
        private Ship _entity;
        public override Ship entity => _entity;
        public bool isPossessed => sys.possessionShips.Contains(_entity);
    }
}
