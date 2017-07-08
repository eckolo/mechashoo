using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System;

public abstract partial class Methods : MonoBehaviour
{
    public delegate bool Terms<Type>(Type target) where Type : Materials;
    public delegate float Rank<Type>(Type target) where Type : Materials;
    public delegate IEnumerator PublicAction<Type>(Type value);

    /// <summary>
    /// 表示名称
    /// </summary>
    [SerializeField]
    private string _displayName = "";
    /// <summary>
    /// 表示名称
    /// </summary>
    public string displayName
    {
        get {
            if(_displayName.Any()) return _displayName;
            if(gameObject != null) return gameObject.name.Replace("(Clone)", "");
            return _displayName;
        }
        protected set {
            _displayName = value;
        }
    }
    /// <summary>
    /// 表示略称
    /// </summary>
    [SerializeField]
    private string _abbreviation = "";
    /// <summary>
    /// 表示略称
    /// </summary>
    public string abbreviation
    {
        get {
            if(_abbreviation.Any()) return _abbreviation;
            return displayName;
        }
    }
    /// <summary>
    /// 内容説明文
    /// </summary>
    [Multiline(8), SerializeField]
    public string explanation = "内容説明";


    // Update is called once per frame
    public virtual void Start()
    {
        if(nowLayer == Configs.Layers.DEFAULT && parentMethod != null) nowLayer = parentMethod.nowLayer;
    }
    // Update is called once per frame
    public virtual void Update()
    {
        if(nextDestroy) ExecuteDestroy();
    }

    public Vector2 position
    {
        get {
            return new Vector2(transform.localPosition.x, transform.localPosition.y);
        }
        set {
            transform.localPosition = new Vector3(value.x, value.y, nowZ);
        }
    }
    public Vector2 globalPosition
    {
        get {
            return new Vector2(transform.position.x, transform.position.y);
        }
        set {
            transform.position = new Vector3(value.x, value.y, globalNowZ);
        }
    }
    /// <summary>
    /// 奥行き位置の設定
    /// </summary>
    public virtual float nowZ
    {
        get {
            return transform.localPosition.z;
        }
        set {
            var keepPosition = transform.localPosition;
            transform.localPosition = new Vector3(keepPosition.x, keepPosition.y, value);
        }
    }
    /// <summary>
    /// 奥行き位置の設定
    /// </summary>
    public virtual float globalNowZ
    {
        get {
            if(parentMethod != null) return parentMethod.globalNowZ + nowZ;
            return nowZ;
        }
    }

    /// <summary>
    /// オブジェクトの拡大率ラッパー
    /// </summary>
    public Vector2 nowScale
    {
        get {
            return transform.localScale;
        }
        set {
            var origin = transform.localScale;
            transform.localScale = new Vector3(value.x, value.y, origin.z);
        }
    }
    public Vector2 lossyScale
    {
        get {
            if(nowParent == null) return transform.localScale;
            return ((Vector2)transform.localScale).Scaling(nowParent.lossyScale);
        }
        set {
            var origin = transform.localScale;
            var setScale = nowParent != null ? value.Rescaling(nowParent.lossyScale) : value;
            transform.localScale = new Vector3(setScale.x, setScale.y, origin.z);
        }
    }

    /// <summary>
    /// レイヤーの設定
    /// </summary>
    public virtual string nowLayer
    {
        get {
            return LayerMask.LayerToName(gameObject.layer);
        }
        set {
            gameObject.layer = LayerMask.NameToLayer(value);
            foreach(Transform child in transform)
            {
                var method = child.GetComponent<Methods>();
                if(method != null) method.nowLayer = value;
            }
        }
    }

