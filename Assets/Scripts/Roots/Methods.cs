using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System;

public partial class Methods : MonoBehaviour
{
    protected delegate bool Terms(Materials target);
    protected delegate float Rank(Materials target);
    protected delegate IEnumerator PublicAction<Type>(Type value);

    /// <summary>
    ///暗調設置
    /// </summary>
    protected Window putDarkTone(float alpha = 1)
    {
        var darkTone = Instantiate(sys.basicDarkTone);
        darkTone.transform.SetParent(sysView.transform);
        darkTone.position = Vector3.forward * 12;
        darkTone.nowOrder = Order.DARKTONE;
        darkTone.size = viewSize;
        darkTone.setAlpha(alpha);
        darkTone.system = true;
        return darkTone;
    }

    /// <summary>
    ///オブジェクト検索関数
    /// </summary>
    protected static List<Materials> getAllObject(Terms map = null)
    {
        var returnList = new List<Materials>();
        foreach(Materials value in FindObjectsOfType(typeof(Materials)))
        {
            if(map == null || map(value)) returnList.Add(value);
        }
        return returnList;
    }
    /// <summary>
    ///最大値条件型オブジェクト検索関数
    /// </summary>
    protected static List<Materials> searchMaxObject(Rank refine, Terms map = null)
    {
        var returnList = new List<Materials>();
        foreach(var value in getAllObject(map))
        {
            if(returnList.Count <= 0) returnList.Add(value);
            else if(refine(value) > refine(returnList[0])) returnList = new List<Materials> { value };
            else if(refine(value) == refine(returnList[0])) returnList.Add(value);
        }

        return returnList;
    }
    /// <summary>
    ///最寄りオブジェクト検索関数
    /// </summary>
    protected List<Materials> getNearObject(Terms map = null)
    {
        return searchMaxObject(target => -(target.transform.position - transform.position).magnitude, map);
    }

    /// <summary>
    ///回転計算関数
    /// </summary>
    protected static Quaternion getRotation(Quaternion baseRotation, float calculation)
    {
        Vector3 axis = new Vector3(baseRotation.x, baseRotation.y, baseRotation.z).normalized;
        return new Quaternion(axis.x, axis.y, axis.z, baseRotation.w * calculation);
    }
    /// <summary>
    ///逆回転生成関数
    /// </summary>
    protected static Quaternion getReverse(Quaternion baseRotation)
    {
        return getRotation(baseRotation, -1);
    }

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
        soundObject.volume = Volume.se * Volume.BASE_SE * baseVolume;
        soundObject.spatialBlend = isSystem ? 0 : 1;
        soundObject.dopplerLevel = 5;
        soundObject.pitch = pitch;

        soundObject.Play();

