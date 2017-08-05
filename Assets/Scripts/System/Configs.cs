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
    public static AimingOperationOption AimingMethod = AIMING_METHOD_DEAULT;
    public const AimingOperationOption AIMING_METHOD_DEAULT = AimingOperationOption.WSAD;
    public static bool aimingWsad
    {
        get {
            if(AimingMethod == AimingOperationOption.WSAD) return true;
            if(AimingMethod == AimingOperationOption.COMBINED) return true;
            return false;
        }
    }
    public static bool aimingShift
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
        public static KeyCode Key1 = KeyCode.Z;
        /// <summary>
        /// ボタン2
        /// </summary>
        public static KeyCode Key2 = KeyCode.X;
        /// <summary>
        /// ボタン3
        /// </summary>
        public static KeyCode Key3 = KeyCode.C;
        /// <summary>
        /// サブボタン
        /// </summary>
        public static KeyCode Sub = KeyCode.LeftShift;
        /// <summary>
        /// 特殊動作ボタン
        /// </summary>
        public static KeyCode Sink = KeyCode.Space;
        /// <summary>
        /// ポーズボタン
        /// </summary>
        public static KeyCode Menu = KeyCode.Escape;
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
        public static KeyCode SubUp = KeyCode.W;
        /// <summary>
        /// サブ↓ボタン
        /// </summary>
        public static KeyCode SubDown = KeyCode.S;
        /// <summary>
        /// サブ←ボタン
        /// </summary>
        public static KeyCode SubLeft = KeyCode.A;
        /// <summary>
        /// サブ→ボタン
        /// </summary>
        public static KeyCode SubRight = KeyCode.D;
    }

    /// <summary>
    /// 音量関連のパラメータ
    /// </summary>
    public struct Volume
    {
        /// <summary>
        /// BGM音量
        /// </summary>
        public static float bgm = BGM_DEFAULT;
        /// <summary>
        /// BGM音量基礎値
        /// </summary>
        public const float BASE_BGM = 0.01f;
        /// <summary>
        /// BGM音量初期値
        /// </summary>
        public const float BGM_DEFAULT = 50;
        /// <summary>
        /// SE音量
        /// </summary>
        public static float se = SE_DEFAULT;
        /// <summary>
        /// SE音量基礎値
        /// </summary>
        public const float BASE_SE = 0.01f;
        /// <summary>
        /// SE音量初期値
        /// </summary>
        public const float SE_DEFAULT = 50;

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
    /// システムテキスト関連のパラメータ
    /// </summary>
    public struct Texts
    {
        /// <summary>
        /// システムテキストのデフォルト文字サイズ
        /// </summary>
        public const int CHAR_SIZE = 13;
        /// <summary>
        /// システムテキストのデフォルト行間幅
        /// </summary>
        public const float LINE_SPACE = 0.1f;
    }

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
        /// <summary>
        /// メインウィンドウの文字表示間隔
        /// </summary>
        public const int MAIN_WINDOW_INTERVAL = 10;
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
        /// 色調
        /// </summary>
        public const string TONE = "DARKTONE";
    }

    /// <summary>
    /// レイヤー名一覧
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

    public struct SaveKeys
    {
        /// <summary>
        /// ストーリー進行度合い
        /// </summary>
        public const string STORY_PHASE = "storyPhase";
        /// <summary>
        /// 機体データ
        /// </summary>
        public const string ADOPTED_SHIP_DATA = "adoptedShipData";
        /// <summary>
        /// 設計図一式
        /// </summary>
        public const string SHIP_DATA_MYLIST = "shipDataMylist";
        /// <summary>
        /// 所持武装
        /// </summary>
        public const string POSSESSION_WEAPONS = "possessionWeapons";
        /// <summary>
        /// 所持機体
        /// </summary>
        public const string POSSESSION_SHIPS = "possessionShips";
        /// <summary>
        /// BGM音量
        /// </summary>
        public const string BGM_VOLUME = "bgmVolume";
        /// <summary>
        /// 効果音音量
        /// </summary>
        public const string SE_VOLUME = "seVolume";
        /// <summary>
        /// 照準操作方式
        /// </summary>
        public const string AIMING_METHOD = "AimingMethod";
    }
}
