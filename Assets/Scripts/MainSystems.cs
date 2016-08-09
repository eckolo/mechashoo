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
    [SerializeField]
    private List<Stage> stages = new List<Stage>();
    /// <summary>
    ///現在のステージ番号
    /// </summary>
    [SerializeField]
    private int nowStageNum = 0;
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

    // Use this for initialization
    public override void Start()
    {
        StartCoroutine(systemStart());
    }
    public IEnumerator systemStart()
    {
        setScenery();
        if (FPScounter != null) StopCoroutine(FPScounter);

        while (!opening) yield return openingAction();
        setBGM();

        setup();

        yield return testAction();
        yield return startStage();

        getPlayer().canRecieveKey = true;

        yield break;
    }
    private void setup()
    {
        setScenery();

        getPlayer().transform.position = initialPlayerPosition;
        getPlayer().deleteArmorBar();
        getPlayer().setArmorBar();
        Application.targetFrameRate = 120;
        flamecount = 0;
    }

    // Update is called once per frame
    public override void Update()
    {
        //setSysText("" + Application.targetFrameRate);
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
        for (int charNum = 1; charNum <= setedText.Length; charNum++)
        {
            string nowText = setedText.Substring(0, charNum);

            setSysText(nowText, MAINTEXT, mainWindowPosition);
            if (charNum % 12 == 0) soundSE(escapementSE, 0.3f, 1.2f);

            if (interval > 0)
            {
                yield return wait(interval);
                if (nowText.Substring(nowText.Length - 1, 1) == " ") yield return wait(interval * 6);
            }
            if (interruption != null && Input.GetKeyDown((KeyCode)interruption)) yield break;
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
        List<string> ships = new List<string>();
        for (var i = 0; i < selectShip.Count; i++) ships.Add(selectShip[i].gameObject.name);
        yield return getChoices(ships, i => getPlayer().copyShipStatus(selectShip[i]));

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
        opening = true;
        nowStage = (Stage)Instantiate(stages[nowStageNum], Vector2.zero, transform.rotation);
        nowStage.transform.parent = transform;

        yield break;
    }

    public int? lastSelect = null;
    public delegate void Action(int nowSelect);
    public IEnumerator getChoices(List<string> choices, Action action = null, bool canCancel = false, int? setSize = null, Vector2? setPosition = null, int newSelect = 0)
    {
        lastSelect = null;

        const string textName = "choices";
        const float baseMas = 45;
        const float interval = 1.2f;

        Vector2 position = (setPosition ?? Vector2.zero) + Vector2.right * 240;
        int baseSize = setSize ?? defaultTextSize;
        var nowSelect = newSelect;
        Vector2 centerPosition = position + Vector2.down * baseSize * interval * (choices.Count - 1) / 2;

        Window backWindow = (Window)Instantiate(basicWindow, Vector2.up * centerPosition.y / baseMas, transform.rotation);

        bool toDecision = false;
        bool toCancel = false;
        while (!toDecision && !toCancel)
        {
            nowSelect %= choices.Count;
            if (action != null) action(nowSelect);

            float width = 0;
            for (int i = 0; i < choices.Count; i++)
            {
                var choice = (i == nowSelect ? ">\t" : "\t") + choices[i];
                var nowPosition = position + Vector2.down * baseSize * interval * i;
                var choiceObj = setSysText(choice, textName + i, nowPosition, baseSize, TextAnchor.MiddleLeft);
                width = Mathf.Max(choiceObj.GetComponent<RectTransform>().sizeDelta.x, width);
            }
            backWindow.transform.localScale = Vector2.right * (width / baseMas + 1) + Vector2.up * baseSize * interval * (choices.Count + 1) / baseMas;

            bool inputUpKey = false;
            bool inputDownKey = false;

            while (!toDecision && !toCancel && !inputUpKey && !inputDownKey)
            {
                toDecision = Input.GetKeyDown(ButtomZ);
                toCancel = Input.GetKeyDown(ButtomX) && canCancel;
                inputUpKey = Input.GetKeyDown(ButtomUp) || Input.GetKeyDown(ButtomRight);
                inputDownKey = Input.GetKeyDown(ButtomDown) || Input.GetKeyDown(ButtomLeft);

                yield return null;
            }

            if (inputDownKey) nowSelect += 1;
            if (inputUpKey) nowSelect += choices.Count - 1;
            if (toCancel) nowSelect = -1;
        }

        for (int i = 0; i < choices.Count; i++) deleteSysText(textName + i);
        backWindow.selfDestroy();
        yield break;
    }
}
