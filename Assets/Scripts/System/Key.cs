using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// キー入力系動作の統括
/// </summary>
public static class Key
{
    /// <summary>
    /// 現在の外部入力可否
    /// </summary>
    public static bool recievable { get; set; } = true;

    public enum Timing { DOWN, ON, UP, OFF }

    /// <summary>
    /// 複数キーのOR押下判定
    /// </summary>
    /// <param name="keys">判定対象キー一覧</param>
    /// <param name="timing">判定対象タイミング</param>
    /// <param name="endProcess">一致キーに対する操作</param>
    /// <returns>判定結果</returns>
    public static bool judge(this List<KeyCode> keys, Timing timing = Timing.DOWN, UnityAction<IEnumerable<KeyCode>> endProcess = null)
    {
        if(keys == null) return false;
        if(keys.Count <= 0) return false;
        var matchKeys = keys.Where(key => key.judge(timing));
        endProcess?.Invoke(matchKeys);
        return matchKeys.Any();
    }
    /// <summary>
    /// 単数キーの押下判定
    /// </summary>
    /// <param name="key">判定対象キー</param>
    /// <param name="timing">判定対象タイミング</param>
    /// <returns>判定結果</returns>
    public static bool judge(this KeyCode key, Timing timing = Timing.DOWN)
    {
        switch(timing)
        {
            case Timing.DOWN:
                return Input.GetKeyDown(key);
            case Timing.ON:
                return Input.GetKey(key);
            case Timing.UP:
                return Input.GetKeyUp(key);
            case Timing.OFF:
                return !Input.GetKey(key) && !Input.GetKey(key) && !Input.GetKeyUp(key);
            default:
                return false;
        }
    }
}

