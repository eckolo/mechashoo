using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public partial class MainSystems : Stage
{
    /// <summary>
    /// 雛形オブジェクトリスト
    /// </summary>
    [Serializable]
    public class BaseObjects
    {
        /// <summary>
        /// HPバーオブジェクトの雛形
        /// </summary>
        public Bar basicBar = null;
        /// <summary>
        /// テキストオブジェクトの雛形
        /// </summary>
        public Text basicText = null;
        /// <summary>
        /// ウィンドウオブジェクトの雛形
        /// </summary>
        public Window basicWindow = null;
        /// <summary>
        /// キャンバスオブジェクトの雛形
        /// </summary>
        public Canvas basicCanvas = null;
        /// <summary>
        /// パネルオブジェクトの雛形
        /// </summary>
        public Panel basicPanel = null;
        /// <summary>
        /// 暗調オブジェクトの雛形
        /// </summary>
        public Window basicDarkTone = null;
        /// <summary>
        /// 色調オブジェクトの雛形
        /// </summary>
        public Window colorTone = null;
        /// <summary>
        /// 減算オブジェクトの雛形
        /// </summary>
        public Window fadeTone = null;
        /// <summary>
        /// 照準オブジェクト雛形
        /// </summary>
        public Effect baseAlignmentSprite = null;
        /// <summary>
        /// 危険察知用NPC照準オブジェクト雛形
        /// </summary>
        public Effect baseAlertAlignmentSprite = null;

        /// <summary>
        /// SEオブジェクトの雛形
        /// </summary>
        public SEroot SErootObject = null;
        /// <summary>
        /// BGMオブジェクトの雛形
        /// </summary>
        public BGMroot BGMrootObject = null;

        /// <summary>
        /// 初期配置用プレイヤーPrefab
        /// </summary>
        public Player initialPlayer;

        /// <summary>
        /// 警告表示用エフェクト
        /// </summary>
        public Effect warningEffect;
    }
    [SerializeField]
    public BaseObjects baseObjects = new BaseObjects();
}
