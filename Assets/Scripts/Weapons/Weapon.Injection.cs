using UnityEngine;
using System.Collections.Generic;
using System;

public partial class Weapon : Parts
{
    /// <summary>
    ///射出孔関連のパラメータ
    /// </summary>
    [Serializable]
    public class Injection
    {
        /// <summary>
        /// 射出孔の座標
        /// </summary>
        public Vector2 hole = Vector2.zero;
        /// <summary>
        /// 弾丸初期角度補正
        /// </summary>
        public float bulletAngle = 0;
        /// <summary>
        /// 射出角補正
        /// </summary>
        public float angle = 0;
        /// <summary>
        /// 初速度
        /// </summary>
        public float initialVelocity = 1;
        /// <summary>
        /// 射出時燃料消費量補正値
        /// </summary>
        public float fuelCostPar = 1;
        /// <summary>
        /// 射出毎の待ち時間の補正
        /// </summary>
        public float shotDelayTweak = 1;
        /// <summary>
        /// 連射数特殊指定
        /// </summary>
        public int burst = 0;
        /// <summary>
        /// 射出弾丸の特殊指定
        /// </summary>
        public Bullet bullet = null;
        /// <summary>
        /// 射出後の弾丸を武装と連結状態にするフラグ
        /// </summary>
        public bool union = false;
        /// <summary>
        /// 弾ブレ度合い補正
        /// </summary>
        public float noAccuracy = 0;
        /// <summary>
        /// 射出時の効果音
        /// </summary>
        public AudioClip se = null;
        /// <summary>
        /// 射出前のチャージエフェクト特殊指定
        /// </summary>
        public Effect charge = null;
        /// <summary>
        /// 射出タイミング特殊指定
        /// </summary>
        public List<ActionType> timing = new List<ActionType>();
    }
}
