using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

public static class Functions
{
    /// <summary>
    ///boolを整数0,1に変換
    /// </summary>
    public static int toInt(this bool value) => value ? 1 : 0;
    /// <summary>
    /// ボタンの入力状態を整数0,1に変換
    /// テストはボタン入力エミュレートがよくわからなくて書いてないので注意
    /// </summary>
    public static int toInt(this KeyCode buttom) => toInt(Input.GetKey(buttom));

    /// <summary>
    ///boolを正負符号に変換
    /// </summary>
    public static int toSign(this bool value) => value ? 1 : -1;
    /// <summary>
    ///適当な数を正負符号に変換
    /// </summary>
    public static int toSign(this float value)
    {
        if(value > 0) return 1;
        if(value < 0) return -1;
        return 0;
    }

    /// <summary>
    /// オブジェクトのリストから特定コンポーネントのリストへの変換
    /// </summary>
    public static List<Output> toComponents<Output, Input>(this List<Input> originList)
        where Output : Methods
        where Input : MonoBehaviour
        => originList
        .Where(methods => methods != null)
        .Where(methods => methods.GetComponent<Output>() != null)
        .Select(methods => methods.GetComponent<Output>()).ToList();
    /// <summary>
    /// オブジェクトのリストから特定コンポーネントのリストへの変換
    /// </summary>
    public static List<Output> toComponents<Output>(this List<Methods> originList)
        where Output : Methods
        => toComponents<Output, Methods>(originList);

    /// <summary>
    /// 等値判定実装してるクラス同士のNULL許可型等値比較メソッド
    /// </summary>
    /// <typeparam name="Data">等値判定可能な型</typeparam>
    /// <param name="data1">比較対象前者</param>
    /// <param name="data2">比較対象後者</param>
    /// <returns>比較結果</returns>
    public static bool EqualsValue<Data>(this Data data1, Data data2)
        where Data : System.IEquatable<Data>
    {
        if(data1 == null && data2 != null) return false;
        if(data1 != null && data2 == null) return false;
        if(data1 == null && data2 == null) return true;
        return data1.Equals(data2);
    }
    /// <summary>
    /// 等値判定実装してるクラスのリスト同士の等値判定
    /// </summary>
    public static bool EqualsList<Type>(this List<Type> originList, List<Type> otherList)
        where Type : System.IEquatable<Type>
    {
        if(originList == null && otherList != null) return false;
        if(originList != null && otherList == null) return false;
        if(originList == null && otherList == null) return true;

        if(originList.Count != otherList.Count) return false;
        for(int index = 0; index < originList.Count; index++)
        {
            if(!originList[index].EqualsValue(otherList[index])) return false;
        }

        return true;
    }

    /// <summary>
    /// 選択肢と選択可能性からランダムで選択肢を選び出す
    /// </summary>
    public static Result selectRandom<Result>(this IEnumerable<Result> values, IEnumerable<int> rates = null)
    {
        rates = rates ?? values.Select(_ => 1);
        var rateValues = rates
            .Select((rate, index) => new
            {
                rate = rate,
                value = values.ToList()[index],
                sumRate = rates
                .Where((__, _index) => _index <= index)
                .Sum(_rate => Mathf.Max(_rate, 0))
            })
            .ToList();
        var selection = Random.Range(0, rates.Sum(rate => Mathf.Max(rate, 0)));
        var results = rateValues
            .Where(rateValue => rateValue.rate >= 0)
            .Where(rateValue => rateValue.sumRate > selection)
            .Where(rateValue => rateValue.sumRate - rateValue.rate <= selection)
            .Select(rateValue => rateValue.value).ToList();
        return results.Single();
    }

    /// <summary>
    /// 数値の上昇度合いを抑える補正関数
    /// </summary>
    /// <param name="origin">元値</param>
    /// <param name="_baseNumber">補正の底数</param>
    /// <returns>補正後の数値</returns>
    public static float log(this float origin, float? _baseNumber = null)
    {
        var baseNumber = _baseNumber ?? Mathf.Exp(1);
        return Mathf.Log(Mathf.Abs(origin) + 1, baseNumber) * origin.toSign();
    }

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
    /// 帯状のエフェクトを指定座標中心に配置する
    /// </summary>
    /// <param name="effect">表示するエフェクト</param>
    /// <param name="central">表示座標中心</param>
    /// <param name="size">エフェクトのサイズ指定</param>
    /// <param name="orign"></param>
    /// <returns></returns>
    public static List<Effect> setStrip(this List<Effect> orign, Effect effect, Vector2 central, float size = 1)
    {
        var effectList = orign ?? new List<Effect>();
        var width = effect.spriteSize.y * size * 2;
        var total = Mathf.CeilToInt(Methods.viewSize.y / 2 / width + 1) * 2 + 1;
        Debug.Log($"{effect.spriteSize} => {width}");

        for(int index = 0; index < total; index++)
        {
            if(effectList.Count <= index)
            {
                effectList.Add(Object.Instantiate(effect));
                effectList[index].nowParent = Methods.sysView.transform;
            }

            var position = central + Vector2.right * width * (index - (total - 1) / 2);
            var sideLimit = (Methods.viewSize.x + width * 2) / 2;
            var diff = Vector2.right * width * total;
            while(position.x > sideLimit) position -= diff;
            while(position.x < -sideLimit) position += diff;

            if(effectList[index] != null)
            {
                effectList[index].position = position;
                effectList[index].lossyScale = Vector2.one * size;
                effectList[index].nowAlpha = 1;
            }
        }
        return effectList;
    }
}
