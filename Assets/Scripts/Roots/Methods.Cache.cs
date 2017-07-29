using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract partial class Methods : MonoBehaviour
{
    /// <summary>
    /// メインシステム記憶キャッシュ
    /// </summary>
    private static MainSystems systemRoot = null;
    /// <summary>
    /// メインシステムオブジェクト取得関数
    /// </summary>
    static protected MainSystems sys
    {
        get {
            return systemRoot = systemRoot ?? GameObject.Find("SystemRoot").GetComponent<MainSystems>();
        }
    }

    /// <summary>
    /// プレイヤー記憶キャッシュ
    /// </summary>
    private static Player _sysPlayer = null;
    /// <summary>
    /// プレイヤーオブジェクト取得関数
    /// </summary>
    static protected Player sysPlayer
    {
        get {
            if(_sysPlayer == null)
            {
                VisualizePlayer();
                TransparentPlayer();
            }
            return _sysPlayer;
        }
    }
    /// <summary>
    /// プレイヤーオブジェクト設置関数
    /// </summary>
    static protected Player VisualizePlayer()
    {
        if(_sysPlayer == null)
        {
            _sysPlayer = Instantiate(sys.baseObjects.initialPlayer);
            _sysPlayer.nowParent = sysPanel.transform;
            _sysPlayer.coreData = null;
        }
        _sysPlayer.coreData = sys.adoptedShipData;
        IndicatePlayer();
        return _sysPlayer;
    }
    /// <summary>
    /// プレイヤーオブジェクト透明化解除関数
    /// </summary>
    static protected Player IndicatePlayer()
    {
        _sysPlayer.gameObject.SetActive(true);
        _sysPlayer.transform.localScale = Vector3.one;
        _sysPlayer.nowLayer = Configs.Layers.PLAYER;
        _sysPlayer.Start();

        Destroy(Camera.main.gameObject.GetComponent<AudioListener>());
        _sysPlayer.nextDestroy = false;
        return _sysPlayer;
    }
    /// <summary>
    /// プレイヤーオブジェクト消去（透明化）関数
    /// </summary>
    static protected Player TransparentPlayer()
    {
        _sysPlayer.Start();
        _sysPlayer.StopMoving();
        _sysPlayer.StopAllWeapon();
        _sysPlayer.PickupSoundObject();
        _sysPlayer.canRecieveKey = false;
        _sysPlayer.gameObject.SetActive(false);

        if(Camera.main.GetComponent<AudioListener>() == null) Camera.main.gameObject.AddComponent<AudioListener>();
        return _sysPlayer;
    }

    /// <summary>
    /// パネルオブジェクト名
    /// </summary>
    protected static string panelName = "Panel";
    /// <summary>
    /// パネル記憶キャッシュ
    /// </summary>
    private static Panel nowPanel = null;
    /// <summary>
    /// パネルオブジェクト取得関数
    /// </summary>
    static public Panel sysPanel
    {
        get {
            if(nowPanel != null) return nowPanel;

            nowPanel = GameObject.Find(panelName) != null
                ? GameObject.Find(panelName).GetComponent<Panel>()
                : null;
            if(nowPanel != null) return nowPanel;

            nowPanel = Instantiate(sys.baseObjects.basicPanel);
            nowPanel.name = panelName;
            return nowPanel;
        }
    }

    /// <summary>
    /// ビューオブジェクト名
    /// </summary>
    protected static string ViewName = "View";
    /// <summary>
    /// ビュー記憶キャッシュ
    /// </summary>
    private static Panel nowView = null;
    /// <summary>
    /// ビューオブジェクト取得関数
    /// </summary>
    static public Panel sysView
    {
        get {
            if(nowView != null) return nowView;

            nowView = GameObject.Find(ViewName) != null
                ? GameObject.Find(ViewName).GetComponent<Panel>()
                : null;
            if(nowView != null) return nowView;

            nowView = Instantiate(sys.baseObjects.basicPanel);
            nowView.name = ViewName;
            return nowView;
        }
    }

    /// <summary>
    /// キャンバスオブジェクト名
    /// </summary>
    protected static string canvasName = "Canvas";
    /// <summary>
    /// キャンバス記憶キャッシュ
    /// </summary>
    private static Canvas nowCanvas = null;
    /// <summary>
    /// キャンバスオブジェクト取得関数
    /// </summary>
    static protected Canvas sysCanvas
    {
        get {
            if(nowCanvas != null) return nowCanvas;

            nowCanvas = GameObject.Find(canvasName) != null
                ? GameObject.Find(canvasName).GetComponent<Canvas>()
                : null;
            if(nowCanvas != null) return nowCanvas;

            nowCanvas = Instantiate(sys.baseObjects.basicCanvas);
            nowCanvas.name = canvasName;
            return nowCanvas;
        }
    }

    /// <summary>
    /// Bar取得関数
    /// </summary>
    protected Bar GetBar(BarType barName, Color? setColor = null)
    {
        Bar barObject = GameObject.Find(barName.ToString()) != null
            ? GameObject.Find(barName.ToString()).GetComponent<Bar>()
            : null;
        if(barObject != null) return barObject;

        barObject = Instantiate(sys.baseObjects.basicBar);
        barObject.nowParent = sysView.transform;
        barObject.name = barName.ToString();
        barObject.nowColor = setColor ?? Color.red;
        return barObject;
    }
    protected enum BarType
    {
        HP, BR, EN
    }
}
