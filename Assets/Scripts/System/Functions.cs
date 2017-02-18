using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public static class Functions
{
    /// <summary>
    ///boolを整数0,1に変換
    /// </summary>
    public static int toInt(this bool value) => value ? 1 : 0;
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

    public static int getLines(this string text) => text.ToList().Where(character => character.Equals("\r\n")).Count() + 1;

    /// <summary>
    ///ボタンの入力状態を整数0,1に変換
    /// </summary>
    public static int toInt(this KeyCode buttom) => toInt(Input.GetKey(buttom));

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
    /// 等値判定実装してるクラスのリスト同士の等値判定
    /// </summary>
    public static bool listEquals<Type>(this List<Type> originList, List<Type> otherList)
        where Type : System.IEquatable<Type>
    {
        if(originList == null && otherList != null) return false;
        if(originList != null && otherList == null) return false;
        if(originList == null && otherList == null) return true;

        if(originList.Count != otherList.Count) return false;
        for(int index = 0; index < originList.Count; index++)
        {
            if(!originList[index].Equals(otherList[index])) return false;
        }

        return true;
    }

    /// <summary>
    /// 選択肢と選択可能性からランダムで選択肢を選び出す
    /// </summary>
    public static Result selectRandom<Result>(this IEnumerable<Result> values, IEnumerable<int> rates)
    {
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
}
