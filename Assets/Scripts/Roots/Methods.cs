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
    public string displayName
    {
        get {
            if(_displayName != null && _displayName != "") return _displayName;
            if(gameObject != null) return gameObject.name.Replace("(Clone)", "");
            return _displayName;
        }
    }
    /// <summary>
    /// 内容説明文
    /// </summary>
    [Multiline, SerializeField]
    public string explanation = "内容説明";


    // Update is called once per frame
    public virtual void Start()
    {
        if(nowLayer == Configs.Layers.DEFAULT && parentMethod != null) nowLayer = parentMethod.nowLayer;
    }
    // Update is called once per frame
    public virtual void Update()
    {
        if(nextDestroy) executeDestroy();
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
    ///奥行き位置の設定
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
    ///奥行き位置の設定
    /// </summary>
    public virtual float globalNowZ
    {
        get {
            if(parentMethod != null) return parentMethod.globalNowZ + nowZ;
            return nowZ;
        }
    }

    public Vector2 lossyScale
    {
        get {
            if(nowParent == null) return transform.localScale;
            return ((Vector2)transform.localScale).scaling(nowParent.lossyScale);
        }
        set {
            var origin = transform.localScale;
            var setScale = nowParent != null ? value.rescaling(nowParent.lossyScale) : value;
            transform.localScale = new Vector3(setScale.x, setScale.y, origin.z);
        }
    }

    /// <summary>
    ///レイヤーの設定
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
    ///横方向の非反転フラグ
    /// </summary>
    public bool widthPositive
    {
        get {
            return lossyScale.x > 0;
        }
    }
    /// <summary>
    /// 横方向の非反転フラグ（数値版）
    /// </summary>
    public float nWidthPositive
    {
        get {
            return lossyScale.x.toSign();
        }
    }
    /// <summary>
    /// 自身の左右反転状態を加味してベクトル補完
    /// </summary>
    /// <param name="inputVector">元のベクトル</param>
    /// <returns>補正後のベクトル</returns>
    protected Vector2 correctWidthVector(Vector2 inputVector)
    {
        return new Vector2(inputVector.x * nWidthPositive, inputVector.y);
    }

    /// <summary>
    ///暗調設置
    /// </summary>
    protected Window putDarkTone(float alpha = 1)
    {
        var darkTone = Instantiate(sys.basicDarkTone);
        darkTone.nowParent = sysView.transform;
        darkTone.position = Vector3.forward * 12;
        darkTone.defaultLayer = Configs.SortLayers.DARKTONE;
        darkTone.nowSize = viewSize;
        darkTone.nowAlpha = alpha;
        darkTone.system = true;
        return darkTone;
    }

    /// <summary>
    ///オブジェクト検索関数
    /// </summary>
    protected static List<Type> getAllObject<Type>(Terms<Type> map = null)
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
    ///最大値条件型オブジェクト検索関数
    /// </summary>
    protected static List<Type> searchMaxObject<Type>(Rank<Type> refine, Terms<Type> map = null)
        where Type : Materials
    {
        var objectList = getAllObject(map);
        if(!objectList.Any()) return objectList;
        var maxValue = objectList.Max(_value => refine(_value));
        return objectList.Where(value => refine(value) >= maxValue).ToList();
    }
    /// <summary>
    ///最寄りオブジェクト検索関数
    /// </summary>
    public List<Type> getNearObject<Type>(Terms<Type> map = null)
        where Type : Materials
        => searchMaxObject(target => -(target.globalPosition - globalPosition).magnitude, map);

    /// <summary>
    ///SE鳴らす関数
    /// </summary>
    protected AudioSource soundSE(AudioClip soundEffect, float baseVolume = 1, float pitch = 1, bool isSystem = false)
    {
        if(soundEffect == null) return null;

        AudioSource soundObject = Instantiate(sys.SErootObject).GetComponent<AudioSource>();
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
    ///システムテキストの背景画像設定
    /// </summary>
    protected static TextsWithWindow setWindowWithText(Text withText, int timeRequired = Configs.Window.DEFAULT_MOTION_TIME)
    {
        var rectTransform = withText.GetComponent<RectTransform>();
        var textSpace = new Vector2(withText.preferredWidth, withText.preferredHeight);
        if(rectTransform == null) return new TextsWithWindow { text = withText };
        var areaSize = textSpace + Vector2.one * withText.fontSize;
        var setPosition = (Vector2)rectTransform.localPosition
            - (rectTransform.pivot - Vector2.one / 2).scaling(areaSize);
        rectTransform.localPosition -= (Vector3)(rectTransform.pivot - Vector2.one / 2).scaling(areaSize - textSpace);

        var result = new TextsWithWindow
        {
            text = withText,
            backWindow = setWindow(setPosition, timeRequired, system: true)
        };
        result.backWindow.nowSize = areaSize.rescaling(baseMas);
        return result;
    }
    /// <summary>
    ///システムテキストへの文字設定
    /// </summary>
    protected static Text setSysText(string setText,
        Vector2? position = null,
        TextAnchor pivot = TextAnchor.MiddleCenter,
        int charSize = Configs.DEFAULT_TEXT_SIZE,
        TextAnchor textPosition = TextAnchor.UpperLeft,
        Color? textColor = null,
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
            textObject = Instantiate(sys.basicText).gameObject;
            textObject.transform.SetParent(sysCanvas.transform);
            textObject.name = setTextName;
        }

        var body = textObject.GetComponent<Text>();
        body.text = setText;
        body.fontSize = charSize;
        body.alignment = textPosition;
        body.color = textColor ?? Color.white;

        Vector2 anchorBase = TextAnchor.MiddleCenter.getAxis();

        var setting = textObject.GetComponent<RectTransform>();
        setting.localPosition = setPosition;
        setting.localScale = new Vector3(1, 1, 1);
        setting.anchorMin = anchorBase;
        setting.anchorMax = anchorBase;
        setting.anchoredPosition = setPosition;
        setting.pivot = pivot.getAxis();
        setting.sizeDelta = new Vector2(body.preferredWidth, body.preferredHeight);
        //何故か一回目の参照ではpreferredHeightの値がおかしいことがあるため2回代入する
        setting.sizeDelta = new Vector2(body.preferredWidth, body.preferredHeight);

        return body;
    }
    /// <summary>
    ///システムテキストの取得
    /// </summary>
    protected static string getSysText(string textName)
    {
        var textObject = GameObject.Find(textName);
        if(textObject == null) return "";
        return getSysText(textObject.GetComponent<Text>());
    }
    /// <summary>
    ///システムテキストの取得
    /// </summary>
    public static string getSysText(Text text)
    {
        if(text == null) return "";
        return text.text;
    }
    /// <summary>
    ///システムテキストの削除
    /// </summary>
    protected static string deleteSysText(string textName)
    {
        var textObject = GameObject.Find(textName);
        if(textObject == null) return "";
        return textObject.GetComponent<Text>().selfDestroy();
    }
    /// <summary>
    ///システムテキストの幅取得
    /// </summary>
    protected static float getTextWidth(string setText, int? size = null)
    {
        var setSize = size ?? Configs.DEFAULT_TEXT_SIZE;

        var text = setSysText(setText, charSize: setSize);
        var result = text.preferredWidth;
        text.selfDestroy();

        return result;
    }

    /// <summary>
    ///ポーズ状態変数
    /// </summary>
    protected static bool onPause = false;
    /// <summary>
    ///ポーズ状態切り替え関数
    /// </summary>
    public static bool switchPause(bool? setPause = null)
    {
        return onPause = setPause ?? !onPause;
    }
    /// <summary>
    /// 指定条件を満たすまで待機する関数
    /// yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected static IEnumerator wait(Func<bool> endCondition, bool isSystem = false)
    {
        while((onPause && !isSystem) || !endCondition()) yield return null;
        yield break;
    }
    /// <summary>
    /// 指定フレーム数待機する関数
    /// yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected static IEnumerator wait(int delay, Func<bool> endCondition = null
        , bool isSystem = false)
    {
        var time = 0;
        yield return wait(() => time++ >= delay || (endCondition?.Invoke() ?? false), isSystem);
        yield break;
    }
    /// <summary>
    /// 指定フレーム数待機する関数
    /// yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected static IEnumerator wait(int delay, List<KeyCode> interruptions, bool isSystem = false)
    {
        yield return wait(delay, () => onKeysDecision(interruptions), isSystem);
        yield break;
    }
    /// <summary>
    /// 指定フレーム数待機する関数
    /// yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected static IEnumerator wait(int delay, KeyCode interruption, bool isSystem = false)
    {
        var interruptions = new List<KeyCode> { interruption };
        yield return wait(delay, interruptions, isSystem);
    }

    /// <summary>
    /// 自身の削除予約
    /// </summary>
    public virtual void selfDestroy(bool system = false)
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
    protected virtual void executeDestroy()
    {
        nextDestroy = false;
        if(gameObject == null) return;
        pickupSoundObject();
        Destroy(gameObject);
    }

    public void pickupSoundObject()
    {
        foreach(Transform target in transform)
        {
            var targetMethod = target.GetComponent<Methods>();
            if(targetMethod == null) continue;

            targetMethod.pickupSoundObject();
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
    protected void destroyAll(bool exceptPlayer = false)
    {
        if(!exceptPlayer) transparentPlayer();
        foreach(Transform target in sysPanel.transform)
        {
            if(target.GetComponent<Player>() != null) continue;

            var targetMethod = target.GetComponent<Methods>();
            if(targetMethod == null) continue;

            targetMethod.selfDestroy(true);
        }
        return;
    }

    /// <summary>
    /// ウィンドウオブジェクト設置関数
    /// </summary>
    protected static Window setWindow(Vector2 setPosition, int timeRequired = Configs.Window.DEFAULT_MOTION_TIME, bool system = false)
    {
        Window setWindow = Instantiate(sys.basicWindow);
        setWindow.nowParent = sysView.transform;
        setWindow.position = setPosition.rescaling(baseMas);
        setWindow.timeRequired = timeRequired;
        setWindow.system = system;
        return setWindow;
    }
    /// <summary>
    /// ウィンドウオブジェクト削除関数
    /// </summary>
    protected static void deleteWindow(Window deletedWindow, int timeRequired = 0, bool system = false)
    {
        if(deletedWindow.gameObject == null) return;
        deletedWindow.timeRequired = timeRequired;
        deletedWindow.system = system;
        deletedWindow.selfDestroy();
    }

    protected IEnumerator getYesOrNo(string question, UnityAction<bool> endProcess, Vector2? position = null, Vector2? questionPosition = null, string ysePhrase = "はい", string noPhrase = "いいえ", bool defaultYes = true)
    {
        var setPosition = position ?? Vector2.zero;

        int selected = 0;
        using(var textWindow = setWindowWithText(
            setSysText(question,
            setPosition + (questionPosition ?? (48 * Vector2.up)),
            TextAnchor.MiddleCenter)))
        {
            yield return getChoices(new List<string> { ysePhrase, noPhrase },
                endProcess: result => selected = result,
                setPosition: setPosition,
                ableCancel: true,
                initialSelected: defaultYes ? 0 : 1);

            endProcess(selected == 0);
            deleteChoices();
        }
    }

    /// <summary>
    /// 複数キーのOR押下判定
    /// </summary>
    protected static bool onKeysDecision(List<KeyCode> keys, KeyTiming timing = KeyTiming.ON)
    {
        if(keys == null || keys.Count <= 0) return false;

        keyDecision decision = T => false;
        switch(timing)
        {
            case KeyTiming.DOWN:
                decision = key => Input.GetKeyDown(key);
                break;
            case KeyTiming.ON:
                decision = key => Input.GetKey(key);
                break;
            case KeyTiming.UP:
                decision = key => Input.GetKeyUp(key);
                break;
            default:
                break;
        }

        return keys.Any(key => decision(key));
    }
    protected delegate bool keyDecision(KeyCode timing);
    protected enum KeyTiming { DOWN, ON, UP }
    /// <summary>
    /// 複数キーのOR押下待ち動作
    /// </summary>
    protected static IEnumerator waitKey(List<KeyCode> receiveableKeys, UnityAction<KeyCode?, bool> endProcess = null, bool isSystem = false)
    {
        if(receiveableKeys.Count <= 0) yield break;

        KeyCode? receivedKey = null;
        bool first = false;
        do
        {
            yield return wait(1, isSystem: isSystem);

            var pressedDownKeys = receiveableKeys.Where(key => Input.GetKeyDown(key));
            if(pressedDownKeys.Any())
            {
                receivedKey = pressedDownKeys.First();
                first = true;
                break;
            }
            var pressedKeys = receiveableKeys.Where(key => Input.GetKey(key));
            if(pressedKeys.Any())
            {
                receivedKey = pressedKeys.First();
                break;
            }
        } while(receivedKey == null);

        endProcess?.Invoke(receivedKey, first);
        yield break;
    }
    /// <summary>
    /// 単数キーのOR押下待ち動作
    /// </summary>
    protected static IEnumerator waitKey(KeyCode receiveableKeys, UnityAction<KeyCode?, bool> endProcess = null, bool isSystem = false)
    {
        yield return waitKey(new List<KeyCode> { receiveableKeys }, endProcess, isSystem);
        yield break;
    }

    /// <summary>
    ///myselfプロパティによってコピー作成できますよインターフェース
    /// </summary>
    public interface ICopyAble<Type>
    {
        Type myself { get; }
    }
    /// <summary>
    ///CopyAbleのListのコピー
    /// </summary>
    public static List<Type> copyStateList<Type>(List<Type> originList) where Type : ICopyAble<Type> => originList.Select(value => value.myself).ToList();

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
}
