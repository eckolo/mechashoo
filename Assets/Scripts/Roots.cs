using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

/// <summary>
///あらゆるオブジェクトの基底関数とか
/// </summary>
public class Roots : Mthods
{
    /// <summary>
    ///横方向の非反転フラグ
    /// </summary>
    public bool widthPositive = true;
    /// <summary>
    ///縦方向の非反転フラグ
    /// </summary>
    [SerializeField]
    public bool heightPositive = true;

    // Update is called once per frame
    public virtual void Start()
    {
        baseStart();
    }
    protected virtual void baseStart() { }

    // Update is called once per frame
    public virtual void Update()
    {
        baseUpdate();
        foreach (var counterName in new List<string>(counterList.Keys))
        {
            counterList[counterName]++;
        }
    }
    protected virtual void baseUpdate() { }
    protected List<Roots> getNearObject(Terms map = null)
    {
        return searchMaxObject(target => -(target.transform.position - transform.position).magnitude, map);
    }

    public virtual bool Action(int? actionNum = null)
    {
        StartCoroutine(baseMotion(actionNum ?? 0));
        return true;
    }
    protected virtual IEnumerator baseMotion(int actionNum)
    {
        yield return Motion(actionNum);
        yield break;
    }

    protected virtual IEnumerator Motion(int actionNum)
    {
        yield break;
    }

    protected static float compileMinusAngle(float angle)
    {
        while (angle < 0) angle += 360;
        while (angle >= 360) angle -= 360;
        return angle;
    }
    protected static float toAngle(Vector2 targetVector)
    {
        return Vector2.Angle(Vector2.right, targetVector) * (Vector2.Angle(Vector2.up, targetVector) <= 90 ? 1 : -1);
    }
    protected void setAngle(Vector2 targetVector, bool width = true)
    {
        transform.rotation = Quaternion.FromToRotation(width ? Vector2.right : Vector2.left, targetVector);
        return;
    }
    public float setAngle(float settedAngle, bool width = true)
    {
        if (!width) settedAngle = 180 - compileMinusAngle(settedAngle);
        var finalAngle = compileMinusAngle(settedAngle);
        transform.localEulerAngles = new Vector3(0, 0, finalAngle);

        return finalAngle;
    }
    public Vector2 invertVector(Vector2 inputVector)
    {
        return new Vector2(inputVector.x * -1, inputVector.y);
    }
    public Vector2 getLossyScale(Transform origin = null)
    {
        if (origin == null) return getLossyScale(transform);
        var next = origin.parent != null ? getLossyScale(origin.parent) : new Vector2(1, 1);
        return new Vector2(origin.localScale.x * next.x, origin.localScale.y * next.y);
    }
    public Quaternion getLossyRotation(Transform inputorigin = null)
    {
        var origin = (inputorigin ?? transform);
        var localQuat = new Vector3(origin.localRotation.x,
            origin.localRotation.y,
            origin.localRotation.z).magnitude != 0
            ? origin.localRotation
            : Quaternion.AngleAxis(0, Vector3.forward);
        localQuat.z *= origin.localScale.x;
        return origin.parent != null
            ? getLossyRotation(origin.parent) * localQuat
            : localQuat;
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
    public void setVerosity(Vector2 verosity, float speed = 0, bool inScreen = false)
    {
        // 実移動量を計算
        var innerVerosity = verosity.normalized * speed;

        if (inScreen)
        {
            // オブジェクトの座標を取得
            var self = transform.position;

            // 画面左下のワールド座標をビューポートから取得
            var lowerLeft = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));

            // 画面右上のワールド座標をビューポートから取得
            var upperRight = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

            // オブジェクトの位置が画面内に収まるように制限をかける
            innerVerosity.x = Mathf.Clamp(
                innerVerosity.x,
                (lowerLeft.x - self.x) * 100,
                (upperRight.x - self.x) * 100);
            innerVerosity.y = Mathf.Clamp(
                innerVerosity.y,
                (lowerLeft.y - self.y) * 100,
                (upperRight.y - self.y) * 100);
        }

        //速度設定
        GetComponent<Rigidbody2D>().velocity = innerVerosity;

        //移動時アクション呼び出し
        setVerosityAction(GetComponent<Rigidbody2D>().velocity, speed);
    }
    protected virtual void setVerosityAction(Vector2 verosity, float speed) { }

    /// <summary>
    /// 弾の作成
    /// 座標・角度直接指定タイプ
    /// </summary>
    protected Bullet injection(Vector2 injectionPosition, float injectionAngle = 0, Bullet injectionBullet = null)
    {
        if (injectionBullet == null) return null;

        //injectionAngle = getLossyScale(transform).x >= 0 ? injectionAngle : 180 - injectionAngle;

        var injectionHoleLocal = new Vector2(
          (transform.rotation * injectionPosition).x * getLossyScale(transform).x,
          (transform.rotation * injectionPosition).y * getLossyScale(transform).y * (heightPositive ? 1 : -1)
         );
        var injectionAngleLocal = getLossyRotation()
            * Quaternion.AngleAxis(injectionAngle, Vector3.forward * getLossyScale(transform).y);
        Debug.Log(Quaternion.AngleAxis(injectionAngle, Vector3.forward * getLossyScale(transform).y));
        if (getLossyScale(transform).x < 0) injectionAngleLocal.eulerAngles = new Vector3(0, 0, 180 - injectionAngleLocal.eulerAngles.z);
        var instantiatedBullet = (Bullet)Instantiate(injectionBullet,
            (Vector2)transform.position + injectionHoleLocal,
            injectionAngleLocal);
        instantiatedBullet.gameObject.layer = gameObject.layer;
        instantiatedBullet.transform.localScale = new Vector2(
            Mathf.Abs(getLossyScale().x),
            Mathf.Abs(getLossyScale().y));
        // ショット音を鳴らす
        //GetComponent<AudioSource>().Play();

        return instantiatedBullet;
    }
}
