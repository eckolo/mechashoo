using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

/// <summary>
///あらゆるオブジェクトの基底関数とか
/// </summary>
public class Material : Methods
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
            if (!timerList.ContainsKey(key)) timerList.Add(key, 0);
            timerList[key] = 0;
            return key;
        }
        public int get(string key)
        {
            return timerList.ContainsKey(key) ? timerList[key] : 0;
        }
        public int stop(string key)
        {
            if (!timerList.ContainsKey(key)) return 0;
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
        startup();
    }
    protected virtual void baseStart() { }

    protected virtual void startup() { }

    // Update is called once per frame
    public virtual void Update()
    {
        baseUpdate();
        byTypeUpdate();
        timer.clock();
    }
    protected virtual void baseUpdate() { }
    protected virtual void byTypeUpdate() { }

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
        var next = origin.parent != null ? getLossyScale(origin.parent) : Vector2.one;
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
    /// 弾の作成
    /// 座標・角度直接指定タイプ
    /// </summary>
    protected Bullet injection(Bullet injectionBullet, Vector2 injectionPosition, float injectionAngle = 0)
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
        instantiatedBullet.transform.parent = getPanel().transform;
        instantiatedBullet.gameObject.layer = gameObject.layer;
        instantiatedBullet.transform.localScale = new Vector2(
            Mathf.Abs(getLossyScale().x),
            Mathf.Abs(getLossyScale().y));

        return instantiatedBullet;
    }

    /// <summary>
    /// エフェクト発生関数
    /// </summary>
    public Effect outbreakEffect(Effect effect, float? baseSize = null, Vector2? position = null)
    {
        Vector2 setPosition = (Vector2)transform.position + (position ?? Vector2.zero);

        Quaternion setRotation = getLossyScale().x >= 0
            ? transform.rotation
            : getReverse(transform.rotation);
        Effect effectObject = (Effect)Instantiate(effect, setPosition, setRotation);
        effectObject.transform.parent = getPanel().transform;
        effectObject.transform.localScale = Vector3.one * (baseSize ?? 1);

        return effectObject;
    }
    /// <summary>
    /// エフェクト発生関数
    /// </summary>
    public Effect outbreakEffect(Effect effect, Vector2 position)
    {
        return outbreakEffect(effect, null, position);
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
    /// ベースの画像サイズ取得関数
    /// </summary>
    protected Vector2 baseSize
    {
        get
        {
            var spriteData = GetComponent<SpriteRenderer>();
            if (spriteData == null) return Vector2.zero;
            if (spriteData.sprite == null) return Vector2.zero;
            return spriteData.sprite.bounds.size;
        }
    }
    /// <summary>
    ///透明度変更関数
    /// </summary>
    protected void setAlpha(float alpha)
    {
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite == null) return;

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
        return;
    }
    /// <summary>
    ///透明度取得関数
    /// </summary>
    protected float getAlpha()
    {
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite == null) return 0;

        return sprite.color.a;
    }
}
