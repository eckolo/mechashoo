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
    ///制御下のPartsリスト
    /// </summary>
    private List<Parts> childPartsList = new List<Parts>();

    /// <summary>
    ///画面内に位置強制するフラグ
    /// </summary>
    protected virtual bool forcedInScreen
    {
        get {
            return false;
        }
    }

    /// <summary>
    ///物体重量
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

    protected override void updateMotion()
    {
        updatePosition();
        base.updateMotion();
    }

    public override void Start()
    {
        base.Start();
        attachPolygonCollider();
        if(nowLayer == Layers.DEFAULT) nowLayer = parentMethod != null ? parentMethod.nowLayer : Layers.NEUTRAL;
        foreach(Transform child in transform)
        {
            var childParts = child.GetComponent<Parts>();
            if(childParts != null) setParts(childParts);
        }
        foreach(var childParts in childPartsList) childParts.nowRoot = this;
    }

    public int setParts(Parts setedParts)
    {
        if(setedParts == null) return -1;

        if(!childPartsList.Contains(setedParts)) childPartsList.Add(setedParts);
        setedParts.nowRoot = this;

        return childPartsList.IndexOf(setedParts);
    }
    public Parts getParts(int index)
    {
        if(index < 0) return null;
        if(index >= childPartsList.Count) return null;

        return childPartsList[index];
    }
    public Component getParts<Component>(int index) where Component : Parts
    {
        var parts = getParts(index);
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
    /// オブジェクトのリストから特定コンポーネントのリストへの変換
    /// </summary>
    public static List<Output> toComponents<Output>(List<Parts> originList)
        where Output : Methods
        => toComponents<Output, Parts>(originList);

    /// <summary>
    /// PolygonCollider2Dコンポーネントをアタッチするだけの関数
    /// </summary>
    protected PolygonCollider2D attachPolygonCollider()
    {
        if(GetComponent<PolygonCollider2D>() != null) Destroy(GetComponent<PolygonCollider2D>());
        var collider = gameObject.AddComponent<PolygonCollider2D>();

        collider.isTrigger = true;

        return collider;
    }

    /// <summary>
    ///オブジェクトが可動範囲内にいるかどうか
    /// </summary>
    protected bool inField
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
    public void stopMoving(float? acceleration = null)
    {
        if(acceleration == null) nowSpeed = Vector2.zero;
        else setVerosity(nowSpeed, 0, acceleration);
    }
    /// <summary>
    ///自然停止ラッパー関数
    /// </summary>
    public Vector2 stopping(float power) => exertPower(nowSpeed, power, 0);
    /// <summary>
    ///オブジェクトへ力を掛ける関数
    /// </summary>
    public Vector2 exertPower(Vector2 direction, float power, float? targetSpeed = null)
    {
        float acceleration = power / weight;

        if(targetSpeed == null) return setVerosity(nowSpeed + direction * acceleration);
        return setVerosity(direction, targetSpeed ?? 0, acceleration);
    }
    /// <summary>
    ///オブジェクトへ力を掛け続けた場合の最終速度予測値取得
    /// </summary>
    public Vector2 getExertPowerResult(Vector2 direction, float power, int time)
    {
        var result = nowSpeed;
        for(int index = 0; index < time; index++)
        {
            var targetSpeed = result + direction * power / weight;
            result = getVerosity(targetSpeed, targetSpeed.magnitude, null, result);
        }
        return result;
    }
    /// <summary>
    ///オブジェクトの移動関数
    /// </summary>
    public Vector2 setVerosity(Vector2 speed) => setVerosity(speed, speed.magnitude);
    /// <summary>
    ///オブジェクトの移動関数
    /// </summary>
    public Vector2 setVerosity(Vector2 verosity, float speed, float? acceleration = null)
    {
        Vector2 originSpeed = nowSpeed;

        //速度設定
        nowSpeed = getVerosity(verosity, speed, acceleration);

        //移動時アクション呼び出し
        setVerosityAction(nowSpeed - originSpeed);
        return nowSpeed;
    }
    /// <summary>
    ///オブジェクトの移動量取得関数
    /// </summary>
    private Vector2 getVerosity(Vector2 verosity, float speed, float? acceleration, Vector2? originSpeed = null)
    {
        Vector2 baseSpeed = originSpeed ?? nowSpeed;
        Vector2 degree = MathV.recalculation(verosity, speed) - baseSpeed;
        var length = degree.magnitude;
        float variation = length != 0
            ? Mathf.Clamp(Mathf.Min(acceleration ?? length, length) / length, -1, 1)
            : 0;

        // 実移動量を計算
        var resultVerosity = baseSpeed + degree * variation;

        if(forcedInScreen) resultVerosity = MathV.within(resultVerosity, fieldLowerLeft - position, fieldUpperRight - position);

        return resultVerosity;
    }
    protected virtual void setVerosityAction(Vector2 acceleration) { }
    public Vector2 nowSpeed { private set; get; }
    void updatePosition()
    {
        var result = position + MathV.rescaling(nowSpeed, baseMas);
        if(forcedInScreen) result = MathV.within(result, fieldLowerLeft, fieldUpperRight);
        position = result;
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
            Terms term = target
                => target.GetComponent<Ship>() != null
                && target.nowLayer != nowLayer;
            return getNearObject(term).FirstOrDefault()?.GetComponent<Ship>();
        }
    }

    /// <summary>
    ///PartsListの削除関数
    ///引数無しで全消去
    /// </summary>
    public void deleteParts(int? sequenceNum = null)
    {
        if(sequenceNum != null) deleteSimpleParts((int)sequenceNum);
        for(int partsNum = 0; partsNum < childPartsList.Count; partsNum++)
        {
            deleteSimpleParts(partsNum);
        }
        childPartsList = new List<Parts>();
    }
    /// <summary>
    ///PartsListから指定した番号のPartsを削除する
    /// </summary>
    private void deleteSimpleParts(int sequenceNum)
    {
        if(sequenceNum < 0) return;
        if(sequenceNum >= childPartsList.Count) return;

        childPartsList[sequenceNum].selfDestroy();
        childPartsList[sequenceNum] = null;
    }
}