    /// <summary>
    /// 横方向の非反転フラグ
    /// </summary>
    public bool widthPositive => lossyScale.x > 0;
    /// <summary>
    /// 横方向の非反転フラグ（数値版）
    /// </summary>
    public float nWidthPositive => lossyScale.x.ToSign();
    /// <summary>
    /// 自身の左右反転状態を加味してベクトル補完
    /// </summary>
    /// <param name="inputVector">元のベクトル</param>
    /// <returns>補正後のベクトル</returns>
    protected Vector2 CorrectWidthVector(Vector2 inputVector)
        => new Vector2(inputVector.x * nWidthPositive, inputVector.y);

    /// <summary>
    /// 暗調設置
    /// </summary>
    /// <param name="alpha">透明度</param>
    /// <returns>設置された暗調オブジェクト</returns>
    protected static Window PutDarkTone(float alpha = 1)
        => PutTone(sys.baseObjects.basicDarkTone, Color.black, alpha);
    /// <summary>
    /// 色調設置
    /// </summary>
    /// <param name="setColor">色味</param>
    /// <param name="alpha">透明度</param>
    /// <returns>設置された色調オブジェクト</returns>
    protected static Window PutColorTone(Color setColor, float alpha = 1)
        => PutTone(sys.baseObjects.colorTone, setColor, alpha);
    /// <summary>
    /// 減算調設置
    /// </summary>
    /// <param name="setColor">色味</param>
    /// <param name="alpha">透明度</param>
    /// <returns>設置された減算調オブジェクト</returns>
    protected static Window PutFadeTone(Color setColor, float alpha = 1)
        => PutTone(sys.baseObjects.fadeTone, setColor, alpha);
    /// <summary>
    /// 色調設置
    /// </summary>
    /// <param name="toneObject">色調オブジェクトの雛形</param>
    /// <param name="setColor">色味</param>
    /// <param name="alpha">透明度</param>
    /// <returns>設置された色調オブジェクト</returns>
    static Window PutTone(Window toneObject, Color setColor, float alpha)
    {
        var tone = Instantiate(toneObject);
        tone.nowParent = sysView.transform;
        tone.position = Vector3.forward * 12;
        tone.defaultLayer = Configs.SortLayers.TONE;
        tone.GetComponent<SpriteRenderer>().color = setColor;
        tone.nowSize = viewSize;
        tone.nowAlpha = alpha;
        tone.system = true;
        return tone;
    }

    /// <summary>
    /// オブジェクト検索関数
    /// </summary>
    protected static List<Type> GetAllObject<Type>(Terms<Type> map = null)
        where Type : Materials
    {
        var materialList = new List<Materials>();
        foreach(Materials value in FindObjectsOfType(typeof(Materials))) materialList.Add(value);
        return materialList
            .Where(material => material.GetComponent<Type>() != null)
            .Where(material => map == null || map(material.GetComponent<Type>()))
            .Select(material => material.GetComponent<Type>()).ToList();
    }
    /// <summary>
    /// 最大値条件型オブジェクト検索関数
    /// </summary>
    protected static List<Type> SearchMaxObject<Type>(Rank<Type> refine, Terms<Type> map = null)
        where Type : Materials
    {
        var objectList = GetAllObject(map);
        if(!objectList.Any()) return objectList;
        var maxValue = objectList.Max(_value => refine(_value));
        return objectList.Where(value => refine(value) >= maxValue).ToList();
    }
    /// <summary>
    /// 最寄りオブジェクト検索関数
    /// </summary>
    public List<Type> GetNearObject<Type>(Terms<Type> map = null)
        where Type : Materials
        => SearchMaxObject(target => -(target.globalPosition - globalPosition).magnitude, map);

    /// <summary>
    /// SE鳴らす関数
    /// </summary>
    protected AudioSource SoundSE(AudioClip soundEffect, float baseVolume = 1, float pitch = 1, bool isSystem = false)
    {
        if(soundEffect == null) return null;

        AudioSource soundObject = Instantiate(sys.baseObjects.SErootObject).GetComponent<AudioSource>();
        soundObject.transform.SetParent(transform);
        soundObject.transform.localPosition = Vector3.zero;

        soundObject.clip = soundEffect;
        soundObject.volume = Configs.Volume.se * Configs.Volume.BASE_SE * baseVolume;
        soundObject.spatialBlend = isSystem ? 0 : 1;
        soundObject.dopplerLevel = 5;
        soundObject.pitch = pitch;

        soundObject.Play();

        return soundObject;
    }

