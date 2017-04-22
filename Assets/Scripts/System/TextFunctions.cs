using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Textオブジェクトを操作する拡張関数群
/// </summary>
public static class TextFunctions
{
    /// <summary>
    /// Textの透明度設定
    /// </summary>
    /// <param name="text">透明度設定先のオブジェクト</param>
    /// <param name="alpha">透明度</param>
    /// <returns>Textの透明度</returns>
    public static float setAlpha(this Text text, float alpha)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        return text.color.a;
    }
    /// <summary>
    /// Textの軸店座標取得
    /// </summary>
    /// <param name="text">位置取得先のオブジェクト</param>
    /// <returns>Textの軸店座標</returns>
    public static Vector2 getPosition(this Text text) => new Vector2(text.rectTransform.localPosition.x, text.rectTransform.localPosition.y);
    /// <summary>
    /// Textの軸店座標設定
    /// </summary>
    /// <param name="text">位置設定先のオブジェクト</param>
    /// <param name="position">位置</param>
    /// <returns>Textの軸店座標</returns>
    public static Vector2 setPosition(this Text text, Vector2 position)
    {
        text.rectTransform.localPosition = new Vector3(position.x, position.y, text.rectTransform.localPosition.z);
        return text.getPosition();
    }
    /// <summary>
    /// システムテキストの削除
    /// </summary>
    /// <param name="text">対象テキストオブジェクト</param>
    /// <param name="system">システムからの削除動作か否か</param>
    /// <returns>削除した文字列の内容</returns>
    public static string selfDestroy(this Text text, bool system = false)
    {
        if(text == null) return "";
        var result = Methods.getSysText(text);
        Object.Destroy(text.gameObject);
        return result;
    }
    /// <summary>
    /// テキストの外周エリアサイズ取得
    /// </summary>
    /// <param name="text">対象テキストオブジェクト</param>
    /// <returns>テキストの外周エリアサイズ</returns>
    public static Vector2 getAreaSize(this Text text)
    {
        var textSpace = new Vector2(text.preferredWidth, text.preferredHeight);
        return textSpace + Vector2.one * text.fontSize;
    }
    /// <summary>
    /// テキストオブジェクトの頂点座標取得
    /// </summary>
    /// <param name="text">対象テキストオブジェクト</param>
    /// <param name="pivot">座標を取得したい頂点</param>
    /// <returns>頂点座標</returns>
    public static Vector2 getVertexPosition(this Text text, TextAnchor pivot = TextAnchor.MiddleCenter)
    {
        var rectTransform = text.GetComponent<RectTransform>();
        var pivotDiff = pivot.getAxis() - rectTransform.pivot;
        return text.getPosition() + pivotDiff.scaling(text.getAreaSize());
    }
}
