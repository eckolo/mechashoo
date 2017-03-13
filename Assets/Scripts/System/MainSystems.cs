using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;

public partial class MainSystems : Stage
{
    /// <summary>
    ///ステージリスト
    /// </summary>
    public List<Stage> stages = new List<Stage>();
    /// <summary>
    ///メインメニュー
    /// </summary>
    public Stage mainMenu = null;
    /// <summary>
    ///次のステージ番号
    /// </summary>
    [NonSerialized]
    public Stage nextStage = null;
    /// <summary>
    ///現在のステージオブジェクト
    /// </summary>
    [NonSerialized]
    public Stage nowStage = null;

    /// <summary>
    ///各種Player用バーオブジェクト
    /// </summary>
    [NonSerialized]
    public Bar playerHPbar = null;
    [NonSerialized]
    public Bar playerBRbar = null;
    [NonSerialized]
    public Bar playerENbar = null;

    /// <summary>
    ///オープニング再生済みフラグ
    /// </summary>
    private bool opening = false;
    /// <summary>
    ///メインテキストオブジェクト
    /// </summary>
    Text mainText = null;
    /// <summary>
    ///FPS表記
    /// </summary>
    Text fpsText = null;

    private IEnumerator FPScounter = null;

    public List<Ship.CoreData> shipDataMylist = new List<Ship.CoreData>();
    public Ship.CoreData adoptedShipData = null;
    public List<Weapon> possessionWeapons = new List<Weapon>();
    public List<Ship> possessionShips = new List<Ship>();

    private Dictionary<string, bool> clearData = new Dictionary<string, bool>();
    public bool getClearFlug(string stageName)
    {
        if(!clearData.ContainsKey(stageName)) return false;
        return clearData[stageName];
    }
    public bool getClearFlug(Stage stage)
    {
        return getClearFlug(stage.displayName);
    }

    // Use this for initialization
    public override void Start()
    {
        switchPause(false);
        StartCoroutine(systemStart());
    }
    public IEnumerator systemStart()
    {
        setScenery();
        Screen.SetResolution(1280, 720, Screen.fullScreen);
        if(FPScounter != null) StopCoroutine(FPScounter);

        yield return wait(1, isSystem: true);
        while(!opening) yield return openingAction();
        setBGM();

        setup();

        yield return setStage();

        nowStage.startStageProcess();

        yield break;
    }
    private void setup()
    {
        setScenery();
        Application.targetFrameRate = 120;
        flamecount = 0;
    }

    // Update is called once per frame
    public override void Update()
    {
        flamecount++;
    }
    int flamecount = 0;

    /// <summary>
    ///メインウィンドウの文字表示間隔
    /// </summary>
    public int mainWindowInterval = 10;
    /// <summary>
    ///メインウィンドウの動作状態
    /// </summary>
    public IEnumerator textMotion = null;
    /// <summary>
    ///メインウィンドウへのテキスト設定
    /// </summary>
    public void setMainWindow(string setedText, KeyCode? interruption = null, int size = Configs.DEFAULT_TEXT_SIZE)
    {
        if(textMotion != null) StopCoroutine(textMotion);
        StartCoroutine(setMainWindow(setedText, mainWindowInterval, interruption, size));
    }
    /// <summary>
    ///メインウィンドウへのテキスト設定
    ///イテレータ使用版
    /// </summary>
    public IEnumerator setMainWindow(string setedText, int interval, KeyCode? interruption = null, int size = Configs.DEFAULT_TEXT_SIZE)
    {
        if(setedText != "")
        {
            textMotion = setMainWindowMotion(setedText, interval, interruption, size);
        }
        else
        {
            textMotion = deleteMainWindowMotion(interval);
        }
        yield return textMotion;
        yield break;
    }
    private IEnumerator setMainWindowMotion(string setedText, int interval, KeyCode? interruption = null, int size = Configs.DEFAULT_TEXT_SIZE)
    {
        Vector2 mainWindowPosition = new Vector2(-1, 1).scaling(screenSize / 2);
        var interruptions = new List<KeyCode>
                {
                    KeyCode.KeypadEnter,
                    KeyCode.Space
                };
        if(interruption != null) interruptions.Add((KeyCode)interruption);

        for(int charNum = 1; charNum <= setedText.Length; charNum++)
        {
            string nowText = setedText.Substring(0, charNum);

            mainText = setSysText(nowText, mainWindowPosition, TextAnchor.UpperLeft, charSize: size, defaultText: mainText);
            if(charNum % 12 == 0) soundSE(escapementSE, 0.3f, 1.2f);

            if(interval > 0)
            {
                yield return wait(interval, interruptions);
                if(nowText.Substring(nowText.Length - 1, 1) == " ")
                    yield return wait(interval * 6, interruptions);
            }
            if(onKeysDecision(interruptions)) yield break;
        }
        yield break;
    }
    private IEnumerator deleteMainWindowMotion(int interval)
    {
        deleteSysText(mainText);
        yield break;
    }

    IEnumerator countFPS()
    {
        if(!Debug.isDebugBuild) yield break;
        while(true)
        {
            yield return new WaitForSeconds(1);
            fpsText = setSysText($"fps:{flamecount}:{1 / Time.deltaTime}", -screenSize / 2, TextAnchor.LowerLeft, 12, TextAnchor.LowerLeft, fpsText);
            flamecount = 0;
        }
    }

    protected override IEnumerator openingAction()
    {
        setScenery();
        yield return setMainWindow("Jugemu, Mu Kotobukigen\r\nFrayed five-ko\r\nOf sea gravel Suigyo\r\nWater end-of-line Unrai end Kazeraimatsu\r\nPunished by living in the treatment of sleep eat\r\nYabura forceps of bush forceps\r\nShoe phosphorus cancer Paipopaipo Paipo\r\nGurindai of shoe phosphorus cancer\r\nOf Ponpoko copy of Gurindai of Ponpokona\r\nOf Nagahisa life Chosuke", mainWindowInterval, Configs.Buttom.Z, size: 18);

        yield return wait(120);
        yield return setMainWindow("", mainWindowInterval);

        opening = true;
        yield break;
    }

    IEnumerator setStage()
    {
        if(stages.Count <= 0) yield break;

        StartCoroutine(FPScounter = countFPS());
        nowStage = Instantiate(nextStage ?? mainMenu, Vector2.zero, transform.rotation);
        nowStage.nowParent = transform;
        nextStage = null;

        nowStage.resetView();

        yield break;
    }
}
