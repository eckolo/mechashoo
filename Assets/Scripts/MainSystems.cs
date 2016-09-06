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
    ///現在のステージ番号
    /// </summary>
    public int nowStageNum = 0;
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
    ///メインウィンドウ名称
    /// </summary>
    const string MAINTEXT = "mainWindow";
    /// <summary>
    ///FPS表記惑名称
    /// </summary>
    const string FPSTEXT = "countFPS";

    private IEnumerator FPScounter = null;

    [SerializeField]
    private List<Ship> selectShip = new List<Ship>();

    private Dictionary<string, bool> clearData = new Dictionary<string, bool>();
    public bool getClearFlug(string stageName)
    {
        if (!clearData.ContainsKey(stageName)) return false;
        return clearData[stageName];
    }
    public bool getClearFlug(Stage stage)
    {
        return getClearFlug(stage.stageName);
    }

    // Use this for initialization
    public override void Start()
    {
        StartCoroutine(systemStart());
    }
    public IEnumerator systemStart()
    {
        setScenery();
        if (FPScounter != null) StopCoroutine(FPScounter);

        yield return null;
        while (!opening) yield return openingAction();
        setBGM();

        setup();

        yield return testAction();
        yield return startStage();

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
    ///メインウィンドウの位置
    /// </summary>
    public IEnumerator textMotion = null;
    /// <summary>
    ///メインウィンドウへのテキスト設定
    /// </summary>
    public void setMainWindow(string setedText, KeyCode? interruption = null)
    {
        if (textMotion != null) StopCoroutine(textMotion);
        StartCoroutine(setMainWindow(setedText, mainWindowInterval, interruption));
    }
    /// <summary>
    ///メインウィンドウへのテキスト設定
    ///イテレータ使用版
    /// </summary>
    public IEnumerator setMainWindow(string setedText, int interval, KeyCode? interruption = null)
    {
        if (setedText != "")
        {
            textMotion = setMainWindowMotion(setedText, interval, interruption);
        }
        else
        {
            textMotion = deleteMainWindowMotion(interval);
        }
        yield return textMotion;
        yield break;
    }
    private IEnumerator setMainWindowMotion(string setedText, int interval, KeyCode? interruption = null)
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

            setSysText(nowText, MAINTEXT, mainWindowPosition);
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

    IEnumerator testAction()
    {
        if (sysPlayer != null)
        {
            List<string> ships = new List<string>();
            for (var i = 0; i < selectShip.Count; i++) ships.Add(selectShip[i].gameObject.name);
            yield return getChoices(ships, i => sysPlayer.copyShipStatus(selectShip[i]));
        }

        yield break;
    }

    IEnumerator openingAction()
    {
        setScenery();
        yield return setMainWindow("Jugemu, Mu Kotobukigen\r\nFrayed five-ko\r\nOf sea gravel Suigyo\r\nWater end-of-line Unrai end Kazeraimatsu\r\nPunished by living in the treatment of sleep eat\r\nYabura forceps of bush forceps\r\nShoe phosphorus cancer Paipopaipo Paipo\r\nGurindai of shoe phosphorus cancer\r\nOf Ponpoko copy of Gurindai of Ponpokona\r\nOf Nagahisa life Chosuke", mainWindowInterval, ButtomZ);

        yield return wait(120);
        yield return setMainWindow("", mainWindowInterval);

        opening = true;
        yield break;
    }

    IEnumerator startStage()
    {
        if (stages.Count <= 0) yield break;

        StartCoroutine(FPScounter = countFPS());
        nowStage = (Stage)Instantiate(stages[nextStageNum], Vector2.zero, transform.rotation);
        nowStage.transform.parent = transform;

        nowStage.resetView();

        yield break;
    }
}
