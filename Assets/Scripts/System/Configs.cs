using UnityEngine;
using System.Collections;


/// <summary>
/// システム設定系
/// </summary>
public struct Configs
{
    /// <summary>
    /// 照準操作方法
    /// </summary>
    public static AimingOperationOption AimingMethod = AimingOperationOption.WSAD;
    public static bool AimingWsad
    {
        get {
            if(AimingMethod == AimingOperationOption.WSAD) return true;
            if(AimingMethod == AimingOperationOption.COMBINED) return true;
            return false;
        }
    }
    public static bool AimingShift
    {
        get {
            if(AimingMethod == AimingOperationOption.SHIFT) return true;
            if(AimingMethod == AimingOperationOption.COMBINED) return true;
            return false;
        }
    }
    public enum AimingOperationOption
    {
        /// <summary>
        /// WSADで操作
        /// </summary>
        WSAD,
        /// <summary>
        /// サブキー押下時に十字キーで操作
        /// </summary>
        SHIFT,
        /// <summary>
        /// 両方法併用
        /// </summary>
        COMBINED
    }
    /// <summary>
    /// キーコンフィグ対応用可変ボタンコード
    /// </summary>
    public struct Buttom
    {
        /// <summary>
        /// ボタン1
        /// </summary>
        public static KeyCode Z = KeyCode.Z;
        /// <summary>
        /// ボタン2
        /// </summary>
        public static KeyCode X = KeyCode.X;
        /// <summary>
        /// ボタン3
        /// </summary>
        public static KeyCode C = KeyCode.C;
        /// <summary>
        /// サブボタン
        /// </summary>
        public static KeyCode Sub = KeyCode.LeftShift;
        /// <summary>
        /// ポーズボタン
        /// </summary>
        public static KeyCode Esc = KeyCode.Escape;
        /// <summary>
        /// ↑ボタン
        /// </summary>
        public static KeyCode Up = KeyCode.UpArrow;
        /// <summary>
        /// ↓ボタン
        /// </summary>
        public static KeyCode Down = KeyCode.DownArrow;
        /// <summary>
        /// ←ボタン
        /// </summary>
        public static KeyCode Left = KeyCode.LeftArrow;
        /// <summary>
        /// →ボタン
        /// </summary>
        public static KeyCode Right = KeyCode.RightArrow;
        /// <summary>
        /// サブ↑ボタン
        /// </summary>
        public static KeyCode W = KeyCode.W;
        /// <summary>
        /// サブ↓ボタン
        /// </summary>
        public static KeyCode S = KeyCode.S;
        /// <summary>
        /// サブ←ボタン
        /// </summary>
        public static KeyCode A = KeyCode.A;
        /// <summary>
        /// サブ→ボタン
        /// </summary>
        public static KeyCode D = KeyCode.D;
    }

    /// <summary>
    /// 音量関連のパラメータ
    /// </summary>
    public struct Volume
    {
        /// <summary>
        /// BGM音量
        /// </summary>
        public static float bgm = 50;
        /// <summary>
        /// BGM音量基礎値
        /// </summary>
        public const float BASE_BGM = 0.003f;
        /// <summary>
        /// SE音量
        /// </summary>
        public static float se = 50;
        /// <summary>
        /// SE音量基礎値
        /// </summary>
        public const float BASE_SE = 0.003f;

        /// <summary>
        /// 最大音量
        /// </summary>
        public const float MAX = 100;
        /// <summary>
        /// 最小音量
        /// </summary>
        public const float MIN = 0;
    }

    /// <summary>
    /// システムテキストのデフォルト文字サイズ
    /// </summary>
    public const int DEFAULT_TEXT_SIZE = 13;

    /// <summary>
    /// メインストーリー進行度合いの初期値
    /// </summary>
    public const uint START_STORY_PHASE = 0;

    /// <summary>
    /// フェードイン・アウトのデフォルト所要時間
    /// </summary>
    public const int DEFAULT_FADE_TIME = 108;

    /// <summary>
    /// ウィンドウ系関連のパラメータ
    /// </summary>
    public struct Window
    {
        public const int DEFAULT_MOTION_TIME = 48;
    }

    /// <summary>
    /// 選択肢系関連のパラメータ
    /// </summary>
    public struct Choice
    {
        /// <summary>
        /// 上下押しっぱなしで連打判定に入るまでの猶予フレーム
        /// </summary>
        public const int KEEP_VERTICAL_LIMIT = 60;
        /// <summary>
        /// 連打判定時の連打間隔フレーム数
        /// </summary>
        public const int KEEP_VERTICAL_INTERVAL = 12;
        /// <summary>
        /// 選択肢ウィンドウアニメーション時間
        /// </summary>
        public const int WINDOW_MOTION_TIME = 48;
        /// <summary>
        /// 選択肢の決定操作時のSE音量補正値
        /// </summary>
        public const float DECISION_SE_VORUME = 0.8f;
        /// <summary>
        /// 選択肢のキャンセル操作時のSE音量補正値
        /// </summary>
        public const float CANCEL_SE_VORUME = 0.8f;
        /// <summary>
        /// 選択肢の選択操作時のSE音量補正値
        /// </summary>
        public const float SETECTING_SE_VORUME = 0.8f;
        /// <summary>
        /// メインメニューの項目最大数
        /// </summary>
        public const int MAX_MENU_CHOICE = 10;
    }

    /// <summary>
    /// 表示順レイヤー名一覧
    /// </summary>
    public struct SortLayers
    {
        /// <summary>
        /// デフォルトレイヤー
        /// </summary>
        public const string DEFAULT = "Default";
        /// <summary>
        /// 物理物体レイヤー
        /// </summary>
        public const string PHYSICAL = "PHYSICAL";
        /// <summary>
        /// ポーズ制御外の画面表示系
        /// </summary>
        public const string SYSTEM_STATE = "SYSTEM_STATE";
        /// <summary>
        /// ポーズ制御下の画面表示系
        /// </summary>
        public const string PUBLIC_STATE = "PUBLIC_STATE";
        /// <summary>
        /// 暗調
        /// </summary>
        public const string DARKTONE = "DARKTONE";
    }

    /// <summary>
    ///レイヤー名一覧
    /// </summary>
    public struct Layers
    {
        /// <summary>
        /// デフォルトレイヤー
        /// </summary>
        public const string DEFAULT = "Default";
        /// <summary>
        /// プレイヤー勢力レイヤー
        /// </summary>
        public const string PLAYER = "Player";
        /// <summary>
        /// エネミー勢力レイヤー
        /// </summary>
        public const string ENEMY = "Enemy";
        /// <summary>
        /// 中立勢力レイヤー
        /// </summary>
        public const string NEUTRAL = "Neutral";
    }
}
