using UnityEngine;
using System.Collections;


public partial class Methods : MonoBehaviour
{
    /// <summary>
    ///ボタン1
    /// </summary>
    protected static KeyCode ButtomZ = KeyCode.Z;
    /// <summary>
    ///ボタン2
    /// </summary>
    protected static KeyCode ButtomX = KeyCode.X;
    /// <summary>
    ///ボタン3
    /// </summary>
    protected static KeyCode ButtomC = KeyCode.C;
    /// <summary>
    ///ボタン4
    /// </summary>
    protected static KeyCode ButtomA = KeyCode.A;
    /// <summary>
    ///ボタン5
    /// </summary>
    protected static KeyCode ButtomS = KeyCode.S;
    /// <summary>
    ///ボタン6
    /// </summary>
    protected static KeyCode ButtomD = KeyCode.D;
    /// <summary>
    ///サブボタン
    /// </summary>
    protected static KeyCode ButtomSub = KeyCode.LeftShift;
    /// <summary>
    ///ポーズボタン
    /// </summary>
    protected static KeyCode ButtomEsc = KeyCode.Escape;
    /// <summary>
    ///↑ボタン
    /// </summary>
    protected static KeyCode ButtomUp = KeyCode.UpArrow;
    /// <summary>
    ///↓ボタン
    /// </summary>
    protected static KeyCode ButtomDown = KeyCode.DownArrow;
    /// <summary>
    ///←ボタン
    /// </summary>
    protected static KeyCode ButtomLeft = KeyCode.LeftArrow;
    /// <summary>
    ///→ボタン
    /// </summary>
    protected static KeyCode ButtomRight = KeyCode.RightArrow;

    /// <summary>
    ///BGM音量
    /// </summary>
    protected static float volumeBGM = 50;
    protected const float baseVolumeBGM = 0.003f;
    /// <summary>
    ///SE音量
    /// </summary>
    protected static float volumeSE = 50;
    protected const float baseVolumeSE = 0.001f;

    protected const float maxVolume = 100;
    protected const float minVolume = 0;

    /// <summary>
    ///システムテキストのデフォルト文字サイズ
    /// </summary>
    protected const int defaultTextSize = 12;

    /// <summary>
    ///選択肢ウィンドウアニメーション時間
    /// </summary>
    protected const int choiceWindowMotionTime = 48;

    /// <summary>
    ///表示レイヤー一覧
    /// </summary>
    protected static class Order
    {
        public const int ship = 10;
        public const int systemState = 110;
        public const int publicState = 80;
        public const int darkTone = 100;
    }
}
