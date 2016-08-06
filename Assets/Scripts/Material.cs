using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 概ね当たり判定を持つ物体全般
/// </summary>
public class Material : Roots
{
    /// <summary>
    ///制御下のPartsリスト
    /// </summary>
    [SerializeField]
    private List<Parts> childPartsList = new List<Parts>();

    protected override void byTypeUpdate()
    {
        updatePosition();
        base.byTypeUpdate();
    }

    protected override void startup()
    {
        base.startup();
        gameObject.AddComponent<PolygonCollider2D>().isTrigger = true;
    }

    public int setParts(Parts setedParts)
    {
        if (setedParts == null) return -1;

        childPartsList.Add(setedParts);
        setedParts.setParent(gameObject.GetComponent<Material>());

        return childPartsList.Count - 1;
    }
    public Parts getParts(int sequenceNum)
    {
        if (sequenceNum < 0) return null;
        if (sequenceNum >= childPartsList.Count) return null;

        return childPartsList[sequenceNum];
    }
    public int getPartsNum()
    {
        return childPartsList.Count;
    }

    /// <summary>
    ///オブジェクトが可動範囲内にいるかどうか
    /// </summary>
    protected bool inScreen()
    {
        // 画面左下のワールド座標をビューポートから取得
        var lowerLeft = Camera.main.ViewportToWorldPoint(new Vector2(-1, -1));
        // 画面右上のワールド座標をビューポートから取得
        var upperRight = Camera.main.ViewportToWorldPoint(new Vector2(2, 2));

        if (transform.position.x < lowerLeft.x) return false;
        if (transform.position.x > upperRight.x) return false;
        if (transform.position.y < lowerLeft.y) return false;
        if (transform.position.y > upperRight.y) return false;
        return true;
    }

    /// <summary>
    ///オブジェクトの移動関数
    /// </summary>
    public void setVerosity(Vector2 verosity, float speed, float? acceleration = null, bool inScreen = false)
    {
        Vector2 beforSpeed = nowSpeed;
        Vector2 degree = (verosity.normalized * speed) - nowSpeed;
        float variation = degree.magnitude != 0
            ? Mathf.Clamp((acceleration ?? degree.magnitude) / degree.magnitude, -1, 1)
            : 0;

        // 実移動量を計算
        var innerVerosity = nowSpeed + degree * variation;

        if (inScreen)
        {
            // オブジェクトの座標を取得
            var self = transform.position;

            // 画面左下のワールド座標をビューポートから取得
            var lowerLeft = Camera.main.ViewportToWorldPoint(Vector2.zero);

            // 画面右上のワールド座標をビューポートから取得
            var upperRight = Camera.main.ViewportToWorldPoint(Vector2.one);

            // オブジェクトの位置が画面内に収まるように制限をかける
            innerVerosity.x = Mathf.Clamp(
                innerVerosity.x,
                (lowerLeft.x - self.x) * getPixel(),
                (upperRight.x - self.x) * getPixel());
            innerVerosity.y = Mathf.Clamp(
                innerVerosity.y,
                (lowerLeft.y - self.y) * getPixel(),
                (upperRight.y - self.y) * getPixel());
        }

        //速度設定
        // GetComponent<Rigidbody2D>().velocity = innerVerosity;
        nowSpeed = innerVerosity;

        //移動時アクション呼び出し
        setVerosityAction(nowSpeed - beforSpeed);
    }
    protected virtual void setVerosityAction(Vector2 acceleration) { }
    [SerializeField]
    public Vector2 nowSpeed = Vector2.zero;
    void updatePosition()
    {
        transform.position += (Vector3)(nowSpeed / getPixel());
    }

    /// <summary>
    ///最寄りの非味方機体検索関数
    /// </summary>
    protected Ship getNearTarget()
    {
        Terms term = target
            => target.GetComponent<Ship>() != null
            && target.gameObject.layer != gameObject.layer;
        List<Roots> shipList = getNearObject(term);

        if (shipList.Count <= 0) return null;
        return shipList[0].GetComponent<Ship>();
    }

    /// <summary>
    ///PartsListの削除関数
    ///引数無しで全消去
    /// </summary>
    public void deleteParts(int? sequenceNum = null)
    {
        if (sequenceNum != null) deleteSimpleParts((int)sequenceNum);
        for (int partsNum = 0; partsNum < childPartsList.Count; partsNum++)
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
        if (sequenceNum < 0) return;
        if (sequenceNum >= childPartsList.Count) return;

        childPartsList[sequenceNum].selfDestroy();
        childPartsList[sequenceNum] = null;
    }
}
