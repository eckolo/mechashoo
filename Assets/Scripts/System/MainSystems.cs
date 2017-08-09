using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Linq;
using static Configs.SaveKeys;

public partial class MainSystems : Stage
{
    /// <summary>
    /// ステージリスト
    /// </summary>
    public List<Stage> stages = new List<Stage>();
    /// <summary>
    /// メインメニュー
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
    uint _storyPhase = Configs.StoryPhase.START;

    /// <summary>
    /// 次のステージ番号
    /// </summary>
    [NonSerialized]
    public Stage nextStage = null;
    /// <summary>
    /// 現在のステージオブジェクト
    /// </summary>
    [NonSerialized]
    public Stage nowStage = null;

    /// <summary>
    /// 各種Player用バーオブジェクト
    /// </summary>
    [NonSerialized]
    public Bar playerHPbar = null;
    [NonSerialized]
    public Bar playerBRbar = null;
    [NonSerialized]
    public Bar playerENbar = null;

    /// <summary>
    /// オープニング再生済みフラグ
    /// </summary>
    private bool opening = false;
    /// <summary>
    /// メインテキストオブジェクト
    /// </summary>
    public Text mainText { get; private set; } = null;
    /// <summary>
    /// FPS表記
    /// </summary>
    Text fpsText = null;

    private IEnumerator FPScounter = null;

    [SerializeField]
    public List<Ship.CoreData> shipDataMylist = new List<Ship.CoreData>();
    [SerializeField]
    public Ship.CoreData adoptedShipData = null;
    /// <summary>
    /// デフォルトの所持武装
    /// </summary>
    [SerializeField]
    private List<Weapon> defaultPossessionWeapons = new List<Weapon>();
    /// <summary>
    /// デフォルトの所持機体
    /// </summary>
    [SerializeField]
    private List<Ship> defaultPossessionShips = new List<Ship>();

    /// <summary>
    /// 所持武装
    /// </summary>
    private List<PossessionState<Weapon>> _possessionWeapons = new List<PossessionState<Weapon>>();
    /// <summary>
    /// 所持武装実体リスト
    /// </summary>
    public List<Weapon> possessionWeapons
    {
        get {
            return _possessionWeapons
                .Where(possession => possession.isPossessed)
                .Select(possession => possession.entity)
                .ToList();
        }
        set {
            _possessionWeapons = value
                .Select(weapon => new PossessionState<Weapon> { entity = weapon, number = 1 })
                .ToList();
        }
    }
    /// <summary>
    /// 所持機体
    /// </summary>
    private List<PossessionState<Ship>> _possessionShips = new List<PossessionState<Ship>>();
    /// <summary>
    /// 所持機体実体リスト
    /// </summary>
    public List<Ship> possessionShips
    {
        get {
            return _possessionShips
                .Where(possession => possession.isPossessed)
                .Select(possession => possession.entity)
                .ToList();
        }
        set {
            _possessionShips = value
                .Select(ship => new PossessionState<Ship> { entity = ship, number = 1 })
                .ToList();
        }
    }

    /// <summary>
    /// 各国の優勢度
    /// </summary>
    public Dominance dominance { get; set; } = new Dominance();
    /// <summary>
    /// 各国の優勢度クラス
    /// </summary>
    [SerializeField]
    public class Dominance
    {
        /// <summary>
        /// 央星帝国
        /// </summary>
        [SerializeField]
        public int theStarEmpire = 0;
        /// <summary>
        /// 古王国
        /// </summary>
        [SerializeField]
        public int oldKingdom = 0;
        /// <summary>
        /// 共和国
        /// </summary>
        [SerializeField]
        public int republic = 0;
        /// <summary>
        /// 公国
        /// </summary>
        [SerializeField]
        public int principality = 0;
    }

    /// <summary>
    /// 武装取得関数
    /// </summary>
    /// <param name="obtainedWeapon">取得する武装</param>
    /// <returns>取得処理に成功したか否か</returns>
    public bool ObtainArticles(Weapon obtainedWeapon)
    {
        if(_possessionWeapons.Select(possession => possession.entity).Contains(obtainedWeapon)) return false;
        _possessionWeapons.Add(new PossessionState<Weapon>
        {
            entity = obtainedWeapon,
            number = 1
        });
        SaveData.SetList(POSSESSION_WEAPONS, possessionWeapons);
        return true;
    }
    /// <summary>
    /// 機体取得関数
    /// </summary>
    /// <param name="obtainedShip">取得する機体</param>
    /// <returns>取得処理に成功したか否か</returns>
    public bool ObtainArticles(Ship obtainedShip)
    {
        if(_possessionShips.Select(possession => possession.entity).Contains(obtainedShip)) return false;
        _possessionShips.Add(new PossessionState<Ship>
        {
            entity = obtainedShip,
            number = 1
        });
        SaveData.SetList(POSSESSION_SHIPS, possessionShips);
        return true;
    }

    void InitializePossessions()
    {
        if(!_possessionWeapons.Any()) possessionWeapons = defaultPossessionWeapons;
        if(!_possessionShips.Any()) possessionShips = defaultPossessionShips;
        SaveData.SetList(POSSESSION_WEAPONS, possessionWeapons);
        SaveData.SetList(POSSESSION_SHIPS, possessionShips);
        SaveData.Save();
    }

