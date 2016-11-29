using UnityEngine;
using System.Collections;


public partial class Methods : MonoBehaviour {
    /// <summary>
    ///キーコンフィグ対応用可変ボタンコード
    /// </summary>
    protected static class Buttom {
        /// <summary>
        ///ボタン1
        /// </summary>
        public static KeyCode Z = KeyCode.Z;
        /// <summary>
        ///ボタン2
        /// </summary>
        public static KeyCode X = KeyCode.X;
        /// <summary>
        ///ボタン3
        /// </summary>
        public static KeyCode C = KeyCode.C;
        /// <summary>
        ///ボタン4
        /// </summary>
        public static KeyCode A = KeyCode.A;
        /// <summary>
        ///ボタン5
        /// </summary>
        public static KeyCode S = KeyCode.S;
        /// <summary>
        ///ボタン6
        /// </summary>
        public static KeyCode D = KeyCode.D;
        /// <summary>
        ///サブボタン
        /// </summary>
        public static KeyCode Sub = KeyCode.LeftShift;
        /// <summary>
        ///ポーズボタン
        /// </summary>
        public static KeyCode Esc = KeyCode.Escape;
        /// <summary>
        ///↑ボタン
        /// </summary>
        public static KeyCode Up = KeyCode.UpArrow;
        /// <summary>
        ///↓ボタン
        /// </summary>
        public static KeyCode Down = KeyCode.DownArrow;
        /// <summary>
        ///←ボタン
        /// </summary>
        public static KeyCode Left = KeyCode.LeftArrow;
        /// <summary>
        ///→ボタン
        /// </summary>
        public static KeyCode Right = KeyCode.RightArrow;
    }

    /// <summary>
    ///音量関連のパラメータ
    /// </summary>
    protected static class Volume {
        /// <summary>
        ///BGM音量
        /// </summary>
        public static float bgm = 50;
        /// <summary>
        ///BGM音量基礎値
        /// </summary>
        public const float BASE_BGM = 0.003f;
        /// <summary>
        ///SE音量
        /// </summary>
        public static float se = 50;
        /// <summary>
        ///SE音量基礎値
        /// </summary>
        public const float BASE_SE = 0.003f;

        /// <summary>
        ///最大音量
        /// </summary>
        public const float MAX = 100;
        /// <summary>
        ///最小音量
        /// </summary>
        public const float MIN = 0;
    }

    /// <summary>
    ///システムテキストのデフォルト文字サイズ
    /// </summary>
    protected const int DEFAULT_TEXT_SIZE = 12;

    /// <summary>
    ///選択肢系関連のパラメータ
    /// </summary>
    protected static class Choice {
        /// <summary>
        ///選択肢ウィンドウアニメーション時間
        /// </summary>
        public const int WINDOW_MOTION_TIME = 48;
        /// <summary>
        ///選択肢の決定操作時のSE音量補正値
        /// </summary>
        public const float DECISION_SE_VORUME = 0.8f;
        /// <summary>
        ///選択肢のキャンセル操作時のSE音量補正値
        /// </summary>
        public const float CANCEL_SE_VORUME = 0.8f;
        /// <summary>
        ///選択肢の選択操作時のSE音量補正値
        /// </summary>
        public const float SETECTING_SE_VORUME = 0.8f;
    }

    /// <summary>
    ///表示レイヤー一覧
    /// </summary>
    protected static class Order {
        /// <summary>
        ///機体ベースレイヤー
        /// </summary>
        public const int SHIP = 10;
        /// <summary>
        ///ポーズ制御外の画面表示系
        /// </summary>
        public const int SYSTEM_STATE = 110;
        /// <summary>
        ///ポーズ制御下の画面表示系
        /// </summary>
        public const int PUBLIC_STATE = 80;
        /// <summary>
        ///暗調
        /// </summary>
        public const int DARKTONE = 100;
    }
}
