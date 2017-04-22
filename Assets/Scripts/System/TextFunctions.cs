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
    /// <returns></returns>
    public static float setAlpha(this Text text, float alpha)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        return text.color.a;
    }
    /// <summary>
    /// Textの位置設定
    /// </summary>
    /// <param name="text">位置設定先のオブジェクト</param>
    /// <param name="position">位置</param>
    /// <returns></returns>
    public static Vector2 setPosition(this Text text, Vector2 position)
    {
        text.rectTransform.localPosition = new Vector3(position.x, position.y, text.rectTransform.localPosition.z);
        return new Vector2(text.rectTransform.localPosition.x, text.rectTransform.localPosition.y);
    }
    /// <summary>
    ///システムテキストの削除
    /// </summary>
    public static string selfDestroy(this Text text, bool system = false)
    {
        if(text == null) return "";
        var result = Methods.getSysText(text);
        Object.Destroy(text.gameObject);
        return result;
    }
}
