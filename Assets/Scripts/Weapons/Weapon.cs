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
    public bool canAction = true;
    /// <summary>
    ///持ち手の座標
    /// </summary>
    public Vector2 handlePosition = Vector2.zero;
    /// <summary>
    ///射出孔のリスト
    /// </summary>
    public List<Vector2> injectionHoles = new List<Vector2>();
    /// <summary>
    ///各射出孔の射出角度補正
    /// </summary>
    [SerializeField]
    protected List<float> injectionAngles = new List<float>();
    /// <summary>
    /// 弾のPrefab
    /// </summary>
    public Bullet Bullet;

    /// <summary>
    ///攻撃動作開始可能かどうか(つまり動作中か否か)の内部フラグ
    /// </summary>
    [SerializeField]
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
    ///デフォルトのモーション番号
    /// </summary>
    [SerializeField]
    private int defActionNum = 0;
    /// <summary>
    ///デフォルトの向き
    /// </summary>
    [SerializeField]
    private float defAngle = 0;
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

    public override void Start()
    {
        base.Start();
        setAngle(defAngle);
        for (int i = 0; i < injectionHoles.Count; i++)
        {
            if (injectionAngles.Count <= i) injectionAngles.Add(0);
        }
    }

    public override void Update()
    {
        base.Update();
        if (inAction())
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 0.6f, 0.8f, 1);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
        }
    }

    public bool inAction()
    {
        if (parentMaterial == null) return !notInAction;
        if (parentMaterial.GetComponent<Weapon>() == null) return !notInAction;
        return parentMaterial.GetComponent<Weapon>().inAction();
    }

    public enum ActionType
    {
        Nomal, Sink, Fixed, Npc
    }
    public bool Action(ActionType action = ActionType.Nomal)
    {
        if (!canAction) return false;
        if (!notInAction) return false;

        notInAction = false;
        return base.Action((int)action);
    }
    protected override IEnumerator baseMotion(int actionNum)
    {
        bool normalOperation = reduceShipFuel(motionFuelCost);

        if (normalOperation) yield return base.baseMotion(actionNum);

        if (actionDelay > 0) yield return wait(actionDelay);
        if (normalOperation) yield return endMotion();

        notInAction = true;

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
        Ship rootShip = getParent().GetComponent<Ship>();
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

        if (injectionHoles.Count <= 0) return null;
        injectionNum = injectionNum % injectionHoles.Count;

        return injection(injectionBullet ?? Bullet, injectionHoles[injectionNum], injectionAngles[injectionNum]);
    }
}
