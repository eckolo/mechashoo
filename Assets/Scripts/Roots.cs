using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

/// <summary>
///あらゆるオブジェクトの基底関数とか
/// </summary>
public class Roots : Methods
{
    /// <summary>
    ///汎用タイマー
    /// </summary>
    [SerializeField]
    protected Timer timer = new Timer();
    /// <summary>
    ///汎用タイマークラス
    /// </summary>
    protected class Timer
    {
        private Dictionary<string, int> timerList = new Dictionary<string, int>();
        public string start(string key)
        {
            string finalKey = key;
            for (var i = 0; timerList.ContainsKey(finalKey); i++)
            {
                finalKey = key + i;
            }
            timerList.Add(finalKey, 0);
            return finalKey;
        }
        public int get(string key)
        {
            return timerList.ContainsKey(key) ? timerList[key] : 0;
        }
        public int stop(string key)
        {
            var finalValue = get(key);
            timerList.Remove(key);
            return finalValue;
        }
        public int reset(string key)
        {
            var finalValue = stop(key);
            start(key);
            return finalValue;
        }
        public void clock()
        {
            foreach (var timerName in new List<string>(timerList.Keys))
            {
                timerList[timerName]++;
            }
        }
    }
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
        updatePosition();
        timer.clock();
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
    public void setVerosity(Vector2 verosity, float speed, float? acceleration = null, bool inScreen = false)
    {
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
            var lowerLeft = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));

            // 画面右上のワールド座標をビューポートから取得
            var upperRight = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

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
        setVerosityAction(nowSpeed, speed);
    }
    protected virtual void setVerosityAction(Vector2 verosity, float speed) { }
    [SerializeField]
    Vector2 nowSpeed = new Vector2(0, 0);
    void updatePosition()
    {
        transform.position += (Vector3)(nowSpeed / getPixel());
    }
    /// <summary>
    /// １マス当たりのピクセル量を得る関数
    /// </summary>
    protected float getPixel()
    {
        if (GetComponent<SpriteRenderer>() == null) return 1;
        if (GetComponent<SpriteRenderer>().sprite == null) return 1;
        return GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
    }

    /// <summary>
    /// 弾の作成
    /// 座標・角度直接指定タイプ
    /// </summary>
    protected Bullet injection(Vector2 injectionPosition, float injectionAngle = 0, Bullet injectionBullet = null, float fuelCorrection = 1)
    {
        if (injectionBullet == null) return null;

        var injectionHoleLocal = new Vector2(
          (transform.rotation * injectionPosition).x * getLossyScale(transform).x,
          (transform.rotation * injectionPosition).y * getLossyScale(transform).y * (heightPositive ? 1 : -1)
         );
        var injectionAngleLocal = getLossyRotation()
            * Quaternion.AngleAxis(injectionAngle, Vector3.forward * getLossyScale(transform).y);
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
