﻿using UnityEngine;
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
    public bool GetClearFlug(string stageName)
    {
        if(!clearData.ContainsKey(stageName)) return false;
        return clearData[stageName];
    }
    public bool GetClearFlug(Stage stage)
    {
        return GetClearFlug(stage.displayName);
    }

    // Use this for initialization
    public override void Start()
    {
        SetAiComments();
        StartSystem();
        if(FPScounter != null) StopCoroutine(FPScounter);
        StartCoroutine(FPScounter = CountFPS());
    }

    public Coroutine StartSystem() => StartCoroutine(StartSystemAction());
    protected IEnumerator StartSystemAction()
    {
        SwitchPause(false);

        SetScenery();
        Screen.SetResolution(1280, 720, Screen.fullScreen);

        yield return Wait(1, isSystem: true);
        while(!opening) yield return OpeningAction();
        SetBGM(initialBGM);

        Setup();
        SetStage();
        nowStage.StartStageProcess();

        yield break;
    }
    private void Setup()
    {
        SetScenery();
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
    public uint CountEnemyAppearances(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return ++nowStage.enemyAppearances;
    }
    /// <summary>
    /// 総撃墜数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>総撃墜数</returns>
    public uint CountShotsToKill(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return ++nowStage.shotsToKill;
    }
    /// <summary>
    /// 攻撃回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>攻撃回数</returns>
    public uint CountAttackCount(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return ++nowStage.attackCount;
    }
    /// <summary>
    /// 攻撃命中回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>攻撃命中回数</returns>
    public uint CountAttackHits(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return ++nowStage.attackHits;
    }
    /// <summary>
    /// 敵弾生成総数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>敵弾生成総数</returns>
    public uint CountEnemyAttackCount(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return ++nowStage.enemyAttackCount;
    }
    /// <summary>
    /// 被弾回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>被弾回数</returns>
    public uint CountToHitCount(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return ++nowStage.toHitCount;
    }
    /// <summary>
    /// 直撃被弾回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>直撃被弾回数</returns>
    public uint CountToDirectHitCount(int plusCount = 1)
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
    public string[] GetAiComment()
    {
        var choiceableList = aiComments.Where(comment => comment.Value());
        if(!choiceableList.Any()) return new string[] {
            "特筆することは何もありませんね。",
            "堅実さも結構ですが、偶には振り切った操縦なども見てみたいものです。"
        };
        return choiceableList.Select(comment => comment.Key).SelectRandom();
    }
    void SetAiComments()
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
    /// メインウィンドウの動作状態
    /// </summary>
    public IEnumerator textMotion = null;
    /// <summary>
    ///メインウィンドウへのテキスト設定
    ///イテレータ使用版
    /// </summary>
    /// <param name="setedText">表示する文章</param>
    /// <param name="interval">表示時間間隔</param>
    /// <param name="interruptions">反応キー種類</param>
    /// <param name="size">文字サイズ</param>
    /// <param name="setPosition">表示位置</param>
    /// <param name="pivot">表示位置座標の基準点</param>
    /// <param name="interruptable">キー押下時に表示を終了するフラグ</param>
    /// <returns>イテレータ</returns>
    public IEnumerator SetMainWindow(string setedText, int interval = Configs.Window.MAIN_WINDOW_INTERVAL, List<KeyCode> interruptions = null, int size = Configs.Texts.CHAR_SIZE, Vector2? setPosition = null, TextAnchor pivot = TextAnchor.UpperLeft, bool interruptable = true)
    {
        if(setedText != "")
        {
            textMotion = SetMainWindowMotion(setedText, setPosition ?? screenSize.Scaling(new Vector2(-1, 1)) / 2, interval, interruptions, size, pivot, interruptable);
        }
        else
        {
            textMotion = DeleteMainWindowMotion(interval);
        }
        yield return textMotion;
        yield break;
    }
    private IEnumerator SetMainWindowMotion(string setedText, Vector2 setPosition, int interval, List<KeyCode> interruptions, int size, TextAnchor pivot, bool interruptable)
    {
        interruptions = interruptions ?? new List<KeyCode>();

        var markText = SetSysText(setedText, setPosition, pivot, charSize: size);
        var setUpperLeftPosition = markText.getVertexPosition(TextAnchor.UpperLeft);
        markText.selfDestroy();

        for(int charNum = 1; charNum <= setedText.Length; charNum++)
        {
            string nowText = setedText.Substring(0, charNum);

            mainText = SetSysText(nowText,
                setUpperLeftPosition,
                TextAnchor.UpperLeft,
                charSize: size,
                bold: true,
                defaultText: mainText);
            SoundSE(ses.escapementSE, 0.3f, 1.2f);

            if(interval > 0)
            {
                yield return Wait(interval, () => interruptions.Judge(Key.Timing.ON));
                if(nowText.Substring(nowText.Length - 1, 1) == " ") yield return Wait(interval * 6, () => interruptions.Judge(Key.Timing.ON));
            }
            if(interruptable && interruptions.Judge(Key.Timing.ON)) yield break;
        }
        yield break;
    }
    private IEnumerator DeleteMainWindowMotion(int interval)
    {
        mainText.selfDestroy();
        yield break;
    }

    IEnumerator CountFPS()
    {
        if(!Debug.isDebugBuild) yield break;
        while(true)
        {
            yield return new WaitForSeconds(1);
            fpsText = SetSysText($@"
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

    protected override IEnumerator OpeningAction()
    {
        SetScenery();
        yield return Fadein();
        yield return SetMainWindow("Jugemu, Mu Kotobukigen\r\nFrayed five-ko\r\nOf sea gravel Suigyo\r\nWater end-of-line Unrai end Kazeraimatsu\r\nPunished by living in the treatment of sleep eat\r\nYabura forceps of bush forceps\r\nShoe phosphorus cancer Paipopaipo Paipo\r\nGurindai of shoe phosphorus cancer\r\nOf Ponpoko copy of Gurindai of Ponpokona\r\nOf Nagahisa life Chosuke", interruptions: Key.Set.decide, size: 18);

        yield return Wait(120);
        yield return SetMainWindow("");
        yield return Fadeout();

        opening = true;
        yield break;
    }

    Stage SetStage()
    {
        if(stages.Count <= 0) return null;

        nowStage = Instantiate(nextStage ?? mainMenu, Vector2.zero, transform.rotation);
        nowStage.nowParent = transform;
        nextStage = null;

        nowStage.ResetView();

        return nowStage;
    }
}
