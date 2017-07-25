using System;
using UnityEngine;

public abstract partial class Stage : Methods
{
    /// <summary>
    /// 報酬武装クラス
    /// </summary>
    [Serializable]
    private class RewardWeapon : RewardPrize<Weapon>
    {
        [SerializeField]
        private Weapon _entity = null;
        public override Weapon entity => _entity;
        public bool isPossessed => sys.possessionWeapons.Contains(_entity);
    }
}
