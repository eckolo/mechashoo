using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;

public class MainSystems : Stage
{
    /// <summary>
    ///ステージリスト
    /// </summary>
    public List<Stage> stages = new List<Stage>();
    /// <summary>
    ///次のステージ番号
    /// </summary>
    public int nextStageNum = 0;
    /// <summary>
    ///現在のステージオブジェクト
    /// </summary>
    public Stage nowStage = null;

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
    ///SEオブジェクトの雛形
    /// </summary>
    public SEroot SErootObject = null;
    /// <summary>
    ///SEオブジェクトの雛形
    /// </summary>
    public BGMroot BGMrootObject = null;

    /// <summary>
    ///文字送りSE
    /// </summary>
    public AudioClip escapementSE = null;

    /// <summary>
    ///オープニング再生済みフラグ
    /// </summary>
    [SerializeField]
    private bool opening = false;

    /// <summary>
    ///初期配置用プレイヤーPrefab
    /// </summary>
    public Player initialPlayer;

    /// <summary>
    ///各種Player用バーオブジェクト
    /// </summary>
    public Bar playerHPbar = null;
    public Bar playerBRbar = null;
    public Bar playerENbar = null;

    /// <summary>
    ///メインウィンドウ名称
    /// </summary>
    const string MAINTEXT = "mainWindow";
    /// <summary>
    ///FPS表記惑名称
    /// </summary>
    const string FPSTEXT = "countFPS";

    private IEnumerator FPScounter = null;

    public List<Ship.CoreData> shipDataMylist = new List<Ship.CoreData>();
    public Ship.CoreData adoptedShipData = null;
    public List<Weapon> possessionWeapons = new List<Weapon>();
    public List<Ship> possessionShips = new List<Ship>();

    private Dictionary<string, bool> clearData = new Dictionary<string, bool>();
    public bool getClearFlug(string stageName)
    {
        if (!clearData.ContainsKey(stageName)) return false;
        return clearData[stageName];
    }
    public bool getClearFlug(Stage stage)
    {
        return getClearFlug(stage.displayName);
    }

    // Use this for initialization
    public override void Start()
    {
        StartCoroutine(systemStart());
    }
    public IEnumerator systemStart()
    {
        setScenery();
        Screen.SetResolution(1024, 768, Screen.fullScreen);
        if (FPScounter != null) StopCoroutine(FPScounter);

        yield return wait(1);
        while (!opening) yield return openingAction();
        setBGM();

        setup();

        yield return setStage();

        nowStage.startStageAction();

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
    ///メインウィンドウの位置
    /// </summary>
    public Vector2 mainWindowPosition = Vector2.zero;
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
    public void setMainWindow(string setedText, KeyCode? interruption = null, int size = defaultTextSize)
    {
        if (textMotion != null) StopCoroutine(textMotion);
        StartCoroutine(setMainWindow(setedText, mainWindowInterval, interruption, size));
    }
    /// <summary>
    ///メインウィンドウへのテキスト設定
    ///イテレータ使用版
    /// </summary>
    public IEnumerator setMainWindow(string setedText, int interval, KeyCode? interruption = null, int size = defaultTextSize)
    {
        if (setedText != "")
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
    private IEnumerator setMainWindowMotion(string setedText, int interval, KeyCode? interruption = null, int size = defaultTextSize)
    {
        List<KeyCode> interruptions = new List<KeyCode>
                {
                    KeyCode.KeypadEnter,
                    KeyCode.Space
                };
        if (interruption != null) interruptions.Add((KeyCode)interruption);

        for (int charNum = 1; charNum <= setedText.Length; charNum++)
        {
            string nowText = setedText.Substring(0, charNum);

            setSysText(nowText, MAINTEXT, mainWindowPosition, size: size);
            if (charNum % 12 == 0) soundSE(escapementSE, 0.3f, 1.2f);

            if (interval > 0)
            {
                yield return wait(interval, interruptions);
                if (nowText.Substring(nowText.Length - 1, 1) == " ") yield return wait(interval * 6, interruptions);
            }
            if (onKeysDecision(interruptions)) yield break;
        }
        yield break;
    }
    private IEnumerator deleteMainWindowMotion(int interval)
    {
        deleteSysText(MAINTEXT);
        yield break;
    }

    IEnumerator countFPS()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            setSysText("fps:" + flamecount + ":" + 1 / Time.deltaTime, FPSTEXT, Vector2.zero, 12, TextAnchor.LowerLeft);
            flamecount = 0;
        }
    }

    IEnumerator openingAction()
    {
        setScenery();
        yield return setMainWindow("Jugemu, Mu Kotobukigen\r\nFrayed five-ko\r\nOf sea gravel Suigyo\r\nWater end-of-line Unrai end Kazeraimatsu\r\nPunished by living in the treatment of sleep eat\r\nYabura forceps of bush forceps\r\nShoe phosphorus cancer Paipopaipo Paipo\r\nGurindai of shoe phosphorus cancer\r\nOf Ponpoko copy of Gurindai of Ponpokona\r\nOf Nagahisa life Chosuke", mainWindowInterval, ButtomZ, size: 18);

        yield return wait(120);
        yield return setMainWindow("", mainWindowInterval);

        opening = true;
        yield break;
    }

    IEnumerator setStage()
    {
        if (stages.Count <= 0) yield break;

        StartCoroutine(FPScounter = countFPS());
        nowStage = (Stage)Instantiate(stages[nextStageNum], Vector2.zero, transform.rotation);
        nowStage.transform.parent = transform;

        nowStage.resetView();

        yield break;
    }
}