    public class PossessionState<Entity> where Entity : Methods
    {
        public Entity entity { get; set; }
        public uint number { get; set; } = 0;
        public bool isPossessed => number > 0;
    }

    private Dictionary<string, bool> clearData = new Dictionary<string, bool>();
    public bool GetClearFlug(Stage stage) => GetClearFlug(stage.displayName);
    bool GetClearFlug(string stageName)
    {
        if(!clearData.ContainsKey(stageName)) return false;
        return clearData[stageName];
    }
    public bool SetClearFlug(Stage stage, bool clear = true) => SetClearFlug(stage.displayName, clear);
    bool SetClearFlug(string stageName, bool clear)
    {
        if(!clearData.ContainsKey(stageName)) clearData.Add(stageName, clear);
        else clearData[stageName] = clear;
        return clearData[stageName];
    }

    // Use this for initialization
    public override void Start()
    {
        LoadAllData();
        SetAiComments();
        StartSystem();
        if(FPScounter != null) StopCoroutine(FPScounter);
        StartCoroutine(FPScounter = CountFPS());
        StartCoroutine(DisplaySaving());
    }

    void LoadAllData()
    {
        storyPhase = (uint)SaveData.GetInt(STORY_PHASE, (int)Configs.StoryPhase.START);
        adoptedShipData = SaveData.GetClass(ADOPTED_SHIP_DATA, default(Ship.CoreData));
        shipDataMylist = SaveData.GetList(SHIP_DATA_MYLIST, new List<Ship.CoreData>());
        possessionWeapons = SaveData.GetList(POSSESSION_WEAPONS, defaultPossessionWeapons);
        possessionShips = SaveData.GetList(POSSESSION_SHIPS, defaultPossessionShips);
        Configs.Volume.bgm = SaveData.GetFloat(BGM_VOLUME, Configs.Volume.BGM_DEFAULT);
        Configs.Volume.se = SaveData.GetFloat(SE_VOLUME, Configs.Volume.SE_DEFAULT);
        Configs.AimingMethod = SaveData.GetInt(AIMING_METHOD, (int)Configs.AIMING_METHOD_DEAULT)
            .Normalize<Configs.AimingOperationOption>();
        dominance = SaveData.GetClass(DOMINANCE, new Dominance());
    }

    public Coroutine StartSystem() => StartCoroutine(StartSystemAction());
    protected IEnumerator StartSystemAction()
    {
        SwitchPause(false);

        InitializePossessions();

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
        return nowStage.IncreaseEnemyAppearances(plusCount);
    }
    /// <summary>
    /// 撃墜必須敵機出現数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>撃墜必須敵機出現数</returns>
    public uint CountMinimumShotDown(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return nowStage.IncreaseMinimumShotDown(plusCount);
    }
    /// <summary>
    /// 接敵回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>敵機出現数</returns>
    public uint CountOpposeEnemy(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return nowStage.IncreaseOpposeEnemy(plusCount);
    }
    /// <summary>
    /// 総撃墜数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>総撃墜数</returns>
    public uint CountShotsToKill(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return nowStage.IncreaseShotsToKill(plusCount);
    }
    /// <summary>
    /// 攻撃回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>攻撃回数</returns>
    public uint CountAttackCount(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return nowStage.IncreaseAttackCount(plusCount);
    }
    /// <summary>
    /// 攻撃命中回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>攻撃命中回数</returns>
    public uint CountAttackHits(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return nowStage.IncreaseAttackHits(plusCount);
    }
    /// <summary>
    /// 敵弾生成総数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>敵弾生成総数</returns>
    public uint CountEnemyAttackCount(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return nowStage.IncreaseEnemyAttackCount(plusCount);
    }
    /// <summary>
    /// 被弾回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>被弾回数</returns>
    public uint CountToHitCount(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return nowStage.IncreaseToHitCount(plusCount);
    }
    /// <summary>
    /// 直撃被弾回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>直撃被弾回数</returns>
    public uint CountToDirectHitCount(int plusCount = 1)
    {
        if(nowStage == null) return 0;
        return nowStage.IncreaseToDirectHitCount(plusCount);
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
    /// 接敵率
    /// </summary>
    public float? opposeEnemyRate
    {
        get {
            if(nowStage == null) return null;
            if(nowStage.enemyAppearances == 0) return null;
            return (float)nowStage.opposeEnemy / nowStage.enemyAppearances;
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

        //撃墜率系亜種
        aiComments.Add(new[]{
            "見敵必殺、反応した相手だけを的確に落としましたね。",
            "なんともまあ傭兵としては評価しづらい技能です。"
        }, () => sys.nowStage.shotsToKill == sys.nowStage.opposeEnemy && shotDownRate < 0.8f);
        aiComments.Add(new[]{
            "接敵数よりも撃墜数の方が大きいとは…つまり気づかれずに撃墜したと。\n\rまた珍妙なことをしでかしましたね。",
            "機体相手の暗殺稼業でもやってみますか？\r\n聞いたことありませんけども。"
        }, () => sys.nowStage.shotsToKill > sys.nowStage.opposeEnemy);
        aiComments.Add(new[]{
            "…本当に最低限のみの撃墜、ですか。\r\nこれはこれで驚異と言えなくも無いですが…",
            "傭兵という仕事について考えさせられますね。\r\n報酬とはいったい何なのか。"
        }, () => sys.nowStage.shotsToKill <= sys.nowStage.minimumShotDown);

        //感知率系
        aiComments.Add(new[]{
            "敵機の半数に関知されず切り抜けるとは、お見事です。",
            "ただ傭兵向きの能力ではありませんね。\r\n秘密情報部への推薦でもいかがでしょう。"
        }, () => sys.opposeEnemyRate < 0.5f);

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
            "…強力な敵性攻撃選んで当たりに行ってませんよね？\r\nもしくは余程障壁の薄い機体でしたか。",
            "しかし、むしろよくこれで依頼を達成しましたね。\r\nある意味では賞賛すべき…いえ、財政に損害を被るためやはり勘弁してください。"
        }, () => protectionRate <= 0 && evasionRate < 0.5f);
    }
    Dictionary<string[], Func<bool>> aiComments = new Dictionary<string[], Func<bool>>();

