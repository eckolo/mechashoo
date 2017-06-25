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
    public static int ToInt(this bool value) => value ? 1 : 0;
    /// <summary>
    /// ボタンの入力状態を整数0,1に変換
    /// テストはボタン入力エミュレートがよくわからなくて書いてないので注意
    /// </summary>
    public static int ToInt(this KeyCode buttom) => ToInt(buttom.Judge(Key.Timing.ON));

    /// <summary>
    ///boolを正負符号に変換
    /// </summary>
    public static int ToSign(this bool value) => value ? 1 : -1;
    /// <summary>
    ///適当な数を正負符号に変換
    /// </summary>
    public static int ToSign(this float value)
    {
        if(value > 0) return 1;
        if(value < 0) return -1;
        return 0;
    }

    /// <summary>
    /// オブジェクトのリストから特定コンポーネントのリストへの変換
    /// </summary>
    public static List<Output> ToComponents<Output, Input>(this List<Input> originList)
        where Output : Methods
        where Input : MonoBehaviour
        => originList
        .Where(methods => methods != null)
        .Where(methods => methods.GetComponent<Output>() != null)
        .Select(methods => methods.GetComponent<Output>()).ToList();
    /// <summary>
    /// オブジェクトのリストから特定コンポーネントのリストへの変換
    /// </summary>
    public static List<Output> ToComponents<Output>(this List<Methods> originList)
        where Output : Methods
        => ToComponents<Output, Methods>(originList);

    /// <summary>
    /// パーツのリストから特定コンポーネントのリストへの変換
    /// </summary>
    public static List<Output> ToComponents<Output>(this List<Parts> originList)
        where Output : Parts
        => ToComponents<Output, Parts>(originList);

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
    public static Result SelectRandom<Result>(this IEnumerable<Result> values, IEnumerable<int> rates = null)
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
            });
        var selection = Random.Range(0, rates.Sum(rate => Mathf.Max(rate, 0)));
        var results = rateValues
            .Where(rateValue => rateValue.rate >= 0)
            .Where(rateValue => rateValue.sumRate > selection)
            .Where(rateValue => rateValue.sumRate - rateValue.rate <= selection)
            .Select(rateValue => rateValue.value);
        return results.Single();
    }

    /// <summary>
    /// 数値の上昇度合いを抑える補正関数
    /// </summary>
    /// <param name="origin">元値</param>
    /// <param name="_baseNumber">補正の底数</param>
    /// <returns>補正後の数値</returns>
    public static float Log(this float origin, float? _baseNumber = null)
    {
        var baseNumber = _baseNumber ?? Mathf.Exp(1);
        return Mathf.Log(Mathf.Abs(origin) + 1, baseNumber) * origin.ToSign();
    }

    /// <summary>
    /// 帯状のエフェクトを指定座標中心に配置する
    /// </summary>
    /// <param name="effect">表示するエフェクト</param>
    /// <param name="central">表示座標中心</param>
    /// <param name="size">エフェクトのサイズ指定</param>
    /// <param name="orign"></param>
    /// <returns></returns>
    public static List<Effect> SetStrip(this List<Effect> orign, Effect effect, Vector2 central, float size = 1)
    {
        var effectList = orign ?? new List<Effect>();
        var width = effect.spriteSize.y * size * 2;
        var total = Mathf.CeilToInt(Methods.viewSize.y / 2 / width + 1) * 2 + 1;

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
    /// <summary>
    /// リストの値コピー
    /// </summary>
    /// <typeparam name="Type">コピー対象リストの要素タイプ</typeparam>
    /// <param name="origin">コピー対象リスト</param>
    /// <returns>コピー後のリスト</returns>
    public static List<Type> Copy<Type>(this List<Type> origin) => origin?.Select(value => value).ToList();

    /// <summary>
    /// nullableのfloat値を所定の表記（デフォルトで百分率）に直す
    /// </summary>
    /// <param name="origin">元の数値</param>
    /// <param name="format">表記方法指定</param>
    /// <returns>所定の表記（デフォルトで百分率）の文字列</returns>
    public static string ToPercentage(this float? origin, string format = "F2") => origin?.ToString(format) ?? "-";

    /// <summary>
    /// まろやかな乱数を生成する
    /// </summary>
    /// <param name="range">乱数範囲</param>
    /// <param name="center">中央値</param>
    /// <param name="centering">まろやか度合</param>
    /// <returns>まろやかな乱数</returns>
    public static float ToMildRandom(this float range, float center = 0, int centering = 5)
    {
        float sum = 0;
        for(int count = 0; count < centering; count++) sum += Random.Range(-range, range);
        return center + sum;
    }
}
