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
    /// メインストーリーの進行度合い
    /// 何ステージ目まで進んだか
    /// </summary>
    public uint storyPhase
    {
        get {
            return _storyPhase;
        }
        set {
            _storyPhase = (uint)Mathf.Max(value, _storyPhase);
        }
    }
    uint _storyPhase = Configs.START_STORY_PHASE;

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
    /// 敵機出現数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>敵機出現数</returns>
    public uint countEnemyAppearances(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return ++nowStage.enemyAppearances;
    }
    /// <summary>
    /// 総撃墜数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>総撃墜数</returns>
    public uint countShotsToKill(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return ++nowStage.shotsToKill;
    }
    /// <summary>
    /// 攻撃回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>攻撃回数</returns>
    public uint countAttackCount(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return ++nowStage.attackCount;
    }
    /// <summary>
    /// 攻撃命中回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>攻撃命中回数</returns>
    public uint countAttackHits(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return ++nowStage.attackHits;
    }
    /// <summary>
    /// 被弾回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>被弾回数</returns>
    public uint countToHitCount(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return ++nowStage.toHitCount;
    }
    /// <summary>
    /// 直撃被弾回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>直撃被弾回数</returns>
    public uint countToDirectHitCount(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return ++nowStage.toDirectHitCount;
    }

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
    public IEnumerator setMainWindow(string setedText, int interval, KeyCode? interruption = null, int size = Configs.DEFAULT_TEXT_SIZE, Vector2? setPosition = null, TextAnchor pivot = TextAnchor.UpperLeft)
    {
        if(setedText != "")
        {
            textMotion = setMainWindowMotion(setedText, setPosition ?? screenSize.scaling(new Vector2(-1, 1)) / 2, interval, interruption, size, pivot);
        }
        else
        {
            textMotion = deleteMainWindowMotion(interval);
        }
        yield return textMotion;
        yield break;
    }
    private IEnumerator setMainWindowMotion(string setedText, Vector2 setPosition, int interval, KeyCode? interruption, int size, TextAnchor pivot)
    {
        var interruptions = new List<KeyCode>
                {
                    KeyCode.KeypadEnter,
                    KeyCode.Space
                };
        if(interruption != null) interruptions.Add((KeyCode)interruption);

        for(int charNum = 1; charNum <= setedText.Length; charNum++)
        {
            string nowText = setedText.Substring(0, charNum);

            mainText = setSysText(nowText, setPosition, pivot, charSize: size, defaultText: mainText);
            if(charNum % 12 == 0) soundSE(ses.escapementSE, 0.3f, 1.2f);

            if(interval > 0)
            {
                yield return wait(interval, interruptions);
                if(nowText.Substring(nowText.Length - 1, 1) == " ") yield return wait(interval * 6, interruptions);
            }
            if(onKeysDecision(interruptions)) yield break;
        }
        yield break;
    }
    private IEnumerator deleteMainWindowMotion(int interval)
    {
        mainText.selfDestroy();
        yield break;
    }

    IEnumerator countFPS()
    {
        if(!Debug.isDebugBuild) yield break;
        while(true)
        {
            yield return new WaitForSeconds(1);
            fpsText = setSysText($@"
敵機出現数:{nowStage?.enemyAppearances}
総撃墜数:{nowStage?.shotsToKill}
攻撃回数:{nowStage?.attackCount}
攻撃命中回数:{nowStage?.attackHits}
被弾回数:{nowStage?.toHitCount}
直撃被弾回数:{nowStage?.toDirectHitCount}
fps:{flamecount}:{1 / Time.deltaTime}", -screenSize / 2, TextAnchor.LowerLeft, 12, TextAnchor.LowerLeft, defaultText: fpsText);
            flamecount = 0;
        }
    }

    protected override IEnumerator openingAction()
    {
        setScenery();
        yield return fadein();
        yield return setMainWindow("Jugemu, Mu Kotobukigen\r\nFrayed five-ko\r\nOf sea gravel Suigyo\r\nWater end-of-line Unrai end Kazeraimatsu\r\nPunished by living in the treatment of sleep eat\r\nYabura forceps of bush forceps\r\nShoe phosphorus cancer Paipopaipo Paipo\r\nGurindai of shoe phosphorus cancer\r\nOf Ponpoko copy of Gurindai of Ponpokona\r\nOf Nagahisa life Chosuke", mainWindowInterval, Configs.Buttom.Z, size: 18);

        yield return wait(120);
        yield return setMainWindow("", mainWindowInterval);
        yield return fadeout();

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
