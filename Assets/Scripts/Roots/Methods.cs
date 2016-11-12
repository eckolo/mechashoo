using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public partial class Methods : MonoBehaviour
{
    protected delegate bool Terms(Materials target);
    protected delegate float Rank(Materials target);
    protected delegate IEnumerator PublicAction<Type>(Type value);

    /// <summary>
    ///システムテキストへの文字設定
    /// </summary>
    protected const int defaultTextSize = 12;

    /// <summary>
    ///オブジェクト検索関数
    /// </summary>
    protected static List<Materials> getAllObject(Terms map = null)
    {
        var returnList = new List<Materials>();
        foreach (Materials value in FindObjectsOfType(typeof(Materials)))
        {
            if (map == null || map(value)) returnList.Add(value);
        }
        return returnList;
    }
    /// <summary>
    ///最大値条件型オブジェクト検索関数
    /// </summary>
    protected static List<Materials> searchMaxObject(Rank refine, Terms map = null)
    {
        List<Materials> returnList = new List<Materials>();
        foreach (var value in getAllObject(map))
        {
            if (returnList.Count <= 0)
            {
                returnList.Add(value);
            }
            else if (refine(value) > refine(returnList[0]))
            {
                returnList = new List<Materials> { value };
            }
            else if (refine(value) == refine(returnList[0]))
            {
                returnList.Add(value);
            }
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
    protected AudioSource soundSE(AudioClip soundEffect, float baseVolume = 1, float pitch = 1)
    {
        if (soundEffect == null) return null;

        AudioSource soundObject = Instantiate(Sys.SErootObject).GetComponent<AudioSource>();

        soundObject.clip = soundEffect;
        soundObject.volume = volumeSE * baseVolumeSE * baseVolume;
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
        switch (anchor)
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

    /// <summary>
    ///システムテキストへの文字設定
    /// </summary>
    protected static Text setSysText(string setText, string textName, Vector2? position = null, int? size = null, TextAnchor textPosition = TextAnchor.UpperLeft)
    {
        Vector2 setPosition = position ?? Vector2.zero;
        GameObject textObject = GameObject.Find(textName);
        if (textObject == null)
        {
            textObject = Instantiate(Sys.basicText).gameObject;
            textObject.transform.SetParent(sysCanvas.transform);
            textObject.name = textName;
        }

        var body = textObject.GetComponent<Text>();
        body.text = setText;
        body.fontSize = size ?? defaultTextSize;
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
    ///システムテキストへの文字設定
    ///位置指定バラバラ版
    /// </summary>
    protected void setSysText(string setText, string textName, float posX, float posY)
    {
        setSysText(setText, textName, new Vector2(posX, posY));
        return;
    }
    /// <summary>
    ///システムテキストの取得
    /// </summary>
    protected string getSysText(string textName)
    {
        var textObject = GameObject.Find(textName);
        if (textObject == null) return "";
        return textObject.GetComponent<Text>().text;
    }
    /// <summary>
    ///システムテキストの削除
    /// </summary>
    protected static void deleteSysText(string textName)
    {
        var textObject = GameObject.Find(textName);
        if (textObject == null) return;
        Destroy(textObject);
        return;
    }
    protected static float getTextWidth(string setText, int? size = null)
    {
        const string temporary = "temporary";

        var returnValue = setSysText(setText, temporary, Vector2.zero, size).GetComponent<RectTransform>().sizeDelta.x;
        deleteSysText(temporary);

        return returnValue;
    }

    /// <summary>
    ///複数キーのOR押下判定
    /// </summary>
    protected bool onKeysDecision(List<KeyCode> keys, keyTiming timing = keyTiming.on)
    {
        if (keys == null || keys.Count <= 0) return false;

        keyDecision decision = T => false;
        switch (timing)
        {
            case keyTiming.down:
                decision = key => Input.GetKeyDown(key);
                break;
            case keyTiming.on:
                decision = key => Input.GetKey(key);
                break;
            case keyTiming.up:
                decision = key => Input.GetKeyUp(key);
                break;
            default:
                break;
        }

        foreach (var key in keys) if (decision(key)) return true;
        return false;
    }
    protected delegate bool keyDecision(KeyCode timing);
    protected enum keyTiming { down, on, up }

    /// <summary>
    ///指定フレーム数待機する関数
    ///yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected IEnumerator wait(int delay, List<KeyCode> interruptions)
    {
        for (var i = 0; i < delay; i++)
        {
            if (onKeysDecision(interruptions)) yield break;
            yield return null;
        }
        yield break;
    }
    /// <summary>
    ///指定フレーム数待機する関数
    ///yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected IEnumerator wait(int delay, KeyCode? interruption = null)
    {
        List<KeyCode> interruptions = new List<KeyCode>();
        if (interruption != null) interruptions.Add((KeyCode)interruption);
        yield return wait(delay, interruptions);
    }

    /// <summary>
    /// 自身の削除関数
    /// </summary>
    public virtual void selfDestroy(bool system = false)
    {
        if (gameObject == null) return;
        Destroy(gameObject);
    }
    /// <summary>
    /// 全体削除関数
    /// </summary>
    protected void destroyAll()
    {
        foreach (Transform target in sysPanel.transform)
        {
            if (target.GetComponent<Player>() != null) continue;

            var targetMethod = target.GetComponent<Methods>();
            if (targetMethod == null) continue;

            targetMethod.selfDestroy(true);
        }

        selfDestroy(true);
        return;
    }

    //static private List<Window> windowList = new List<Window>();
    /// <summary>
    ///ウィンドウオブジェクト設置関数
    /// </summary>
    static protected Window setWindow(Vector2 setPosition)
    {
        Window setWindow = Instantiate(Sys.basicWindow);
        setWindow.transform.SetParent(sysView.transform);
        setWindow.position = viewPosition + MathV.rescaling(setPosition, baseMas);
        return setWindow;
    }
    /// <summary>
    ///ウィンドウオブジェクト削除関数
    /// </summary>
    static protected void deleteWindow(Window deletedWindow)
    {
        if (deletedWindow.gameObject == null) return;
        deletedWindow.selfDestroy();
    }

    protected static IEnumerator waitKey(List<KeyCode> receiveableKeys, UnityAction<KeyCode?, bool> endProcess)
    {
        if (receiveableKeys.Count <= 0) yield break;

        KeyCode? receivedKey = null;
        bool first = false;
        do
        {
            yield return null;

            foreach (var receiveableKey in receiveableKeys)
            {
                if (Input.GetKeyDown(receiveableKey))
                {
                    receivedKey = receiveableKey;
                    first = true;
                    break;
                }
                if (Input.GetKey(receiveableKey))
                {
                    receivedKey = receiveableKey;
                    break;
                }
            }
        } while (receivedKey == null);

        endProcess(receivedKey, first);
        yield break;
    }

    /// <summary>
    ///myselfプロパティによってコピー作成できますよ属性
    /// </summary>
    public interface CopyAble<Type>
    {
        Type myself { get; }
    }
    /// <summary>
    ///CopyAbleのListのコピー
    /// </summary>
    public static List<Type> copyStateList<Type>(List<Type> originList) where Type : CopyAble<Type>
    {
        return originList.Select(value => value.myself).ToList();
    }

    /// <summary>
    ///boolを整数に変換
    /// </summary>
    protected int toInt(bool value) { return (value ? 1 : 0); }
    /// <summary>
    ///ボタンの入力状態を整数に変換
    /// </summary>
    protected int toInt(KeyCode buttom) { return toInt(Input.GetKey(buttom)); }
}
