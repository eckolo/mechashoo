using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 武装クラス
/// </summary>
public class Weapon : Parts
{
    /// <summary>
    ///現在攻撃動作可能かどうかの判定フラグ
    /// </summary>
    [System.NonSerialized]
    public bool canAction = true;
    /// <summary>
    ///持ち手の座標
    /// </summary>
    public Vector2 handlePosition = Vector2.zero;
    /// <summary>
    ///射出孔関連のパラメータ
    /// </summary>
    [System.Serializable]
    public class InjectionHole
    {
        /// <summary>
        ///射出孔の座標
        /// </summary>
        public Vector2 hole = Vector2.zero;
        /// <summary>
        ///射出角補正
        /// </summary>
        public float angle = 0;
        /// <summary>
        ///射出弾丸の特殊指定
        /// </summary>
        public Bullet bullet = null;
        /// <summary>
        ///射出タイミング特殊指定
        /// </summary>
        public List<ActionType> timing = new List<ActionType>();
    }
    public List<InjectionHole> injections = new List<InjectionHole>();
    /// <summary>
    /// 弾のPrefab
    /// </summary>
    public Bullet Bullet;

    /// <summary>
    ///攻撃動作開始可能かどうか(つまり動作中か否か)の内部フラグ
    /// </summary>
    [System.NonSerialized]
    protected bool notInAction = true;

    /// <summary>
    ///1モーションの所要時間
    /// </summary>
    [SerializeField]
    protected int timeRequired;
    /// <summary>
    /// アクション毎の間隔
    /// </summary>
    public int actionDelay;
    /// <summary>
    ///弾丸密度
    /// </summary>
    [SerializeField]
    protected int density = 1;
    /// <summary>
    ///デフォルトの向き
    /// </summary>
    [SerializeField]
    private float defAngle = 0;
    /// <summary>
    ///外部由来の基礎角度
    /// </summary>
    private float baseAngle = 0;
    /// <summary>
    ///Handに対しての描画順の前後のデフォルト値
    /// </summary>
    public int defaultZ = -1;
    /// <summary>
    ///起動時燃料基準値
    /// </summary>
    public float motionFuelCost = 1;
    /// <summary>
    ///射出時燃料基準値
    /// </summary>
    public float injectionFuelCost = 1;

    /// <summary>
    ///次のモーションの内部指定値
    /// </summary>
    private ActionType nextAction = ActionType.NoMotion;

    public override void Start()
    {
        base.Start();
        setAngle(baseAngle + defAngle);
    }

    public override void Update()
    {
        base.Update();
        if (inAction)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 0.6f, 0.8f, 1);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
        }
    }

    public float setBaseAngle(float setedAngle)
    {
        return baseAngle = setedAngle;
    }
    public override float setAngle(float settedAngle)
    {
        return base.setAngle(baseAngle + settedAngle);
    }

    public bool inAction
    {
        get
        {
            if (parentMaterial == null) return !notInAction;
            if (parentMaterial.GetComponent<Weapon>() == null) return !notInAction;
            return parentMaterial.GetComponent<Weapon>().inAction;
        }
    }

    public enum ActionType
    {
        NoMotion,
        Nomal,
        Sink,
        Fixed,
        Npc
    }
    public bool Action(ActionType action = ActionType.Nomal)
    {
        if (!canAction || !notInAction || action == ActionType.NoMotion)
        {
            nextAction = action;
            return false;
        }

        notInAction = false;
        return base.Action((int)action);
    }
    protected override IEnumerator baseMotion(int actionNum)
    {
        bool normalOperation = reduceShipFuel(motionFuelCost);

        if (normalOperation) yield return base.baseMotion(actionNum);

        var timerKey = "weaponEndMotion";
        timer.start(timerKey);
        if (normalOperation) yield return endMotion();
        if (actionDelay > 0) yield return wait(actionDelay - timer.get(timerKey));
        timer.stop(timerKey);

        notInAction = true;
        if (nextAction != ActionType.NoMotion)
        {
            Action(nextAction);
            nextAction = ActionType.NoMotion;
        }

        yield break;
    }

    protected override IEnumerator Motion(int actionNum)
    {
        yield return Motion((ActionType)actionNum);
        yield break;
    }
    protected virtual IEnumerator Motion(ActionType action)
    {
        injection((int)action);
        yield break;
    }
    protected virtual IEnumerator endMotion()
    {
        yield break;
    }

    protected bool reduceShipFuel(float reduceValue, float fuelCorrection = 1)
    {
        Ship rootShip = nowParent.GetComponent<Ship>();
        if (rootShip == null) return true;
        return rootShip.reduceFuel(reduceValue * fuelCorrection);
    }

    /// <summary>
    /// 弾の作成
    /// 武装毎の射出孔番号で指定するタイプ
    /// </summary>
    protected Bullet injection(int injectionNum = 0, float fuelCorrection = 1, Bullet injectionBullet = null)
    {
        if (injectionBullet ?? Bullet == null) return null;

        if (!reduceShipFuel(injectionFuelCost, fuelCorrection)) return injectionBullet ?? Bullet;

        if (injections.Count <= 0) return null;
        injectionNum = injectionNum % injections.Count;

        return injection(injectionBullet ?? Bullet, injections[injectionNum].hole, injections[injectionNum].angle);
    }
}