    /// <summary>
    /// メインウィンドウの動作状態
    /// </summary>
    public IEnumerator textMotion = null;
    /// <summary>
    /// メインウィンドウへのテキスト設定
    /// イテレータ使用版
    /// </summary>
    /// <param name="setedText">表示する文章</param>
    /// <param name="interval">表示時間間隔</param>
    /// <param name="interruptions">反応キー種類</param>
    /// <param name="size">文字サイズ</param>
    /// <param name="setPosition">表示位置</param>
    /// <param name="pivot">表示位置座標の基準点</param>
    /// <param name="interruptable">キー押下時に表示を終了するフラグ</param>
    /// <returns>コルーチン</returns>
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
        var setUpperLeftPosition = markText.VertexPosition(TextAnchor.UpperLeft);
        markText.SelfDestroy();

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
        mainText.SelfDestroy();
        yield break;
    }

    IEnumerator CountFPS()
    {
        if(!Debug.isDebugBuild) yield break;
        while(true)
        {
            yield return new WaitForSeconds(1);
            fpsText = SetSysText($@"
ストーリー:{storyPhase}
各国優勢度:{dominance.theStarEmpire},{dominance.oldKingdom},{dominance.republic},{dominance.principality}
敵機出現数:{nowStage?.enemyAppearances}
最低撃墜数:{nowStage?.minimumShotDown}
総撃墜数:{nowStage?.shotsToKill}
接敵回数:{nowStage?.opposeEnemy}
攻撃回数:{nowStage?.attackCount}
攻撃命中回数:{nowStage?.attackHits}
敵弾生成総数:{nowStage?.enemyAttackCount}
被弾回数:{nowStage?.toHitCount}
直撃被弾回数:{nowStage?.toDirectHitCount}
fps:{flamecount}:{1 / Time.deltaTime}", -screenSize / 2 + Vector2.up * savingText.AreaSize().y, TextAnchor.LowerLeft, 12, TextAnchor.LowerLeft, defaultText: fpsText);
            flamecount = 0;
        }
    }

    public static bool onSaving { get; set; } = false;
    Text savingText = null;
    IEnumerator DisplaySaving()
    {
        if(!Debug.isDebugBuild) yield break;
        for(var time = 0; true; time++)
        {
            var text = onSaving ? "各種情報記録中" + new string('.', time = time % 4) : "";
            savingText = SetSysText(text, -screenSize / 2, TextAnchor.LowerLeft, 18, TextAnchor.LowerLeft, defaultText: savingText);
            yield return new WaitForSeconds(1);
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
    /// <summary>
    /// BGM設定関数
    /// </summary>
    public static AudioSource SetBGM(AudioClip setBGM = null)
    {
        var baseMusic = GameObject.Find("MusicRoot");
        foreach(Transform oldMusic in baseMusic.transform)
        {
            var oldBgmRoot = oldMusic.GetComponent<BGMroot>();
            if(oldBgmRoot.audioSource.clip == setBGM) return oldBgmRoot.audioSource;
            Destroy(oldMusic.gameObject);
        }

        if(setBGM == null) return null;

        var BGM = Instantiate(sys.baseObjects.BGMrootObject);
        BGM.transform.SetParent(baseMusic.transform);
        BGM.audioSource.clip = setBGM;
        BGM.audioSource.Play();

        return BGM.audioSource;
    }
    /// <summary>
    /// BGM音量設定関数
    /// </summary>
    /// <param name="volume">設定する音量</param>
    public static void SetBgmVolume(float volume)
    {
        var baseMusic = GameObject.Find("MusicRoot");
        foreach(Transform oldMusic in baseMusic.transform)
        {
            var bgmObject = oldMusic.GetComponent<BGMroot>();
            if(bgmObject == null) continue;
            bgmObject.baseVolume = volume;
        }
    }
}
