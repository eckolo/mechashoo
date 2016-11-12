using UnityEngine;
using System.Collections;

public partial class Methods : MonoBehaviour
{
    /// <summary>
    ///メインシステム記憶キャッシュ
    /// </summary>
    private static MainSystems systemRoot = null;
    /// <summary>
    ///メインシステムオブジェクト取得関数
    /// </summary>
    static protected MainSystems Sys
    {
        get
        {
            return systemRoot = systemRoot ?? GameObject.Find("SystemRoot").GetComponent<MainSystems>();
        }
    }

    /// <summary>
    ///プレイヤー記憶キャッシュ
    /// </summary>
    private static Player player = null;
    /// <summary>
    ///プレイヤーオブジェクト取得関数
    /// </summary>
    static protected Player sysPlayer
    {
        get
        {
            if (player == null)
            {
                visualizePlayer();
                transparentPlayer();
            }
            return player;
        }
    }
    /// <summary>
    ///プレイヤーオブジェクト設置関数
    /// </summary>
    static protected Player visualizePlayer()
    {
        if (player == null)
        {
            player = Instantiate(Sys.initialPlayer);
            player.transform.SetParent(sysPanel.transform);
            player.coreData = null;
        }
        player.coreData = Sys.adoptedShipData;
        indicatePlayer();
        return player;
    }
    /// <summary>
    ///プレイヤーオブジェクト透明化解除関数
    /// </summary>
    static protected Player indicatePlayer()
    {
        bool originalActive = player.gameObject.activeSelf;
        player.gameObject.SetActive(true);
        if (!originalActive) player.Start();
        return player;
    }
    /// <summary>
    ///プレイヤーオブジェクト消去（透明化）関数
    /// </summary>
    static protected Player transparentPlayer()
    {
        player.gameObject.SetActive(false);
        return player;
    }

    /// <summary>
    ///パネルオブジェクト名
    /// </summary>
    protected static string panelName = "Panel";
    /// <summary>
    ///パネル記憶キャッシュ
    /// </summary>
    private static Panel nowPanel = null;
    /// <summary>
    ///パネルオブジェクト取得関数
    /// </summary>
    static protected Panel sysPanel
    {
        get
        {
            if (nowPanel != null) return nowPanel;

            nowPanel = GameObject.Find(panelName) != null
                ? GameObject.Find(panelName).GetComponent<Panel>()
                : null;
            if (nowPanel != null) return nowPanel;

            nowPanel = Instantiate(Sys.basicPanel);
            nowPanel.name = panelName;
            return nowPanel;
        }
    }

    /// <summary>
    ///ビューオブジェクト名
    /// </summary>
    protected static string ViewName = "View";
    /// <summary>
    ///ビュー記憶キャッシュ
    /// </summary>
    private static Panel nowView = null;
    /// <summary>
    ///ビューオブジェクト取得関数
    /// </summary>
    static protected Panel sysView
    {
        get
        {
            if (nowView != null) return nowView;

            nowView = GameObject.Find(ViewName) != null
                ? GameObject.Find(ViewName).GetComponent<Panel>()
                : null;
            if (nowView != null) return nowView;

            nowView = Instantiate(Sys.basicPanel);
            nowView.name = ViewName;
            return nowView;
        }
    }

    /// <summary>
    ///キャンバスオブジェクト名
    /// </summary>
    protected static string canvasName = "Canvas";
    /// <summary>
    ///キャンバス記憶キャッシュ
    /// </summary>
    private static Canvas nowCanvas = null;
    /// <summary>
    ///キャンバスオブジェクト取得関数
    /// </summary>
    static protected Canvas sysCanvas
    {
        get
        {
            if (nowCanvas != null) return nowCanvas;

            nowCanvas = GameObject.Find(canvasName) != null
                ? GameObject.Find(canvasName).GetComponent<Canvas>()
                : null;
            if (nowCanvas != null) return nowCanvas;

            nowCanvas = Instantiate(Sys.basicCanvas);
            nowCanvas.name = canvasName;
            return nowCanvas;
        }
    }

    /// <summary>
    ///Bar取得関数
    /// </summary>
    protected Bar getBar(barType barName, Color? setColor = null)
    {
        Bar barObject = GameObject.Find(barName.ToString()) != null
            ? GameObject.Find(barName.ToString()).GetComponent<Bar>()
            : null;
        if (barObject != null) return barObject;

        barObject = Instantiate(Sys.basicBar);
        barObject.transform.parent = sysView.transform;
        barObject.name = barName.ToString();
        barObject.GetComponent<SpriteRenderer>().color = setColor ?? Color.red;
        return barObject;
    }
    protected enum barType
    {
        HPbar, BRbar, ENbar
    }
}
