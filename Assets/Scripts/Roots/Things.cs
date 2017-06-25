using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 概ね当たり判定を持つ物体全般
/// </summary>
public class Things : Materials
{
    /// <summary>
    /// 制御下のPartsリスト
    /// </summary>
    private List<Parts> childPartsList = new List<Parts>();

    /// <summary>
    /// 画面内に位置強制するフラグ
    /// </summary>
    protected virtual bool forcedInScreen
    {
        get {
            return false;
        }
    }

    /// <summary>
    /// 物体重量
    /// </summary>
    [SerializeField]
    private float _weight = 1;
    public float weight
    {
        get {
            return _weight;
        }
        protected set {
            _weight = value;
        }
    }

    protected override void UpdateMotion()
    {
        UpdatePosition();
        base.UpdateMotion();
    }

    public override void Start()
    {
        base.Start();
        AttachPolygonCollider();
        if(nowLayer == Configs.Layers.DEFAULT) nowLayer = parentMethod != null ? parentMethod.nowLayer : Configs.Layers.NEUTRAL;
        foreach(Transform child in transform)
        {
            var childParts = child.GetComponent<Parts>();
            if(childParts != null) SetParts(childParts);
        }
        foreach(var childParts in childPartsList) childParts.nowRoot = this;
    }

    public int SetParts(Parts setedParts)
    {
        if(setedParts == null) return -1;

        if(!childPartsList.Contains(setedParts)) childPartsList.Add(setedParts);
        setedParts.nowRoot = this;

        return childPartsList.IndexOf(setedParts);
    }
    public Parts GetParts(int index)
    {
        if(index < 0) return null;
        if(index >= childPartsList.Count) return null;

        return childPartsList[index];
    }
    public Component GetParts<Component>(int index) where Component : Parts
    {
        var parts = GetParts(index);
        if(parts == null) return null;
        return parts.GetComponent<Component>();
    }
    public List<Parts> getPartsList
    {
        get {
            return childPartsList;
        }
    }
    public int partsListCount
    {
        get {
            return childPartsList.Count;
        }
    }

    /// <summary>
    /// PolygonCollider2Dコンポーネントをアタッチするだけの関数
    /// </summary>
    protected PolygonCollider2D AttachPolygonCollider()
    {
        foreach(var collider2D in GetComponents<PolygonCollider2D>()) Destroy(collider2D);
        var collider = gameObject.AddComponent<PolygonCollider2D>();

        collider.isTrigger = true;

        return collider;
    }

    /// <summary>
    ///オブジェクトが可動範囲内にいるかどうか
    /// </summary>
    public bool inField
    {
        get {
            if(globalPosition.x < fieldLowerLeft.x) return false;
            if(globalPosition.x > fieldUpperRight.x) return false;
            if(globalPosition.y < fieldLowerLeft.y) return false;
            if(globalPosition.y > fieldUpperRight.y) return false;
            return true;
        }
    }