    /// <summary>
    /// システムテキストの背景画像設定
    /// </summary>
    protected static TextsWithWindow SetWindowWithText(Text withText, int timeRequired = Configs.Window.DEFAULT_MOTION_TIME)
    {
        var rectTransform = withText.GetComponent<RectTransform>();
        if(rectTransform == null) return new TextsWithWindow { text = withText };
        var setPosition = withText.getVertexPosition();
        rectTransform.localPosition -= (Vector3)(rectTransform.pivot - Vector2.one / 2) * withText.fontSize;

        var result = new TextsWithWindow
        {
            text = withText,
            backWindow = SetWindow(setPosition, timeRequired, system: true)
        };
        result.backWindow.nowSize = withText.getAreaSize().Rescaling(baseMas);
        return result;
    }
    /// <summary>
    /// システムテキストへの文字設定
    /// </summary>
    /// <param name="setText">表示する文字列</param>
    /// <param name="position">表示位置</param>
    /// <param name="pivot">表示位置を文字列全体のどの位置に持ってくるか</param>
    /// <param name="charSize">文字サイズ</param>
    /// <param name="textPosition">文字列の上下左右寄せ</param>
    /// <param name="textColor">文字色</param>
    /// <param name="lineSpace">行間空白幅</param>
    /// <param name="defaultText">変化対象の文字列オブジェクト</param>
    /// <param name="textName">変化対象の文字列名</param>
    /// <returns>生成された文字列オブジェクト</returns>
    protected static Text SetSysText(string setText,
        Vector2? position = null,
        TextAnchor pivot = TextAnchor.MiddleCenter,
        int charSize = Configs.Texts.CHAR_SIZE,
        TextAnchor textPosition = TextAnchor.UpperLeft,
        Color? textColor = null,
        float lineSpace = Configs.Texts.LINE_SPACE,
        bool bold = false,
        Text defaultText = null,
        string textName = null)
    {
        Vector2 setPosition = position ?? Vector2.zero;
        var setTextName = textName ?? setText;
        var textObject = GameObject.Find(setTextName);
        if(defaultText != null) textObject = defaultText.gameObject;
        else if(textName == null)
        {
            for(ulong index = 0; textObject != null; index++)
            {
                setTextName = $"{setText}_{index}";
                textObject = GameObject.Find(setTextName);
            }
        }
        if(textObject == null)
        {
            textObject = Instantiate(sys.baseObjects.basicText).gameObject;
            textObject.transform.SetParent(sysCanvas.transform);
            textObject.name = setTextName;
        }

        var body = textObject.GetComponent<Text>();
        body.text = setText;
        body.fontSize = charSize;
        body.alignment = textPosition;
        body.color = textColor ?? Color.white;
        body.lineSpacing = lineSpace + 1;
        body.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;

        Vector2 anchorBase = TextAnchor.MiddleCenter.GetAxis();

        var setting = textObject.GetComponent<RectTransform>();
        setting.localPosition = setPosition;
        setting.localScale = new Vector3(1, 1, 1);
        setting.anchorMin = anchorBase;
        setting.anchorMax = anchorBase;
        setting.anchoredPosition = setPosition;
        setting.pivot = pivot.GetAxis();
        setting.sizeDelta = new Vector2(body.preferredWidth, body.preferredHeight);
        //何故か一回目の参照ではpreferredHeightの値がおかしいことがあるため2回代入する
        setting.sizeDelta = new Vector2(body.preferredWidth, body.preferredHeight);

        return body;
    }
    /// <summary>
    /// システムテキストの内容取得
    /// </summary>
    /// <param name="textName">取得テキスト名</param>
    /// <returns>テキスト内容</returns>
    protected static string GetSysText(string textName)
    {
        var textObject = GameObject.Find(textName);
        if(textObject == null) return "";
        return GetSysText(textObject.GetComponent<Text>());
    }
    /// <summary>
    /// システムテキストの取得
    /// </summary>
    /// <param name="text">取得テキストオブジェクト</param>
    /// <returns>テキスト内容</returns>
    public static string GetSysText(Text text)
    {
        if(text == null) return "";
        return text.text;
    }
    /// <summary>
    /// システムテキストの削除
    /// </summary>
    /// <param name="textName">削除テキスト名</param>
    /// <returns>削除テキストの内容</returns>
    protected static string DeleteSysText(string textName)
    {
        var textObject = GameObject.Find(textName);
        if(textObject == null) return "";
        return textObject.GetComponent<Text>().selfDestroy();
    }
    /// <summary>
    /// システムテキストの幅取得
    /// </summary>
    /// <param name="setText">幅取得テキスト名</param>
    /// <param name="size">想定文字サイズ</param>
    /// <returns>テキスト幅</returns>
    protected static float GetTextWidth(string setText, int? size = null)
    {
        var setSize = size ?? Configs.Texts.CHAR_SIZE;

        var text = SetSysText(setText, charSize: setSize);
        var result = text.preferredWidth;
        text.selfDestroy();

        return result;
    }

