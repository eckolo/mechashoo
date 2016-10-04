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
    ///表示名称
    /// </summary>
    [SerializeField]
    private string _displayName = "";
    public string displayName
    {
        get
        {
            if (_displayName != null && _displayName != "") return _displayName;
            if (gameObject != null) return gameObject.name.Replace("(Clone)", "");
            return _displayName;
        }
    }
    /// <summary>
    ///汎用タイマー
    /// </summary>
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
    ///横方向の反転を加味した向きベクトル
    /// </summary>
    protected Vector2 nowForward
    {
        get
        {
            return transform.right * nWidthPositive;
        }
    }
    /// <summary>
    ///横方向の非反転フラグ
    /// </summary>
    public bool widthPositive
    {
        get
        {
            return getLossyScale(transform).x > 0;
        }
    }
    public float nWidthPositive
    {
        get
        {
            return getLossyScale(transform).x == 0
                ? 0
                : getLossyScale(transform).x / Mathf.Abs(getLossyScale(transform).x);
        }
    }
    protected bool invertWidth(bool? setPositice = null)
    {
        bool nextPositive = setPositice ?? !widthPositive;
        var scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (nextPositive ? 1 : -1);
        transform.localScale = scale;
        return widthPositive;
    }
    protected bool invertWidth(float setPositice)
    {
        if (setPositice == 0) return widthPositive;
        return invertWidth(setPositice > 0);
    }
    /// <summary>
    ///左右反転を加味した角度補正
    /// </summary>
    protected float getWidthRealAngle(float angle, bool? degree = null)
    {
        return angle + ((degree ?? widthPositive) ? 0 : 180);
    }
    /// <summary>
    ///縦方向の非反転フラグ
    /// </summary>
    [SerializeField]
    public bool heightPositive = true;

    // Update is called once per frame
    public virtual void Start() { }

    // Update is called once per frame
    public virtual void Update()
    {
        timer.clock();
        var keepPosition = transform.localPosition;
        if (keepPosition.z != 0) transform.localPosition = new Vector3(keepPosition.x, keepPosition.y, 0);
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

    public float setAngle(Vector2 targetVector)
    {
        return setAngle(getWidthRealAngle(MathA.toAngle(targetVector)));
    }
    public virtual float setAngle(float settedAngle)
    {
        var finalAngle = MathA.compile(settedAngle);
        transform.localEulerAngles = new Vector3(0, 0, finalAngle);

        return finalAngle;
    }
    public float nowLossyAngle
    {
        get
        {
            return transform.rotation.eulerAngles.z;
        }
    }
    public float nowLocalAngle
    {
        get
        {
            return transform.localRotation.eulerAngles.z;
        }
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

    protected Vector2 correctWidthVector(Vector2 inputVector)
    {
        return new Vector2(inputVector.x * nWidthPositive, inputVector.y);
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
        instantiatedBullet.transform.parent = sysPanel.transform;
        instantiatedBullet.gameObject.layer = gameObject.layer;
        instantiatedBullet.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
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
        effectObject.transform.parent = sysPanel.transform;
        effectObject.transform.localPosition = setPosition;
        effectObject.baseScale = baseSize ?? 1;

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
    protected float parPixel
    {
        get
        {
            if (GetComponent<SpriteRenderer>() == null) return 1;
            if (GetComponent<SpriteRenderer>().sprite == null) return 1;
            return GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        }
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
    public void setAlpha(float alpha)
    {
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite == null) return;

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
        return;
    }
    /// <summary>
    ///透明度取得関数
    /// </summary>
    protected float nowAlpha
    {
        get
        {
            var sprite = GetComponent<SpriteRenderer>();
            if (sprite == null) return 0;

            return sprite.color.a;
        }
    }

    /// <summary>
    ///存在判定関数
    /// </summary>
    public bool isExist
    {
        get
        {
            if (gameObject == null) return false;
            return gameObject.activeSelf;
        }
    }
}
