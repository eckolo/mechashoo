using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public partial class MainSystems : Stage
{
    /// <summary>
    ///HPバーオブジェクトの雛形
    /// </summary>
    public Bar basicBar = null;
    /// <summary>
    ///テキストオブジェクトの雛形
    /// </summary>
    public Text basicText = null;
    /// <summary>
    ///ウィンドウオブジェクトの雛形
    /// </summary>
    public Window basicWindow = null;
    /// <summary>
    ///キャンバスオブジェクトの雛形
    /// </summary>
    public Canvas basicCanvas = null;
    /// <summary>
    ///パネルオブジェクトの雛形
    /// </summary>
    public Panel basicPanel = null;
    /// <summary>
    ///暗調オブジェクトの雛形
    /// </summary>
    public Window basicDarkTone = null;

    /// <summary>
    ///SEオブジェクトの雛形
    /// </summary>
    public SEroot SErootObject = null;
    /// <summary>
    ///BGMオブジェクトの雛形
    /// </summary>
    public BGMroot BGMrootObject = null;

    /// <summary>
    ///初期配置用プレイヤーPrefab
    /// </summary>
    public Player initialPlayer;
}
