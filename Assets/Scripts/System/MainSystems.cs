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
        systemStart();
        if(FPScounter != null) StopCoroutine(FPScounter);
        StartCoroutine(FPScounter = countFPS());
    }

    public Coroutine systemStart() => StartCoroutine(systemStartAction());
    protected IEnumerator systemStartAction()
    {
        switchPause(false);

        setScenery();
        Screen.SetResolution(1280, 720, Screen.fullScreen);

        yield return wait(1, isSystem: true);
        while(!opening) yield return openingAction();
        setBGM(initialBGM);

        setup();
        setStage();
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
    public float? shotDownRate
    {
        get {
            if(nowStage == null) return null;
            if(nowStage.enemyAppearances == 0) return null;
            return (float)nowStage.shotsToKill / nowStage.enemyAppearances;
        }
    }
    /// <summary>
    /// 命中率
    /// </summary>
    public float? accuracy
    {
        get {
            if(nowStage == null) return null;
            if(nowStage.attackCount == 0) return null;
            return (float)nowStage.attackHits / nowStage.attackCount;
        }
    }
    /// <summary>
    /// 回避率
    /// </summary>
    public float? evasionRate
    {
        get {
            if(nowStage == null) return null;
            if(nowStage.enemyAttackCount == 0) return null;
            return 1 - (float)nowStage.toHitCount / nowStage.enemyAttackCount;
        }
    }
    /// <summary>
    /// 防御率
    /// </summary>
    public float? protectionRate
    {
        get {
            if(nowStage == null) return null;
            if(nowStage.toHitCount == 0) return null;
            return 1 - (float)nowStage.toDirectHitCount / nowStage.toHitCount;
        }
    }

    /// <summary>
    /// 戦績に対する人工知能のコメント作成
    /// </summary>
    /// <returns></returns>
    public string[] getAiComment()
    {
        var choiceableList = aiComments.Where(comment => comment.Value());
        if(!choiceableList.Any()) return new string[] {
            "特筆することは何もありませんね。",
            "堅実さも結構ですが、偶には振り切った操縦なども見てみたいものです。"
        };
        return choiceableList.Select(comment => comment.Key).selectRandom();
    }
    void setAiComments()
    {
        //撃墜率系
        aiComments.Add(new[]{
            "敵性機全機撃墜、おめでとうございます。\r\n大変素晴らしい戦果です。",
            "傭兵の評価は歩合制、つまり報酬にも期待が持てますよ。\r\n口座への振り込みが待ち遠しいですね。",
            "ついでに私の付属仮想部品の更新など…"
        }, () => 1 <= shotDownRate);
        aiComments.Add(new[]{
            "8割以上の敵性機体を撃墜しましたか。\r\n流石ですね。",
            "「退却も兵の要」とは言いますが、あなたには不要な言葉でしょう。",
            "この調子で稼いでいきましょう。"
        }, () => 0.8f <= shotDownRate && shotDownRate < 1);
        aiComments.Add(new[]{
            "…1機も撃墜せず、ですか。\r\nこれはこれで驚異と言えなくも無いですが…",
            "傭兵という仕事について考えさせられますね。\r\n報酬とはいったい何なのか。"
        }, () => shotDownRate <= 0);

        //命中率系
        aiComments.Add(new[]{
            "まさに1石をもって2羽を落とす。\r\n素晴らしい命中精度と武装制御ですね。",
            "ここまで一掃できるとなると、なるほど。\r\nまるで小型機がゴミの…いえ何でもありません。",
            "次は武装を変えてこの域を目指してみることをお勧めしますよ。\r\n別に私が挑戦するわけでもありませんし。"
        }, () => 2 <= accuracy);
        aiComments.Add(new[]{
            "命中率が1を超えていますね。\r\n1射にて複数に命中させた証です。",
            "まあ1以上の数値は査定評価に響きませんので、実質単なる自己満足ですが。",
            "さらに高みを目指すのでしたら、そうですね。\r\n持続型の線形砲や広域の炸裂弾など常用してみてはいかがでしょうか。"
        }, () => 1 < accuracy && accuracy < 2);
        aiComments.Add(new[]{
            "命中率が0.5未満、つまり半分以上外していますね。",
            "非会敵時も武装を動作させていませんか？\r\n無駄弾は燃料不足と隙の元ですよ。",
            "…もし把握の上での無駄撃ちであるならば、特に私から言うことは有りません。\r\n機体をどう動かそうと、最後は搭乗者の自由なのですから。"
        }, () => 0 < accuracy && accuracy < 0.5f);

        //回避率系
        aiComments.Add(new[]{
            "敵性攻撃を全て回避したようですね。\r\n驚嘆すべき回避能力です。",
            "もう装甲板も障壁も無しで問題無いのではありませんか？"
        }, () => 1 <= evasionRate);
        aiComments.Add(new[]{
            "敵性攻撃をほぼ全て回避したようですね。\r\n驚嘆すべき回避能力です。",
            "もう装甲板も障壁も無しで問題無いのではありませんか？"
        }, () => 0.9f <= evasionRate && evasionRate < 1);
        aiComments.Add(new[]{
            "…敵性攻撃のほぼ全弾命中、おめでとうございます。",
            "逆に何をどうすればこうなるのか、教えていただけませんか？"
        }, () => 0 < evasionRate && evasionRate < 0.1f);
        aiComments.Add(new[]{
            "…敵性攻撃の全弾命中、おめでとうございます。",
            "逆に何をどうすればこうなるのか、教えていただけませんか？"
        }, () => evasionRate == 0);
        aiComments.Add(new[]{
            "回避率が負の値…こんなことも有るのですね。\r\n一時は計測機器の不具合かと。",
            "敵性攻撃を逃がさない秘訣でもあるのでしょうか？\r\nいえ、別に教えていただかなくても結構ですが。"
        }, () => evasionRate < 0);

        //防御率系
        aiComments.Add(new[]{
            "防御率1、完全防御ですか。\r\nこれはまた奇異な成果をあげましたね。",
            "この数値が意味するところは、敵性攻撃を全て障壁で受けたという事実。\r\n弾幕を瞬時に分析し反応する技能の証です。",
            "…ただまあ、もう少し回避していただいても問題はありませんよ？"
        }, () => 1 <= protectionRate && evasionRate < 0.5f);
        aiComments.Add(new[]{
            "すみませんが、ひとつ言わせていただいても？",
            "…強力な敵性攻撃選んで当たりに行ってませんよね？\r\nもしくは余程障壁の薄い自体設計を用いたのでしょうか。",
            "しかし、むしろよくこれで依頼を達成しましたね。\r\nある意味では賞賛すべき…いえ、財政に損害を被るためやはり勘弁してください。"
        }, () => protectionRate <= 0 && evasionRate < 0.5f);
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
            if(interruptions.get()) yield break;
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

    Stage setStage()
    {
        if(stages.Count <= 0) return null;

        nowStage = Instantiate(nextStage ?? mainMenu, Vector2.zero, transform.rotation);
        nowStage.nowParent = transform;
        nextStage = null;

        nowStage.resetView();

        return nowStage;
    }
}
