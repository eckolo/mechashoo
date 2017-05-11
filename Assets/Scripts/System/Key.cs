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

    /// <summary>
    /// キーリストに指定キーが含まれるか否か判定
    /// </summary>
    /// <param name="judgedKey">指定のキー</param>
    /// <param name="keys">含まれる判定先キーリスト</param>
    /// <returns>含まれているか否か</returns>
    public static bool judge(this KeyCode judgedKey, List<KeyCode> keys) => keys.Contains(judgedKey);
    /// <summary>
    /// キーリストに指定キーが含まれるか否か判定
    /// </summary>
    /// <param name="judgedKey">指定のキー</param>
    /// <param name="keys">含まれる判定先キーリスト</param>
    /// <returns>含まれているか否か</returns>
    public static bool judge(this KeyCode? judgedKey, List<KeyCode> keys) => judgedKey?.judge(keys) ?? false;

    /// <summary>
    /// 用途別キーセット
    /// </summary>
    public static class Set
    {
        /// <summary>
        /// 決定キーセット
        /// </summary>
        public static List<KeyCode> decide => new List<KeyCode>
        {
            Configs.Buttom.Key1,
            KeyCode.Return,
            KeyCode.Space
        };
        /// <summary>
        /// キャンセルキーセット
        /// </summary>
        public static List<KeyCode> cancel => new List<KeyCode>
        {
            Configs.Buttom.Key2,
            Configs.Buttom.Menu
        };
        /// <summary>
        /// ↑キーセット
        /// </summary>
        public static List<KeyCode> up => new List<KeyCode>
        {
            Configs.Buttom.Up,
            Configs.Buttom.SubUp
        };
        /// <summary>
        /// ↓キーセット
        /// </summary>
        public static List<KeyCode> down => new List<KeyCode>
        {
            Configs.Buttom.Down,
            Configs.Buttom.SubDown
        };
        /// <summary>
        /// 上下キーセット
        /// </summary>
        public static List<KeyCode> vertical => up.Concat(down).ToList();
        /// <summary>
        /// ←キーセット
        /// </summary>
        public static List<KeyCode> left => new List<KeyCode>
        {
            Configs.Buttom.Left,
            Configs.Buttom.SubLeft
        };
        /// <summary>
        /// →キーセット
        /// </summary>
        public static List<KeyCode> right => new List<KeyCode>
        {
            Configs.Buttom.Right,
            Configs.Buttom.SubRight
        };
        /// <summary>
        /// 左右キーセット
        /// </summary>
        public static List<KeyCode> horizontal => left.Concat(right).ToList();
    }
}

