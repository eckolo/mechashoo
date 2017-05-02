﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// キー入力系動作の統括
/// </summary>
public static class Keies
{
    /// <summary>
    /// 現在の外部入力可否
    /// </summary>
    public static bool recievable { get; set; } = true;

    public enum KeyTiming { DOWN, ON, UP }

    /// <summary>
    /// 複数キーのOR押下判定
    /// </summary>
    /// <param name="keys">判定対象キー一覧</param>
    /// <param name="timing">判定対象タイミング</param>
    /// <returns>判定結果</returns>
    public static bool decision(this List<KeyCode> keys, KeyTiming timing = KeyTiming.ON)
    {
        if(keys == null) return false;
        if(keys.Count <= 0) return false;
        return keys.Any(key => key.decision(timing));
    }
    /// <summary>
    /// 単数キーの押下判定
    /// </summary>
    /// <param name="key">判定対象キー</param>
    /// <param name="timing">判定対象タイミング</param>
    /// <returns>判定結果</returns>
    public static bool decision(this KeyCode key, KeyTiming timing)
    {
        switch(timing)
        {
            case KeyTiming.DOWN:
                return Input.GetKeyDown(key);
            case KeyTiming.ON:
                return Input.GetKey(key);
            case KeyTiming.UP:
                return Input.GetKeyUp(key);
            default:
                return false;
        }
    }
}