    /// <summary>
    /// ポーズ状態変数
    /// </summary>
    protected static bool onPause { get; private set; } = false;
    /// <summary>
    /// ポーズ状態切り替え関数
    /// </summary>
    public static bool SwitchPause(bool? setPause = null)
    {
        return onPause = setPause ?? !onPause;
    }
    /// <summary>
    /// 指定条件を満たすまで待機する関数
    /// yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected static IEnumerator Wait(Func<bool> endCondition, bool isSystem = false)
    {
        while((onPause && !isSystem) || !endCondition()) yield return null;
        yield break;
    }
    /// <summary>
    /// 指定フレーム数待機する関数
    /// yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected static IEnumerator Wait(int delay, Func<bool> endCondition = null
        , bool isSystem = false)
    {
        var time = 0;
        yield return Wait(() => time++ >= delay || (endCondition?.Invoke() ?? false), isSystem);
        yield break;
    }
    /// <summary>
    /// 指定フレーム数待機する関数
    /// yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected static IEnumerator Wait(int delay, List<KeyCode> interruptions, bool isSystem = false)
    {
        yield return Wait(delay, () => interruptions.Judge(), isSystem);
        yield break;
    }
    /// <summary>
    /// 指定フレーム数待機する関数
    /// yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected static IEnumerator Wait(int delay, KeyCode interruption, bool isSystem = false)
    {
        var interruptions = new List<KeyCode> { interruption };
        yield return Wait(delay, interruptions, isSystem);
    }
    /// <summary>
    /// 複数キーのOR押下待ち動作
    /// </summary>
    protected static IEnumerator Wait(List<KeyCode> receiveableKeys, UnityAction<KeyCode?, bool> endProcess = null, bool isSystem = false)
    {
        if(receiveableKeys.Count <= 0) yield break;

        KeyCode? receivedKey = null;
        bool first = false;
        do
        {
            yield return Wait(1, isSystem: isSystem);

            if(receiveableKeys.Judge(Key.Timing.DOWN, keys => receivedKey = keys.FirstOrDefault()))
            {
                first = true;
                break;
            }
            if(receiveableKeys.Judge(Key.Timing.ON, keys => receivedKey = keys.FirstOrDefault()))
            {
                break;
            }
        } while(receivedKey == null);

        endProcess?.Invoke(receivedKey, first);
        yield break;
    }
    /// <summary>
    /// 単数キーのOR押下待ち動作
    /// </summary>
    protected static IEnumerator Wait(KeyCode receiveableKeys, UnityAction<KeyCode?, bool> endProcess = null, bool isSystem = false)
    {
        yield return Wait(new List<KeyCode> { receiveableKeys }, endProcess, isSystem);
        yield break;
    }

    /// <summary>
    /// 自身の削除予約
    /// </summary>
    public virtual void DestroyMyself(bool system = false)
    {
        nextDestroy = true;
    }
    /// <summary>
    /// 自身の削除予約フラグ
    /// </summary>
    protected bool nextDestroy { get; private set; } = false;
    /// <summary>
    /// 自身の削除実行関数
    /// </summary>
    protected virtual void ExecuteDestroy()
    {
        nextDestroy = false;
        if(gameObject == null) return;
        PickupSoundObject();
        Destroy(gameObject);
    }

    public void PickupSoundObject()
    {
        foreach(Transform target in transform)
        {
            var targetMethod = target.GetComponent<Methods>();
            if(targetMethod == null) continue;

            targetMethod.PickupSoundObject();
        }
        foreach(Transform target in transform)
        {
            var targetSEroot = target.GetComponent<SEroot>();
            if(targetSEroot == null) continue;

            targetSEroot.nowParent = nowParent;
        }
    }

    /// <summary>
    /// 全体削除関数
    /// </summary>
    protected void DestroyAll(bool exceptPlayer = false)
    {
        if(!exceptPlayer) TransparentPlayer();
        foreach(Transform target in sysPanel.transform)
        {
            if(target.GetComponent<Player>() != null) continue;

            var targetMethod = target.GetComponent<Methods>();
            if(targetMethod == null) continue;

            targetMethod.DestroyMyself(true);
        }
        return;
    }

    /// <summary>
    /// ウィンドウオブジェクト設置関数
    /// </summary>
    protected static Window SetWindow(Vector2 setPosition, int timeRequired = Configs.Window.DEFAULT_MOTION_TIME, bool system = false)
    {
        Window setWindow = Instantiate(sys.baseObjects.basicWindow);
        setWindow.nowParent = sysView.transform;
        setWindow.position = setPosition.Rescaling(baseMas);
        setWindow.timeRequired = timeRequired;
        setWindow.system = system;
        return setWindow;
    }
    /// <summary>
    /// ウィンドウオブジェクト削除関数
    /// </summary>
    protected static void DeleteWindow(Window deletedWindow, int timeRequired = 0, bool system = false)
    {
        if(deletedWindow.gameObject == null) return;
        deletedWindow.timeRequired = timeRequired;
        deletedWindow.system = system;
        deletedWindow.DestroyMyself();
    }

    protected IEnumerator GetYesOrNo(string question, UnityAction<bool> endProcess, Vector2? position = null, Vector2? questionPosition = null, string ysePhrase = "はい", string noPhrase = "いいえ", bool defaultYes = true)
    {
        var setPosition = position ?? Vector2.zero;

        int selected = 0;
        using(var textWindow = SetWindowWithText(
            SetSysText(question,
            setPosition + (questionPosition ?? (48 * Vector2.up)),
            TextAnchor.MiddleCenter)))
        {
            yield return ChoiceAction(new List<string> { ysePhrase, noPhrase },
                endProcess: result => selected = result,
                setPosition: setPosition,
                ableCancel: true,
                initialSelected: defaultYes ? 0 : 1);

            endProcess(selected == 0);
            DeleteChoices();
        }
    }

    /// <summary>
    /// myselfプロパティによってコピー作成できますよインターフェース
    /// </summary>
    public interface ICopyAble<Type>
    {
        Type myself { get; }
    }
    /// <summary>
    /// CopyAbleのListのコピー
    /// </summary>
    public static List<Type> CopyStateList<Type>(List<Type> originList) where Type : ICopyAble<Type> => originList.Select(value => value.myself).ToList();

    /// <summary>
    /// 親設定のラッパー関数
    /// </summary>
    public Transform nowParent
    {
        get {
            return transform.parent;
        }
        set {
            var keepZ = nowZ;
            transform.SetParent(value);
            nowZ = keepZ;
        }
    }
    /// <summary>
    /// 親設定のラッパー関数
    /// </summary>
    public Methods parentMethod
    {
        get {
            if(nowParent == null) return null;
            return nowParent.GetComponent<Methods>();
        }
        set {
            nowParent = value.transform;
        }
    }
    /// <summary>
    /// 配下オブジェクトのラッパー関数
    /// </summary>
    public List<Methods> nowChildren
    {
        get {
            var result = new List<Methods>();
            foreach(Transform child in transform) result.Add(child.GetComponent<Methods>());
            result = result.Where(item => item != null).ToList();
            return result;
        }
    }

    /// <summary>
    /// １マス当たりのピクセル量を得る関数
    /// </summary>
    protected float parPixel
    {
        get {
            if(GetComponent<SpriteRenderer>() == null) return 1;
            if(GetComponent<SpriteRenderer>().sprite == null) return 1;
            return GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        }
    }
    /// <summary>
    /// ベースの画像サイズ取得関数
    /// </summary>
    public Vector2 spriteSize
    {
        get {
            var spriteData = GetComponent<SpriteRenderer>();
            if(spriteData == null) return Vector2.zero;
            if(spriteData.sprite == null) return Vector2.zero;
            return spriteData.sprite.bounds.size;
        }
    }

    static Window fadeTone = null;
    protected static IEnumerator Fadein(int timeRequired = Configs.DEFAULT_FADE_TIME)
    {
        fadeTone = fadeTone ?? PutFadeTone(Color.white, 1);
        for(int time = 0; time < timeRequired; time++)
        {
            fadeTone.nowAlpha = Easing.quadratic.SubIn(time, timeRequired - 1);
            yield return Wait(1);
        }
        yield break;
    }
    protected static IEnumerator Fadeout(int timeRequired = Configs.DEFAULT_FADE_TIME)
    {
        fadeTone = fadeTone ?? PutFadeTone(Color.white, 0);
        for(int time = 0; time < timeRequired; time++)
        {
            fadeTone.nowAlpha = Easing.quadratic.Out(time, timeRequired - 1);
            yield return Wait(1);
        }
        yield break;
    }
    /// <summary>
    /// 所定のスクリプトをアタッチされたオブジェクトの生成
    /// </summary>
    /// <typeparam name="Type">アタッチするスクリプト</typeparam>
    /// <param name="objectName">オブジェクト名称</param>
    /// <returns>生成されたオブジェクト</returns>
    protected static Type InjectObject<Type>(string objectName) where Type : Methods
    {
        var injected = new GameObject(objectName, typeof(Type)).GetComponent<Type>();
        injected.nowParent = sysPanel.transform;
        return injected;
    }
    /// <summary>
    /// オブジェクトの画像ラッパープロパティ
    /// </summary>
    public Sprite nowSprite
    {
        get {
            var spriteRender = GetComponent<SpriteRenderer>();
            if(spriteRender == null) return null;
            return spriteRender.sprite;
        }
        set {
            var spriteRender = GetComponent<SpriteRenderer>();
            if(spriteRender == null) spriteRender = gameObject.AddComponent<SpriteRenderer>();
            spriteRender.sprite = value;
        }
    }
    /// <summary>
    /// オブジェクトのマテリアルラッパープロパティ
    /// </summary>
    public Material nowMaterial
    {
        get {
            var spriteRender = GetComponent<SpriteRenderer>();
            if(spriteRender == null) return null;
            return spriteRender.material;
        }
        set {
            var spriteRender = GetComponent<SpriteRenderer>();
            if(spriteRender == null) spriteRender = gameObject.AddComponent<SpriteRenderer>();
            spriteRender.material = value;
        }
    }
}
