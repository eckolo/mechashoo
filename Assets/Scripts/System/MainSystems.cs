using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Linq;

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
    public Text mainText { get; private set; } = null;
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
        setAiComments();
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
        setBGM(initialBGM);

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
    /// 敵弾生成総数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>敵弾生成総数</returns>
    public uint countEnemyAttackCount(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return ++nowStage.enemyAttackCount;
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
    /// 撃墜率
    /// </summary>
    public float shotDownRate
    {
        get {
            if(nowStage == null) return 0;
            if(nowStage.enemyAppearances == 0) return 0;
            return (float)nowStage.shotsToKill / nowStage.enemyAppearances;
        }
    }
    /// <summary>
    /// 命中率
    /// </summary>
    public float accuracy
    {
        get {
            if(nowStage == null) return 0;
            if(nowStage.attackCount == 0) return 0;
            return (float)nowStage.attackHits / nowStage.attackCount;
        }
    }
    /// <summary>
    /// 回避率
    /// </summary>
    public float evasionRate
    {
        get {
            if(nowStage == null) return 0;
            if(nowStage.enemyAttackCount == 0) return 0;
            return 1 - (float)nowStage.toHitCount / nowStage.enemyAttackCount;
        }
    }
    /// <summary>
    /// 防御率
    /// </summary>
    public float protectionRate
    {
        get {
            if(nowStage == null) return 0;
            if(nowStage.toHitCount == 0) return 0;
            return 1 - (float)nowStage.toDirectHitCount / nowStage.toHitCount;
        }
    }

    /// <summary>
    /// 戦績に対する人工知能のコメント作成
    /// </summary>
    /// <returns></returns>
    public string[] getAiComment() => aiComments
        .Where(comment => comment.Value())
        .Select(comment => comment.Key)
        .selectRandom();
    void setAiComments()
    {
        //命中率系
        aiComments.Add(new[]{
            "まさに1石をもって2羽を落とす。\r\n素晴らしい命中精度と武装制御ですね。",
            "ここまで一掃できるとなると、なるほど。\r\nまるで小型機がゴミの…いえ何でもありません。",
            "次は武装を変えてこの域を目指してみることをお勧めしますよ。\r\n別に私が挑戦するわけでもありませんし。"
        }, () => accuracy >= 2);
        aiComments.Add(new[]{
            "命中率が1を超えていますね。\r\n1射にて複数を仕留めた証です。",
            "まあ1以上の数値は査定評価に響きませんので、実質単なる自己満足ですが。",
            "さらに高みを目指すのでしたら、そうですね。\r\n持続型の線形砲で薙ぎ払うなどしてみてはいかがでしょうか。"
        }, () => 2 > accuracy && accuracy > 1);
        aiComments.Add(new[]{
            "中々の命中精度ですね。",
            "単に当てるに留まらず、「当て方」を意識すると良いのでは。",
            "偶には二兎を追ってみるものです。"
        }, () => 1 > accuracy && accuracy >= 0.8f);
        aiComments.Add(new[]{
            $"命中率{accuracy.ToString("F2")}\r\n…まあそれなりですか。",
            "ばら撒きも戦法ですが、当てる意識も持ちたいところですね。",
            "雑な連射を控えれば、自ずと私の評価も雑ではなくなるでしょう。\r\n多分。"
        }, () => 0.8f > accuracy && accuracy >= 0.5f);
        aiComments.Add(new[]{
            "命中率が0.5未満、つまり半分以上外していますね。",
            "非会敵時も武装を動作させていませんか？\r\n無駄弾は燃料不足と隙の元ですよ。",
            "…もし把握の上での無駄撃ちであるならば、特に私から言うことは有りません。\r\n機体をどう動かそうと、最後は搭乗者の自由なのですから。"
        }, () => 0.5f > accuracy);
    }
    Dictionary<string[], Func<bool>> aiComments = new Dictionary<string[], Func<bool>>();

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

        var markText = setSysText(setedText, setPosition, pivot, charSize: size);
        var setUpperLeftPosition = markText.getVertexPosition(TextAnchor.UpperLeft);
        markText.selfDestroy();

        for(int charNum = 1; charNum <= setedText.Length; charNum++)
        {
            string nowText = setedText.Substring(0, charNum);

            mainText = setSysText(nowText,
                setUpperLeftPosition,
                TextAnchor.UpperLeft,
                charSize: size,
                defaultText: mainText);
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
敵弾生成総数:{nowStage?.enemyAttackCount}
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
