using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// あらゆるオブジェクトの基底関数とか
/// </summary>
public class Materials : Methods
{
    // Update is called once per frame
    public override void Start()
    {
        base.Start();

        AttachRigidbody();

        StartCoroutine(StartMotion());
        if(nowSort == Configs.SortLayers.DEFAULT) nowSort = Configs.SortLayers.PHYSICAL;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Rigidbody2Dコンポーネントをアタッチするだけの関数
    /// </summary>
    protected Rigidbody2D AttachRigidbody()
    {
        var rigidbody = GetComponent<Rigidbody2D>();
        if(rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody2D>();

        rigidbody.gravityScale = 0;

        return rigidbody;
    }

    /// <summary>
    /// 汎用タイマー
    /// </summary>
    protected Timer timer = new Timer();
    /// <summary>
    /// 汎用タイマークラス
    /// </summary>
    protected class Timer
    {
        private Dictionary<string, int> timerList = new Dictionary<string, int>();
        public string Start(string key)
        {
            if(!timerList.ContainsKey(key)) timerList.Add(key, 0);
            timerList[key] = 0;
            return key;
        }
        public int Get(string key)
        {
            return timerList.ContainsKey(key) ? timerList[key] : 0;
        }
        public int Stop(string key)
        {
            if(!timerList.ContainsKey(key)) return 0;
            var result = Get(key);
            timerList.Remove(key);
            return result;
        }
        public int Reset(string key)
        {
            var result = Stop(key);
            Start(key);
            return result;
        }
        public void Clock()
        {
            foreach(var timerName in new List<string>(timerList.Keys))
            {
                timerList[timerName]++;
            }
        }
    }
    /// <summary>
    /// 横方向の反転を加味した向きベクトル
    /// </summary>
    public Vector2 nowForward
    {
        get {
            return transform.right * nWidthPositive;
        }
        protected set {
            SetAngle(value);
        }
    }
    /// <summary>
    /// 物体の左右反転状態操作
    /// </summary>
    /// <param name="setPositice">右向かせるか否か</param>
    /// <returns>右向いてるか否か</returns>
    public virtual bool InvertWidth(bool? setPositice = null)
    {
        bool nextPositive = setPositice ?? !widthPositive;
        var scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (nextPositive ? 1 : -1);
        transform.localScale = scale;
        return widthPositive;
    }
    /// <summary>
    /// 物体の左右反転状態操作
    /// </summary>
    /// <param name="setPositice">基準となるX座標</param>
    /// <returns>右向いてるか否か</returns>
    public bool InvertWidth(float setPositice)
    {
        if(setPositice == 0) return widthPositive;
        return InvertWidth(setPositice > 0);
    }
    /// <summary>
    /// 物体の左右反転状態操作
    /// </summary>
    /// <param name="setVector">向いてる方向ベクトル</param>
    /// <returns>右向いてるか否か</returns>
    public bool InvertWidth(Vector2 setVector) => InvertWidth(setVector.x);

    /// <summary>
    /// 左右反転を加味した角度補正
    /// </summary>
    protected float GetWidthRealAngle(float angle)
    {
        if(!widthPositive) return angle.Invert();
        return angle;
    }
    /// <summary>
    /// 左右反転を加味した角度補正
    /// </summary>
    protected Quaternion GetWidthRealRotation(Quaternion rotation)
    {
        if(!widthPositive) return rotation.Invert();
        return rotation;
    }
    /// <summary>
    /// 縦方向の非反転フラグ
    /// </summary>
    [SerializeField]
    public bool heightPositive = true;

    protected IEnumerator StartMotion()
    {
        while(true)
        {
            UpdateMotion();
            yield return Wait(1);
        }
    }
    protected virtual void UpdateMotion()
    {
        timer.Clock();
    }

    public virtual bool Action(int? actionNum = null)
    {
        StartCoroutine(BaseMotion(actionNum ?? 0));
        return true;
    }
    protected virtual IEnumerator BaseMotion(int actionNum)
    {
        yield return Motion(actionNum);
        yield break;
    }

    protected virtual IEnumerator Motion(int actionNum)
    {
        yield break;
    }

    public float SetAngle(Vector2 targetVector)
    {
        return SetAngle(targetVector.ToAngle() + (widthPositive ? 0 : 180));
    }
    public float SetAngle(Quaternion targetRotation)
    {
        return SetAngle(targetRotation.ToAngle());
    }
    public virtual float nowAngle
    {
        get {
            return transform.localEulerAngles.z.Compile();
        }
        set {
            var finalAngle = value.Compile();
            transform.localEulerAngles = new Vector3(0, 0, finalAngle);
        }
    }
    public virtual float SetAngle(float settedAngle)
    {
        nowAngle = settedAngle;
        return nowAngle;
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
    public Quaternion GetLossyRotation(Transform inputorigin = null)
    {
        var origin = (inputorigin ?? transform);
        var localQuat = new Vector3(origin.localRotation.x,
            origin.localRotation.y,
            origin.localRotation.z).magnitude != 0
            ? origin.localRotation
            : Quaternion.AngleAxis(0, Vector3.forward);
        localQuat.z *= origin.localScale.x;
        return origin.parent != null
            ? GetLossyRotation(origin.parent) * localQuat
            : localQuat;
    }

    /// <summary>
    /// 表示順の設定
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

    /// <summary>
    /// 物体生成
    /// </summary>
    /// <typeparam name="Injected">生成するオブジェクトの型</typeparam>
    /// <param name="injectObject">生成するオブジェクトの雛形</param>
    /// <param name="injectPosition">生成する座標</param>
    /// <param name="injectAngle">生成時の角度</param>
    /// <returns>生成されたオブジェクト</returns>
    protected virtual Injected Inject<Injected>(Injected injectObject, Vector2 injectPosition, float injectAngle = 0) where Injected : Things
    {
        if(injectObject == null) return null;

        var localLossyRotation = (lossyScale.x.ToSign() * GetLossyRotation().ToAngle()).ToRotation();
        Vector2 injectHoleLocal = localLossyRotation * injectPosition.Scaling(lossyScale);
        var injectAngleLocal = GetLossyRotation() * (lossyScale.y.ToSign() * injectAngle).ToRotation();
        if(lossyScale.x < 0) injectAngleLocal = injectAngleLocal.Invert();

        var injected = Instantiate(injectObject);
        injected.nowParent = sysPanel.transform;
        injected.position = globalPosition + injectHoleLocal;
        injected.SetAngle(injectAngleLocal);

        injected.nowSort = nowSort;
        injected.nowOrder = nowOrder;
        injected.nowLayer = nowLayer;
        injected.transform.localScale = new Vector2(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.y));

        return injected;
    }

    /// <summary>
    /// エフェクト発生関数
    /// </summary>
    public Effect OutbreakEffect(Effect effect, float? baseSize = null, Vector2? position = null)
    {
        Vector2 setPosition = globalPosition + (position ?? Vector2.zero);

        var effectObject = Instantiate(effect, setPosition, transform.rotation);
        effectObject.nowParent = sysPanel.transform;
        effectObject.position = setPosition;
        effectObject.nowSort = nowSort;
        effectObject.nowOrder = nowOrder;
        effectObject.nowLayer = nowLayer;
        effectObject.nowZ = nowZ - 1;
        effectObject.transform.localScale = ((Vector2)effectObject.transform.localScale).Scaling(lossyScale);
        effectObject.baseScale = baseSize ?? 1;

        return effectObject;
    }
    /// <summary>
    /// エフェクト発生関数
    /// </summary>
    public Effect OutbreakEffect(Effect effect, Vector2 position)
    {
        return OutbreakEffect(effect, null, position);
    }

    /// <summary>
    /// オブジェクトの透明度プロパティ
    /// </summary>
    public float nowAlpha
    {
        get {
            var sprite = GetComponent<SpriteRenderer>();
            if(sprite == null) return 0;

            return sprite.color.a;
        }
        set {
            var sprite = GetComponent<SpriteRenderer>();
            if(sprite == null) return;

            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, value);
        }
    }

    /// <summary>
    /// 存在判定関数
    /// </summary>
    public bool isExist
    {
        get {
            if(gameObject == null) return false;
            return gameObject.activeSelf;
        }
    }

    public override void DestroyMyself(bool system = false)
    {
        nowAlpha = 0;
        base.DestroyMyself(system);
    }
}