    /// <summary>
    ///強制停止関数
    /// </summary>
    public void StopMoving(float? acceleration = null)
    {
        if(acceleration == null) nowSpeed = Vector2.zero;
        else SetVerosity(nowSpeed, 0, acceleration);
    }
    /// <summary>
    ///自然停止ラッパー関数
    /// </summary>
    /// <param name="power">停止加力量</param>
    /// <returns>結果速度</returns>
    public Vector2 Stopping(float power) => ExertPower(nowSpeed, power, 0);
    /// <summary>
    /// オブジェクトへ力を掛ける関数
    /// </summary>
    /// <param name="direction">力のかかる方向</param>
    /// <param name="power">力の大きさ</param>
    /// <param name="targetSpeed">最終目標速度</param>
    /// <returns>結果速度</returns>
    public virtual Vector2 ExertPower(Vector2 direction, float power, float? targetSpeed = null)
    {
        float acceleration = power / weight;

        var setSpeed = targetSpeed ?? (nowSpeed + direction.normalized * acceleration).magnitude;
        return SetVerosity(direction, setSpeed, acceleration);
    }
    /// <summary>
    /// オブジェクトへ力を掛ける関数
    /// </summary>
    public Vector2 ExertPower(float direction, float power, float? targetSpeed = null) => ExertPower(direction.ToVector(), power, targetSpeed);
    /// <summary>
    ///オブジェクトへ力を掛け続けた場合の最終速度予測値取得
    /// </summary>
    public Vector2 GetExertPowerResult(Vector2 direction, float power, int time)
    {
        var result = nowSpeed;
        for(int index = 0; index < time; index++)
        {
            var targetSpeed = result + direction * power / weight;
            result = GetVerosity(targetSpeed, targetSpeed.magnitude, null, result);
        }
        return result;
    }
    /// <summary>
    ///オブジェクトの移動関数
    /// </summary>
    public Vector2 SetVerosity(Vector2 speed) => SetVerosity(speed, speed.magnitude);
    /// <summary>
    ///オブジェクトの移動関数
    /// </summary>
    public Vector2 SetVerosity(Vector2 verosity, float speed, float? acceleration = null)
    {
        Vector2 originSpeed = nowSpeed;

        //速度設定
        nowSpeed = GetVerosity(verosity, speed, acceleration);

        //移動時アクション呼び出し
        SetVerosityAction(nowSpeed - originSpeed);
        return nowSpeed;
    }
    /// <summary>
    ///オブジェクトの移動量取得関数
    /// </summary>
    private Vector2 GetVerosity(Vector2 verosity, float speed, float? acceleration, Vector2? originSpeed = null)
    {
        Vector2 baseSpeed = originSpeed ?? nowSpeed;
        Vector2 degree = verosity.ToVector(speed) - baseSpeed;
        var length = degree.magnitude;
        float variation = length != 0
            ? Mathf.Clamp(Mathf.Min(acceleration ?? length, length) / length, -1, 1)
            : 0;

        return baseSpeed + degree * variation;
    }
    protected virtual void SetVerosityAction(Vector2 acceleration) { }
    /// <summary>
    /// 現在の速度
    /// </summary>
    public virtual Vector2 nowSpeed { private set; get; }
    /// <summary>
    /// 1フレーム前の速度
    /// </summary>
    public virtual Vector2 preSpeed { private set; get; }
    void UpdatePosition()
    {
        if(nextDestroy) return;
        preSpeed = nowSpeed;
        var result = position + nowSpeed.Rescaling(baseMas);
        if(forcedInScreen) result = result.Within(fieldLowerLeft, fieldUpperRight);
        position = result;
    }
    /// <summary>
    /// ぶつかった瞬間に呼び出される
    /// </summary>
    protected virtual void OnTriggerEnter2D(Collider2D target)
    {
        if(!OnEnter(target)) return;

        var thing = target.GetComponent<Things>();
        var impact = preSpeed * weight + OnCrashImpact(thing);
        var impactResult = (impact * 100).Log() / 100;
        if(thing.isSolid) thing.ExertPower(impactResult, impact.magnitude);
    }
    protected virtual Vector2 OnCrashImpact(Things target) => Vector2.zero;
    protected bool OnEnter(Collider2D target)
    {
        if(!ableEnter) return false;
        if(target == null) return false;

        var thing = target.GetComponent<Things>();
        if(thing == null) return false;
        if(!thing.ableEnter) return false;

        return true;
    }
    protected bool OnEnter(Things target)
    {
        if(target == null) return false;
        return OnEnter(target.GetComponent<Collider2D>());
    }
    [SerializeField]
    bool _ableEnter = true;
    public bool ableEnter
    {
        get {
            if(!inField) return false;
            var parts = GetComponent<Parts>();
            if(parts != null && parts.nowRoot != null) return false;
            return _ableEnter;
        }
    }
    [SerializeField]
    bool _isSolid = true;
    public bool isSolid
    {
        get {
            return _isSolid;
        }
    }

    /// <summary>
    ///奥行き位置の設定
    /// </summary>
    public override float nowZ
    {
        get {
            return base.nowZ;
        }

        set {
            base.nowZ = value;
            foreach(var childParts in childPartsList) childParts.nowZ = value;
        }
    }

    /// <summary>
    ///最寄りの非味方機体検索関数
    /// </summary>
    protected Ship nowNearTarget
    {
        get {
            Terms<Ship> term = target => target.nowLayer != nowLayer && target.inField;
            return GetNearObject(term).FirstOrDefault();
        }
    }

    /// <summary>
    ///PartsListの削除関数
    ///引数無しで全消去
    /// </summary>
    public void DeleteParts(int? sequenceNum = null)
    {
        if(sequenceNum != null) DeleteSimpleParts((int)sequenceNum);
        for(int partsNum = 0; partsNum < childPartsList.Count; partsNum++)
        {
            DeleteSimpleParts(partsNum);
        }
        childPartsList = new List<Parts>();
    }
    /// <summary>
    ///PartsListから指定した番号のPartsを削除する
    /// </summary>
    private void DeleteSimpleParts(int sequenceNum)
    {
        if(sequenceNum < 0) return;
        if(sequenceNum >= childPartsList.Count) return;

        childPartsList[sequenceNum].DestroyMyself();
        childPartsList[sequenceNum] = null;
    }
}