        return soundObject;
    }

    /// <summary>
    ///アンカーパラメータからアンカー一座標を取得する関数
    /// </summary>
    protected static Vector2 getAxis(TextAnchor anchor, TextAnchor? pibot = null)
    {
        var pibotPosition = pibot != null ? getAxis((TextAnchor)pibot) : Vector2.zero;
        switch(anchor)
        {
            case TextAnchor.UpperLeft:
                return Vector2.up - pibotPosition;
            case TextAnchor.UpperCenter:
                return Vector2.right / 2 + Vector2.up - pibotPosition;
            case TextAnchor.UpperRight:
                return Vector2.right + Vector2.up - pibotPosition;
            case TextAnchor.MiddleLeft:
                return Vector2.up / 2 - pibotPosition;
            case TextAnchor.MiddleCenter:
                return Vector2.right / 2 + Vector2.up / 2 - pibotPosition;
            case TextAnchor.MiddleRight:
                return Vector2.right + Vector2.up / 2 - pibotPosition;
            case TextAnchor.LowerLeft:
                return Vector2.zero - pibotPosition;
            case TextAnchor.LowerCenter:
                return Vector2.right / 2 - pibotPosition;
            case TextAnchor.LowerRight:
                return Vector2.right - pibotPosition;
            default:
                return Vector2.right / 2 + Vector2.up / 2 - pibotPosition;
        }
    }

    protected class TextsWithWindow
    {
        public TextsWithWindow()
        {
            textNames = new List<string>();
        }
        public List<string> textNames
        {
            get
            {
                return texts.Select(textObj => textObj.name).ToList();
            }
            set
            {
                texts = value.Select(name => GameObject.Find(name).GetComponent<Text>()).ToList();
            }
        }
        public List<Text> texts { get; set; }
        public Window backWindow { get; set; }
        public Vector2 underLeft
        {
            get
            {
                if(backWindow == null) return position - textArea / 2;
                return backWindow.underLeft;
            }
        }
        public Vector2 upperRight
        {
            get
            {
                if(backWindow == null) return position + textArea / 2;
                return backWindow.upperRight;
            }
        }
        Vector2 position
        {
            get
            {
                if(backWindow == null)
                {
                    return texts
                         .Select(textObj => textObj.GetComponent<RectTransform>().localPosition)
                         .Aggregate((vec1, vec2) => vec1 + vec2) / texts.Count;
                }
                return MathV.scaling(backWindow.position, baseMas);
            }
        }
        Vector2 textArea
        {
            get
            {
                var allText = texts
                    .Select(textObj => textObj.text)
                    .Aggregate((text1, text2) => text1 + text2);
                var width = allText
                    .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => getTextWidth(line))
                    .Max();
                var height = texts.First().fontSize * 1.5f * getLines(allText);
                return new Vector2(width, height);
            }
        }
    }
    /// <summary>
    ///システムテキストへの文字設定
    /// </summary>
    protected static Text setSysText(string setText,
        string textName,
        Vector2? position = null,
        TextAnchor pibot = TextAnchor.MiddleCenter,
        int? size = null,
        TextAnchor textPosition = TextAnchor.UpperLeft,
        bool withWindow = true)
    {
        Vector2 basePosition = position ?? Vector2.zero;
        int setTextSize = size ?? DEFAULT_TEXT_SIZE;

        return setSysTextCore(textName, setText, setTextSize, setPosition, textPosition);
    }
    /// <summary>
    ///システムテキストへの文字設定
    ///位置指定バラバラ版
    /// </summary>
    protected void setSysText(string setText, string textName, float posX, float posY)
    {
        setSysText(setText, textName, new Vector2(posX, posY));
        return;
    }
    /// <summary>
    ///システムテキストへの文字設定コア処理
    /// </summary>
    static Text setSysTextCore(string textName, string setText, int setTextSize, Vector2 setPosition, TextAnchor textPosition = TextAnchor.UpperLeft)
    {
        var textObject = GameObject.Find(textName);
        if(textObject == null)
        {
            textObject = Instantiate(sys.basicText).gameObject;
            textObject.transform.SetParent(sysCanvas.transform);
            textObject.name = textName;
        }

        var body = textObject.GetComponent<Text>();
        body.text = setText;
        body.fontSize = setTextSize;
        body.alignment = textPosition;

        Vector2 axis = getAxis(textPosition);

        var setting = textObject.GetComponent<RectTransform>();
        setting.localPosition = setPosition;
        setting.localScale = new Vector3(1, 1, 1);
        setting.anchorMin = axis;
        setting.anchorMax = axis;
        setting.anchoredPosition = setPosition;
        setting.sizeDelta = new Vector2(body.preferredWidth, body.preferredHeight);
        setting.pivot = axis;

        return body;
    }
    /// <summary>
    ///システムテキストの取得
    /// </summary>
    protected static string getSysText(string textName)
    {
        var textObject = GameObject.Find(textName);
        if(textObject == null) return "";
        return textObject.GetComponent<Text>().text;
    }
    /// <summary>
    ///システムテキストの削除
    /// </summary>
    protected static void deleteSysText(string textName)
    {
        var textObject = GameObject.Find(textName);
        if(textObject == null) return;
        Destroy(textObject);
        return;
    }
    protected static float getTextWidth(string setText, int? size = null)
    {
        const string temporary = "temporary";
        var setSize = size ?? DEFAULT_TEXT_SIZE;

        var result = setSysTextCore(temporary, setText, setSize, Vector2.zero)
            .GetComponent<RectTransform>()
            .sizeDelta.x;
        deleteSysText(temporary);

        return result;
    }

    /// <summary>
    ///複数キーのOR押下判定
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

        foreach(var key in keys) if(decision(key)) return true;
        return false;
    }
    protected delegate bool keyDecision(KeyCode timing);
    protected enum KeyTiming { DOWN, ON, UP }

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
    ///指定フレーム数待機する関数
    ///yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected static IEnumerator wait(int delay, List<KeyCode> interruptions, bool isSystem = false)
    {
        for(var time = 0; time < delay; time++)
        {
            if(onPause && !isSystem) time -= 1;
            else if(onKeysDecision(interruptions)) yield break;
            yield return null;
        }
        yield break;
    }
    /// <summary>
    ///指定フレーム数待機する関数
    ///yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected static IEnumerator wait(int delay, KeyCode? interruption = null, bool system = false)
    {
        var interruptions = new List<KeyCode>();
        if(interruption != null) interruptions.Add((KeyCode)interruption);
        yield return wait(delay, interruptions, system);
    }

    /// <summary>
    /// 自身の削除関数
    /// </summary>
    public virtual void selfDestroy(bool system = false)
    {
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

            targetSEroot.transform.SetParent(transform.parent);
        }
    }

    /// <summary>
    /// 全体削除関数
    /// </summary>
    protected void destroyAll()
    {
        transparentPlayer();
        foreach(Transform target in sysPanel.transform)
        {
            if(target.GetComponent<Player>() != null) continue;

            var targetMethod = target.GetComponent<Methods>();
            if(targetMethod == null) continue;

            targetMethod.selfDestroy(true);
        }

        selfDestroy(true);
        return;
    }

    /// <summary>
    ///ウィンドウオブジェクト設置関数
    /// </summary>
    protected Window setWindow(Vector2 setPosition, int timeRequired = 0, bool system = false)
    {
        Window setWindow = Instantiate(sys.basicWindow);
        setWindow.transform.SetParent(sysView.transform);
        setWindow.position = MathV.rescaling(setPosition, baseMas);
        setWindow.timeRequired = timeRequired;
        setWindow.system = system;
        return setWindow;
    }
    /// <summary>
    ///ウィンドウオブジェクト削除関数
    /// </summary>
    protected void deleteWindow(Window deletedWindow, int timeRequired = 0, bool system = false)
    {
        if(deletedWindow.gameObject == null) return;
        deletedWindow.timeRequired = timeRequired;
        deletedWindow.system = system;
        deletedWindow.selfDestroy();
    }

    protected static IEnumerator waitKey(List<KeyCode> receiveableKeys, UnityAction<KeyCode?, bool> endProcess, bool isSystem = false)
    {
        if(receiveableKeys.Count <= 0) yield break;

        KeyCode? receivedKey = null;
        bool first = false;
        do
        {
            yield return wait(1, system: isSystem);

            foreach(var receiveableKey in receiveableKeys)
            {
                if(Input.GetKeyDown(receiveableKey))
                {
                    receivedKey = receiveableKey;
                    first = true;
                    break;
                }
                if(Input.GetKey(receiveableKey))
                {
                    receivedKey = receiveableKey;
                    break;
                }
            }
        } while(receivedKey == null);

        endProcess(receivedKey, first);
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
    public static List<Type> copyStateList<Type>(List<Type> originList) where Type : ICopyAble<Type>
    {
        return originList.Select(value => value.myself).ToList();
    }

    /// <summary>
    ///boolを整数に変換
    /// </summary>
    protected static int toInt(bool value) { return (value ? 1 : 0); }
    /// <summary>
    ///boolを正負符号に変換
    /// </summary>
    protected static int toSign(bool value) { return (value ? 1 : -1); }
    /// <summary>
    ///適当な数を正負符号に変換
    /// </summary>
    protected static int toSign(float value)
    {
        if(value > 0) return 1;
        if(value < 0) return -1;
        return 0;
    }

    protected static int getLines(string text)
    {
        return text.ToList().Where(character => character.Equals("\r\n")).Count() + 1;
    }

    /// <summary>
    ///ボタンの入力状態を整数に変換
    /// </summary>
    protected int toInt(KeyCode buttom) { return toInt(Input.GetKey(buttom)); }

    protected static bool isDefined<enumType>(int value)
    {
        return System.Enum.IsDefined(typeof(enumType), value);
    }
}
