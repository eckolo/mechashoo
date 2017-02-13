using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

/// <summary>
///あらゆるオブジェクトの基底関数とか
/// </summary>
public class Materials : Methods
{
    // Update is called once per frame
    public override void Start()
    {
        base.Start();

        StartCoroutine(startMotion());
        if(nowSort == Configs.SortLayers.DEFAULT) nowSort = Configs.SortLayers.PHYSICAL;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
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
            if(!timerList.ContainsKey(key)) timerList.Add(key, 0);
            timerList[key] = 0;
            return key;
        }
        public int get(string key)
        {
            return timerList.ContainsKey(key) ? timerList[key] : 0;
        }
        public int stop(string key)
        {
            if(!timerList.ContainsKey(key)) return 0;
            var result = get(key);
            timerList.Remove(key);
            return result;
        }
        public int reset(string key)
        {
            var result = stop(key);
            start(key);
            return result;
        }
        public void clock()
        {
            foreach(var timerName in new List<string>(timerList.Keys))
            {
                timerList[timerName]++;
            }
        }
    }
    /// <summary>
    ///横方向の反転を加味した向きベクトル
    /// </summary>
    public Vector2 nowForward
    {
        get {
            return transform.right * nWidthPositive;
        }
        protected set {
            setAngle(value);
        }
    }
    /// <summary>
    ///横方向の非反転フラグ
    /// </summary>
    public bool widthPositive
    {
        get {
            return lossyScale.x > 0;
        }
    }
    public float nWidthPositive
    {
        get {
            return lossyScale.x.toSign();
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
        if(setPositice == 0) return widthPositive;
        return invertWidth(setPositice > 0);
    }
    protected bool invertWidth(Vector2 setVector) => invertWidth(setVector.x);
    /// <summary>
    ///左右反転を加味した角度補正
    /// </summary>
    protected float getWidthRealAngle(float angle)
    {
        if(!widthPositive) return angle.invert();
        return angle;
    }
    /// <summary>
    ///左右反転を加味した角度補正
    /// </summary>
    protected Quaternion getWidthRealRotation(Quaternion rotation)
    {
        if(!widthPositive) return rotation.invert();
        return rotation;
    }
    /// <summary>
    ///縦方向の非反転フラグ
    /// </summary>
    [SerializeField]
    public bool heightPositive = true;

    protected IEnumerator startMotion()
    {
        while(true)
        {
            updateMotion();
            yield return wait(1);
        }
    }
    protected virtual void updateMotion()
    {
        timer.clock();
    }

    public virtual bool action(int? actionNum = null)
    {
        StartCoroutine(baseMotion(actionNum ?? 0));
        return true;
    }
    protected virtual IEnumerator baseMotion(int actionNum)
    {
        yield return motion(actionNum);
        yield break;
    }

    protected virtual IEnumerator motion(int actionNum)
    {
        yield break;
    }

    public float setAngle(Vector2 targetVector)
    {
        return setAngle(targetVector.toAngle() + (widthPositive ? 0 : 180));
    }
    public float setAngle(Quaternion targetRotation)
    {
        return setAngle(targetRotation.toAngle());
    }
    public virtual float setAngle(float settedAngle)
    {
        var finalAngle = settedAngle.compile();
        transform.localEulerAngles = new Vector3(0, 0, finalAngle);

        return finalAngle;
    }
    public float nowLossyAngle
    {
        get {
            return transform.rotation.eulerAngles.z;
        }
    }
    public float nowLocalAngle
    {
        get {
            return transform.localRotation.eulerAngles.z;
        }
    }
    public Vector2 invertVector(Vector2 inputVector)
    {
        return new Vector2(inputVector.x * -1, inputVector.y);
    }
    public Vector2 lossyScale
    {
        get {
            if(nowParent == null) return transform.localScale;
            return ((Vector2)transform.localScale).scaling(nowParent.lossyScale);
        }
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
    ///表示順の設定
    /// </summary>
    public virtual string nowSort
    {
        get {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if(spriteRenderer == null) return Configs.SortLayers.DEFAULT;
            return spriteRenderer.sortingLayerName;
        }
        set {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if(spriteRenderer != null) spriteRenderer.sortingLayerName = value;
            foreach(Transform child in transform)
            {
                var materials = child.GetComponent<Materials>();
                if(materials != null)
                {
                    materials.nowSort = value;
                    materials.nowOrder = nowOrder;
                }
            }
        }
    }
    public int nowOrder
    {
        get {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if(spriteRenderer == null) return 0;
            return spriteRenderer.sortingOrder;
        }
        set {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if(spriteRenderer != null) spriteRenderer.sortingOrder = value;
            foreach(Transform child in transform)
            {
                var materials = child.GetComponent<Materials>();
                if(materials != null) materials.nowOrder = value;
            }
        }
    }

    protected Vector2 correctWidthVector(Vector2 inputVector)
    {
        return new Vector2(inputVector.x * nWidthPositive, inputVector.y);
    }

    /// <summary>
    /// 物体生成
    /// </summary>
    protected Injected inject<Injected>(Injected injectObject, Vector2 injectPosition, float injectAngle = 0) where Injected : Things
    {
        if(injectObject == null) return null;

        var localLossyRotation = (lossyScale.x.toSign() * getLossyRotation().toAngle()).toRotation();
        Vector2 injectHoleLocal = localLossyRotation * injectPosition.scaling(lossyScale);
        var injectAngleLocal = getLossyRotation() * (lossyScale.y.toSign() * injectAngle).toRotation();
        if(lossyScale.x < 0) injectAngleLocal = injectAngleLocal.invert();

        var injected = Instantiate(injectObject);
        injected.nowParent = sysPanel.transform;
        injected.position = globalPosition + injectHoleLocal;
        injected.setAngle(injectAngleLocal);

        injected.nowSort = nowSort;
        injected.nowLayer = nowLayer;
        injected.transform.localScale = new Vector2(
            Mathf.Abs(lossyScale.x),
            Mathf.Abs(lossyScale.y));

        return injected;
    }

    /// <summary>
    /// エフェクト発生関数
    /// </summary>
    public Effect outbreakEffect(Effect effect, float? baseSize = null, Vector2? position = null)
    {
        Vector2 setPosition = globalPosition + (position ?? Vector2.zero);

        var effectObject = Instantiate(effect, setPosition, transform.rotation);
        effectObject.nowParent = sysPanel.transform;
        effectObject.position = setPosition;
        effectObject.transform.localScale = ((Vector2)effectObject.transform.localScale).scaling(lossyScale);
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
        get {
            if(GetComponent<SpriteRenderer>() == null)
                return 1;
            if(GetComponent<SpriteRenderer>().sprite == null)
                return 1;
            return GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        }
    }
    /// <summary>
    /// ベースの画像サイズ取得関数
    /// </summary>
    protected Vector2 baseSize
    {
        get {
            var spriteData = GetComponent<SpriteRenderer>();
            if(spriteData == null)
                return Vector2.zero;
            if(spriteData.sprite == null)
                return Vector2.zero;
            return spriteData.sprite.bounds.size;
        }
    }
    /// <summary>
    ///透明度変更関数
    /// </summary>
    public void setAlpha(float alpha)
    {
        var sprite = GetComponent<SpriteRenderer>();
        if(sprite == null)
            return;

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
        return;
    }
    /// <summary>
    ///透明度取得関数
    /// </summary>
    protected float nowAlpha
    {
        get {
            var sprite = GetComponent<SpriteRenderer>();
            if(sprite == null)
                return 0;

            return sprite.color.a;
        }
    }

    /// <summary>
    ///存在判定関数
    /// </summary>
    public bool isExist
    {
        get {
            if(gameObject == null)
                return false;
            return gameObject.activeSelf;
        }
    }
}
