﻿using UnityEngine;
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
    ///オープニング再生済みフラグ
    /// </summary>
    [SerializeField]
    private bool opening = false;

    /// <summary>
    ///初期配置用プレイヤーPrefab
    /// </summary>
    [SerializeField]
    private Player initialPlayer;

    /// <summary>
    ///各種キーのAxes名
    /// </summary>
    const string decisionName = "ShotRight";

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
        if (FPScounter != null) StopCoroutine(FPScounter);

        while (!opening) yield return openingAction();

        Instantiate(initialPlayer).name = playerName;
        getPlayer().deleteArmorBar();
        getPlayer().setArmorBar();
        Application.targetFrameRate = 120;
        flamecount = 0;

        yield return testAction();
        yield return startStage();

        getPlayer().canRecieveKey = true;

        yield break;
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
    public Vector2 mainWindowPosition = new Vector2(0, 0);
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
    public void setMainWindow(string setedText)
    {
        if (textMotion != null) StopCoroutine(textMotion);
        StartCoroutine(setMainWindow(setedText, mainWindowInterval));
    }
    /// <summary>
    ///メインウィンドウへのテキスト設定
    ///イテレータ使用版
    /// </summary>
    public IEnumerator setMainWindow(string setedText, int interval)
    {
        if (setedText != "")
        {
            textMotion = setMainWindowMotion(setedText, interval);
        }
        else
        {
            textMotion = deleteMainWindowMotion(interval);
        }
        yield return textMotion;
        yield break;
    }
    private IEnumerator setMainWindowMotion(string setedText, int interval)
    {
        for (int charNum = 1; charNum <= setedText.Length; charNum++)
        {
            string nowText = setedText.Substring(0, charNum);

            setSysText(nowText, "mainWindow", mainWindowPosition);
            if (interval > 0) yield return wait(interval);
        }
        yield break;
    }
    private IEnumerator deleteMainWindowMotion(int interval)
    {
        var setedText = getSysText("mainWindow");
        for (int charNum = 0; charNum < setedText.Length; charNum++)
        {
            string nowText = setedText.Substring(charNum, setedText.Length - charNum);

            setSysText(nowText, "mainWindow", mainWindowPosition);
            if (interval > 0) yield return wait(interval);
        }
        deleteSysText("mainWindow");
        yield break;
    }

    IEnumerator countFPS()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            setSysText("fps:" + flamecount + ":" + 1 / Time.deltaTime, "countFPS", new Vector2(-60, 120));
            flamecount = 0;
        }
    }

    IEnumerator testAction()
    {
        yield return null;

        deleteSysText("countFPS");
        string textKey = "ShipSelect";
        bool toNext = false;
        int selected = 0;

        while (!toNext)
        {
            selected = selected % selectShip.Count;
            setSysText("← " + selectShip[selected].gameObject.name + " →", textKey, new Vector2(240, -105));
            getPlayer().copyShipStatus(selectShip[selected]);

            toNext = false;
            bool inputRightKey = false;
            bool inputLeftKey = false;

            while (!toNext && !inputRightKey && !inputLeftKey)
            {
                toNext = Input.GetKeyDown(KeyCode.Z);
                inputRightKey = Input.GetKeyDown(KeyCode.RightArrow);
                inputLeftKey = Input.GetKeyDown(KeyCode.LeftArrow);

                yield return null;
            }

            if (inputRightKey) selected += 1;
            if (inputLeftKey) selected += selectShip.Count - 1;

            yield return new WaitForSeconds(0.1f);
        }

        deleteSysText(textKey);
        yield break;
    }

    IEnumerator openingAction()
    {
        setScenery();
        yield return setMainWindow("Jugemu, Mu Kotobukigen\r\nFrayed five-ko\r\nOf sea gravel Suigyo\r\nWater end-of-line Unrai end Kazeraimatsu\r\nPunished by living in the treatment of sleep eat\r\nYabura forceps of bush forceps\r\nShoe phosphorus cancer Paipopaipo Paipo\r\nGurindai of shoe phosphorus cancer\r\nOf Ponpoko copy of Gurindai of Ponpokona\r\nOf Nagahisa life Chosuke", mainWindowInterval);

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
        nowStage = (Stage)Instantiate(stages[nowStageNum], new Vector2(0, 0), transform.rotation);
        nowStage.transform.parent = transform;

        yield break;
    }
}
