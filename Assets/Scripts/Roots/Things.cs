using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    [SerializeField]
    private bool forcedScreen = false;

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

        childPartsList.Add(setedParts);
        setedParts.nowRoot = this;

        return childPartsList.Count - 1;
    }
    public Parts getParts(int sequenceNum)
    {
        if(sequenceNum < 0) return null;
        if(sequenceNum >= childPartsList.Count) return null;

        return childPartsList[sequenceNum];
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
    ///オブジェクトの移動関数
    /// </summary>
    public Vector2 setVerosity(Vector2 speed) => setVerosity(speed, speed.magnitude);
    /// <summary>
    ///オブジェクトの移動関数
    /// </summary>
    public Vector2 setVerosity(Vector2 verosity, float speed, float? acceleration = null)
    {
        Vector2 originSpeed = nowSpeed;
        Vector2 degree = MathV.recalculation(verosity, speed) - originSpeed;
        var length = degree.magnitude;
        float variation = length != 0
            ? Mathf.Clamp(Mathf.Min(acceleration ?? length, length) / length, -1, 1)
            : 0;

        // 実移動量を計算
        var innerVerosity = originSpeed + degree * variation;

        if(forcedScreen)
        {
            innerVerosity.x = Mathf.Clamp(
                innerVerosity.x,
                (fieldLowerLeft.x - globalPosition.x) * baseMas.x,
                (fieldUpperRight.x - globalPosition.x) * baseMas.x);
            innerVerosity.y = Mathf.Clamp(
                innerVerosity.y,
                (fieldLowerLeft.y - globalPosition.y) * baseMas.y,
                (fieldUpperRight.y - globalPosition.y) * baseMas.y);
        }

        //速度設定
        nowSpeed = innerVerosity;

        //移動時アクション呼び出し
        setVerosityAction(nowSpeed - originSpeed);
        return nowSpeed;
    }
    protected virtual void setVerosityAction(Vector2 acceleration) { }
    public Vector2 nowSpeed { private set; get; }
    void updatePosition()
    {
        position += MathV.rescaling(nowSpeed, baseMas);
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
                && target.gameObject.layer != gameObject.layer;
            List<Materials> shipList = getNearObject(term);

            if(shipList.Count <= 0) return null;
            return shipList[0].GetComponent<Ship>();
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
