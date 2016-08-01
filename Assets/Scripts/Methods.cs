﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Methods : MonoBehaviour
{
    /// <summary>
    ///イージング関数群
    /// </summary>
    protected static Easing easing = new Easing();

    protected delegate bool Terms(Roots target);
    protected delegate float Rank(Roots target);

    /// <summary>
    ///ボタン1
    /// </summary>
    protected static KeyCode ButtomZ = KeyCode.Z;
    /// <summary>
    ///ボタン2
    /// </summary>
    protected static KeyCode ButtomX = KeyCode.X;
    /// <summary>
    ///ボタン3
    /// </summary>
    protected static KeyCode ButtomC = KeyCode.C;
    /// <summary>
    ///サブボタン
    /// </summary>
    protected static KeyCode ButtomSub = KeyCode.LeftShift;
    /// <summary>
    ///↑ボタン
    /// </summary>
    protected static KeyCode ButtomUp = KeyCode.UpArrow;
    /// <summary>
    ///↓ボタン
    /// </summary>
    protected static KeyCode ButtomDown = KeyCode.DownArrow;
    /// <summary>
    ///←ボタン
    /// </summary>
    protected static KeyCode ButtomLeft = KeyCode.LeftArrow;
    /// <summary>
    ///→ボタン
    /// </summary>
    protected static KeyCode ButtomRight = KeyCode.RightArrow;
    /// <summary>
    ///水平方向のキー入力
    /// </summary>
    protected static string ButtomNameWidth = "Horizontal";
    /// <summary>
    ///垂直方向のキー入力
    /// </summary>
    protected static string ButtomNameHeight = "Vertical";

    /// <summary>
    ///BGM音量
    /// </summary>
    protected static float volumeBGM = 0.3f;
    /// <summary>
    ///SE音量
    /// </summary>
    protected static float volumeSE = 0.3f;

    /// <summary>
    ///システムテキストへの文字設定
    /// </summary>
    static protected int defaultTextSize = 18;

    /// <summary>
    ///メインシステム記憶キャッシュ
    /// </summary>
    private static MainSystems systemRoot = null;
    /// <summary>
    ///メインシステムオブジェクト取得関数
    /// </summary>
    static protected MainSystems getSystem()
    {
        return systemRoot = systemRoot ?? GameObject.Find("SystemRoot").GetComponent<MainSystems>();
    }

    /// <summary>
    ///プレイヤーオブジェクト名
    /// </summary>
    protected static string playerName = "player";
    /// <summary>
    ///プレイヤー記憶キャッシュ
    /// </summary>
    private static Player player = null;
    /// <summary>
    ///プレイヤーオブジェクト取得関数
    /// </summary>
    static protected Player getPlayer()
    {
        if (player != null) return player;
        return player = GameObject.Find(playerName) != null
            ? GameObject.Find(playerName).GetComponent<Player>()
            : null;
    }

    /// <summary>
    ///オブジェクト検索関数
    /// </summary>
    protected static List<Roots> getAllObject(Terms map = null)
    {
        var returnList = new List<Roots>();
        foreach (Roots value in FindObjectsOfType(typeof(Roots)))
        {
            if (map == null || map(value)) returnList.Add(value);
        }
        return returnList;
    }
    /// <summary>
    ///最大値条件型オブジェクト検索関数
    /// </summary>
    protected static List<Roots> searchMaxObject(Rank refine, Terms map = null)
    {
        List<Roots> returnList = new List<Roots>();
        foreach (var value in getAllObject(map))
        {
            if (returnList.Count <= 0)
            {
                returnList.Add(value);
            }
            else if (refine(value) > refine(returnList[0]))
            {
                returnList = new List<Roots> { value };
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
    protected List<Roots> getNearObject(Terms map = null)
    {
        return searchMaxObject(target => -(target.transform.position - transform.position).magnitude, map);
    }

    /// <summary>
    ///ベクトル関係の汎用計算クラス
    ///一部オーバーロード用の数値クラスが混じっているので注意
    /// </summary>
    protected static class MathV
    {
        /// <summary>
        ///大きい方のベクトルを取得
        /// </summary>
        public static Vector2 Max(Vector2 main, Vector2 sub)
        {
            return main.magnitude >= sub.magnitude ? main : sub;
        }
        /// <summary>
        ///ベクトル長の最小値を設定
        /// </summary>
        public static Vector2 Max(Vector2 main, float limit)
        {
            return Max(main, main.normalized * limit);
        }
        /// <summary>
        ///小さい方のベクトルを取得
        /// </summary>
        public static Vector2 Min(Vector2 main, Vector2 sub)
        {
            return main.magnitude <= sub.magnitude ? main : sub;
        }
        /// <summary>
        ///ベクトル長の最大値を設定
        /// </summary>
        public static Vector2 Min(Vector2 main, float limit)
        {
            return Min(main, main.normalized * limit);
        }
        /// <summary>
        ///mainのベクトルをsubに合わせて補正する
        /// </summary>
        public static Vector2 correctValue(Vector2 main, Vector2 sub, float degree = 0.5f)
        {
            return main * degree + sub * (1 - degree);
        }
        /// <summary>
        ///mainの数値をsubに合わせて補正する
        /// </summary>
        public static float correctValue(float main, float sub, float degree = 0.5f)
        {
            return main * degree + sub * (1 - degree);
        }
    }

    /// <summary>
    ///SE鳴らす関数
    /// </summary>
    protected AudioSource soundSE(AudioClip soundEffect, float baseVolume = 0, float pitch = 1)
    {
        AudioSource soundObject = GetComponent<AudioSource>();
        if (soundObject == null) soundObject = gameObject.AddComponent<AudioSource>();

        soundObject.clip = soundEffect;
        soundObject.volume = volumeSE + baseVolume;
        soundObject.playOnAwake = false;
        soundObject.pitch = pitch;

        soundObject.Play();

        return soundObject;
    }

    /// <summary>
    ///システムテキストへの文字設定
    /// </summary>
    protected Text setSysText(string setText, string textName, Vector2? position = null, int? size = null, TextAnchor textPosition = TextAnchor.UpperLeft)
    {
        Vector2 setPosition = position ?? new Vector2(0, 0);
        GameObject textObject = GameObject.Find(textName)
            ?? Instantiate(getSystem().basicText).gameObject;
        textObject.transform.SetParent(GameObject.Find("Canvas").transform);
        textObject.name = textName;

        var body = textObject.GetComponent<Text>();
        body.text = setText;
        body.fontSize = size ?? defaultTextSize;
        body.alignment = textPosition;

        Vector2 axis = new Vector2(0, 0);
        switch (textPosition)
        {
            case TextAnchor.UpperLeft:
                axis += Vector2.up;
                break;
            case TextAnchor.UpperCenter:
                axis += Vector2.right / 2 + Vector2.up;
                break;
            case TextAnchor.UpperRight:
                axis += Vector2.right + Vector2.up;
                break;
            case TextAnchor.MiddleLeft:
                axis += Vector2.up / 2;
                break;
            case TextAnchor.MiddleCenter:
                axis += Vector2.right / 2 + Vector2.up / 2;
                break;
            case TextAnchor.MiddleRight:
                axis += Vector2.right + Vector2.up / 2;
                break;
            case TextAnchor.LowerLeft:
                axis += Vector2.zero;
                break;
            case TextAnchor.LowerCenter:
                axis += Vector2.right / 2;
                break;
            case TextAnchor.LowerRight:
                axis += Vector2.right;
                break;
            default:
                break;
        }

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
    protected void deleteSysText(string textName)
    {
        var textObject = GameObject.Find(textName);
        if (textObject == null) return;
        Destroy(textObject);
        return;
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

    /// <summary>
    ///指定フレーム数待機する関数
    ///yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected IEnumerator wait(int delay, KeyCode? interruption = null)
    {
        for (var i = 0; i < delay; i++)
        {
            if (interruption != null && Input.GetKeyDown((KeyCode)interruption)) yield break;
            yield return null;
        }
        yield break;
    }
    /// <summary>
    /// 自身の削除関数
    /// </summary>
    public virtual void selfDestroy()
    {
        Destroy(gameObject);
    }

    protected class Easing
    {
        /// <summary>
        ///線形変動
        /// </summary>
        public Linear liner = new Linear();
        /// <summary>
        ///二乗変動
        /// </summary>
        public Quadratic quadratic = new Quadratic();
        /// <summary>
        ///三乗変動
        /// </summary>
        public Cubic cubic = new Cubic();
        /// <summary>
        ///四乗変動
        /// </summary>
        public Quartic quartic = new Quartic();
        /// <summary>
        ///五乗変動
        /// </summary>
        public Quintic quintic = new Quintic();
        /// <summary>
        ///円形変動
        /// </summary>
        public Sinusoidal sinusoidal = new Sinusoidal();
        /// <summary>
        ///累乗変動
        /// </summary>
        public Exponential exponential = new Exponential();
        /// <summary>
        ///乗根変動
        /// </summary>
        public Circular circular = new Circular();

        public class BaseEaaing
        {
            public virtual float In(float max, float time, float limit)
            {
                Debug.Log(max);
                return max;
            }
            public float In(float time, float limit)
            {
                return In(1, time, limit);
            }
            public float Out(float max, float time, float limit)
            {
                return max - In(max, limit - time, limit);
            }
            public float Out(float time, float limit)
            {
                return Out(1, time, limit);
            }
            public float InOut(float max, float time, float limit)
            {
                return time < limit / 2
                    ? In(max / 2, time, limit / 2)
                    : Out(max / 2, time - limit / 2, limit / 2) + max / 2;
            }
            public float InOut(float time, float limit)
            {
                return InOut(1, time, limit);
            }
        }
        public class Linear : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time / limit;
            }
        }
        public class Quadratic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time * time / limit / limit;
            }
        }
        public class Cubic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time * time * time / limit / limit / limit;
            }
        }
        public class Quartic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time * time * time * time / limit / limit / limit / limit;
            }
        }
        public class Quintic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time * time * time * time * time / limit / limit / limit / limit / limit;
            }
        }
        public class Sinusoidal : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return -max * Mathf.Cos(time * Mathf.PI / limit / 2) + max;
            }
        }
        public class Exponential : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * Mathf.Pow(2, 10 * (time - limit) / limit);
            }
        }
        public class Circular : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return -max * (Mathf.Sqrt(1 - time * time / limit / limit) - 1);
            }
        }
    }
}
