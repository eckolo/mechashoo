using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// 各ステージ動作の基底クラス
/// </summary>
public abstract partial class Stage : Methods
{
    /// <summary>
    /// 依頼者名
    /// </summary>
    [SerializeField]
    private string _requester = "依頼者名";
    public string requester
    {
        get {
            return _requester;
        }
        set {
            _requester = value;
        }
    }
    [SerializeField]
    private string _location = "依頼場所";
    protected string location
    {
        get {
            return _location;
        }
        set {
            _location = value;
        }
    }
    /// <summary>
    /// 特殊ステージフラグ
    /// </summary>
    public bool isSystem = false;

    /// <summary>
    /// 初期背景
    /// </summary>
    [SerializeField]
    protected MeshRenderer initialScenery;
    private MeshRenderer scenery = null;
    /// <summary>
    /// 初期BGM
    /// </summary>
    [SerializeField]
    protected List<AudioClip> BGMList;
    protected AudioClip initialBGM => BGMList.FirstOrDefault();

    /// <summary>
    /// ステージサイズ
    /// </summary>
    [SerializeField]
    public Vector2 fieldSize = Vector2.one;
    /// <summary>
    /// ビューの初期位置
    /// </summary>
    [SerializeField]
    protected Vector2 initialViewPosition = Vector2.zero;
    /// <summary>
    /// プレイヤー機の初期位置
    /// </summary>
    [SerializeField]
    protected Vector2 initialPlayerPosition = new Vector2(-3.6f, 0);

    /// <summary>
    /// ステージの難易度
    /// オプションからの難易度設定とか用
    /// </summary>
    [SerializeField]
    protected ulong stageLevel = 1;

    /// <summary>
    /// ステージに出てくるNPCのリスト
    /// 基本的に出現対象はここから指定する
    /// </summary>
    [SerializeField]
    protected List<Npc> enemyList = new List<Npc>();

    protected const int INTERVAL = 2400;
    protected const int INTERVAL_A_LITTLE = INTERVAL / 10;

    /// <summary>
    /// 敵機出現数
    /// </summary>
    public uint enemyAppearances { get; private set; } = 0;
    /// <summary>
    /// 最低限の撃墜数
    /// </summary>
    public uint minimumShotDown { get; private set; } = 0;
    /// <summary>
    /// 接敵回数
    /// </summary>
    public uint opposeEnemy { get; private set; } = 0;
    /// <summary>
    /// 総撃墜数
    /// </summary>
    public uint shotsToKill { get; private set; } = 0;
    /// <summary>
    /// 攻撃回数
    /// </summary>
    public uint attackCount { get; private set; } = 0;
    /// <summary>
    /// 攻撃命中回数
    /// </summary>
    public uint attackHits { get; private set; } = 0;
    /// <summary>
    /// 敵弾生成総数
    /// </summary>
    public uint enemyAttackCount { get; private set; } = 0;
    /// <summary>
    /// 被弾回数
    /// </summary>
    public uint toHitCount { get; private set; } = 0;
    /// <summary>
    /// 直撃被弾回数
    /// </summary>
    public uint toDirectHitCount { get; private set; } = 0;

    /// <summary>
    /// 敵機出現数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>敵機出現数</returns>
    public uint IncreaseEnemyAppearances(int plusCount = 1) => enemyAppearances = enemyAppearances + (uint)plusCount;
    /// <summary>
    /// 撃墜必須敵機出現数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>撃墜必須敵機出現数</returns>
    public uint IncreaseMinimumShotDown(int plusCount = 1) => minimumShotDown = minimumShotDown + (uint)plusCount;
    /// <summary>
    /// 接敵回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>敵機出現数</returns>
    public uint IncreaseOpposeEnemy(int plusCount = 1) => opposeEnemy = opposeEnemy + (uint)plusCount;
    /// <summary>
    /// 総撃墜数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>総撃墜数</returns>
    public uint IncreaseShotsToKill(int plusCount = 1) => shotsToKill = shotsToKill + (uint)plusCount;
    /// <summary>
    /// 攻撃回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>攻撃回数</returns>
    public uint IncreaseAttackCount(int plusCount = 1) => attackCount = attackCount + (uint)plusCount;
    /// <summary>
    /// 攻撃命中回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>攻撃命中回数</returns>
    public uint IncreaseAttackHits(int plusCount = 1) => attackHits = attackHits + (uint)plusCount;
    /// <summary>
    /// 敵弾生成総数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>敵弾生成総数</returns>
    public uint IncreaseEnemyAttackCount(int plusCount = 1) => enemyAttackCount = enemyAttackCount + (uint)plusCount;
    /// <summary>
    /// 被弾回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>被弾回数</returns>
    public uint IncreaseToHitCount(int plusCount = 1) => toHitCount = toHitCount + (uint)plusCount;
    /// <summary>
    /// 直撃被弾回数カウント関数
    /// </summary>
    /// <param name="plusCount">カウント増加数</param>
    /// <returns>直撃被弾回数</returns>
    public uint IncreaseToDirectHitCount(int plusCount = 1) => toDirectHitCount = toDirectHitCount + (uint)plusCount;

    /// <summary>
    /// ステージアクションのコルーチンを所持する変数
    /// </summary>
    public Coroutine nowStageAction = null;
    /// <summary>
    /// 各部のステージアクションのコルーチンを所持する変数
    /// </summary>
    public Coroutine nowStageActionMain = null;

    [SerializeField]
    private List<RewardWeapon> rewardWeapons = new List<RewardWeapon>();
    [SerializeField]
    private List<RewardShip> rewardShips = new List<RewardShip>();

    public Weapon GetRewardWeaponData(RewardTermType termType)
        => rewardWeapons.FirstOrDefault(weapon => weapon.termType == termType)?.entity;
    public Ship GetRewardShipData(RewardTermType termType)
        => rewardShips.FirstOrDefault(ship => ship.termType == termType)?.entity;

    /// <summary>
    /// 報酬条件表示の更新
    /// </summary>
    /// <param name="originText">報酬条件文元オブジェクト</param>
    /// <returns>報酬条件文</returns>
    Text UpdateRewardText(Text originText = null)
    {
        const int baseCharSize = 24;
        var weaponRewardData = rewardWeapons
            .Select(reward => new
            {
                text = $"*{reward.termData.explanation}",
                isPossessed = reward.isPossessed,
                termType = reward.termType
            });
        var shipRewardData = rewardShips
            .Select(reward => new
            {
                text = $"*{reward.termData.explanation}",
                isPossessed = reward.isPossessed,
                termType = reward.termType
            });
        if(!weaponRewardData.Any() && !shipRewardData.Any()) return SetSysText("");
        var messageRewards = weaponRewardData
            .Concat(shipRewardData)
            .OrderBy(reward => reward.termType)
            .Select(data => data.isPossessed ? $"<color=grey>{data.text}</color>" : data.text)
            .Aggregate((explanation1, explanation2) => $"{explanation1}\r\n{explanation2}");
        return SetSysText(setText: $"\r\n<size={baseCharSize + 1}>達成目標</size>\r\n{messageRewards}",
             position: Vector2.up * viewSize.y * baseMas.y / 2,
             pivot: TextAnchor.UpperCenter,
             charSize: baseCharSize,
             defaultText: originText);
    }
    /// <summary>
    /// ステージクリア報酬処理
    /// </summary>
    /// <returns>イテレータ</returns>
    IEnumerator ObtainRewardAction()
    {
        if(!rewardWeapons.Any() && !rewardShips.Any()) yield break;

        SetScenery(sys.baseObjects.darkScene);
        MainSystems.SetBGM();
        var text = UpdateRewardText();

        yield return Fadein(0);

        var notPossessedWeapons = rewardWeapons
            .Where(weapon => !weapon.isPossessed);
        var notPossessedShips = rewardShips
            .Where(ship => !ship.isPossessed);
        var getableWeapons = notPossessedWeapons
            .Where(weapon => weapon.termData.term());
        var getableShips = notPossessedShips
            .Where(ship => ship.termData.term());

        yield return WaitMessages("人工頭脳", new[] { "本依頼の達成目標一覧はこちらです。" }, false);
        if(!notPossessedWeapons.Any() && !notPossessedShips.Any())
        {
            yield return WaitMessages("人工頭脳", new[] { "おっと、全て達成済みでしたか。" }, false);
        }
        else if(!getableWeapons.Any() && !getableShips.Any())
        {
            yield return WaitMessages("人工頭脳", new[] { "…特に新規達成した達成目標はありませんね。" }, false);
        }
        else
        {
            foreach(var weapon in getableWeapons)
            {
                var successful = sys.ObtainArticles(weapon.entity);
                if(successful)
                {
                    text = UpdateRewardText(text);
                    yield return WaitMessages("人工頭脳", new[] {
                        $"達成目標「{weapon.termData.explanation}」を達成。",
                        $"達成報酬「{weapon.entity.abbreviation}」を入手しました。"
                    }, false);
                }
            }
            foreach(var ship in getableShips)
            {
                var successful = sys.ObtainArticles(ship.entity);
                if(successful)
                {
                    text = UpdateRewardText(text);
                    yield return WaitMessages("人工頭脳", new[] {
                        $"達成目標「{ship.termData.explanation}」を達成。",
                        $"達成報酬「{ship.entity.abbreviation}」を入手しました。"
                    }, false);
                }
            }
        }

        for(int time = 0; time < Configs.DEFAULT_FADE_TIME; time++)
        {
            var setedAlpha = Easing.quadratic.SubOut(time, Configs.DEFAULT_FADE_TIME - 1);
            text.SetAlpha(setedAlpha);
            yield return Wait(1);
        }
        yield return Fadeout(0);
        DeleteSysText(text);
        yield break;
    }

    /// <summary>
    /// 通信文章のデフォルト表示位置
    /// </summary>
    protected static Vector2 mainTextPosition => Vector2.down * viewSize.y * baseMas.y / 4;

    public bool isCleared
    {
        get {
            return sys.GetClearFlug(this);
        }
    }
    public virtual bool ableChoice
    {
        get {
            return true;
        }
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        MainSystems.SetBGM(initialBGM);
        SetScenery();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if(nowStageAction != null && !isContinue) EndStageProcess();
        if(Configs.Buttom.Menu.Judge()) StartCoroutine(PauseMenu());
        if(scenery != null) scenery.transform.localPosition = viewPosition / 2;
    }

    /// <summary>
    /// ステージ選択可能フラグ
    /// </summary>
    public virtual bool challengeable => false;

    Text locationText = null;
    /// <summary>
    /// 場所の表示
    /// </summary>
    /// <param name="locationName">場所名</param>
    /// <returns>イテレータ</returns>
    protected IEnumerator DisplayLocation(string locationName)
    {
        var basePosition = viewPosition + viewSize.Scaling(-0.5f, 0.5f).Scaling(baseMas);
        locationText = SetSysText(locationName, basePosition, TextAnchor.UpperLeft, Configs.Texts.CHAR_SIZE * 2);
        locationText.SetAlpha(0);

        var movementWidth = locationText.AreaSize().x;
        var timelimit = Configs.DEFAULT_FADE_TIME;
        for(int time = 0; time < timelimit; time++)
        {
            if(locationText == null) yield break;
            var xTweak = Easing.quadratic.SubIn(movementWidth, time, timelimit - 1);
            var setAlpha = Easing.quadratic.Out(time, timelimit - 1);
            locationText.SetPosition(basePosition + Vector2.right * xTweak);
            locationText.SetAlpha(setAlpha);
            yield return Wait(1);
        }
        yield return Wait(timelimit);
        for(int time = 0; time < timelimit; time++)
        {
            if(locationText == null) yield break;
            var xTweak = Easing.quadratic.In(-movementWidth, time, timelimit - 1);
            var setAlpha = Easing.quadratic.SubIn(time, timelimit - 1);
            locationText.SetPosition(basePosition + Vector2.right * xTweak);
            locationText.SetAlpha(setAlpha);
            yield return Wait(1);
        }

        DeleteSysText(locationText);
        locationText = null;
        yield break;
    }

    /// <summary>
    /// ポーズメニューアクション
    /// </summary>
    IEnumerator PauseMenu()
    {
        if(onPause) yield break;
        if(isSystem) yield break;
        if(!sysPlayer.canRecieveKey) yield break;

        SwitchPause(true);
        var pauseDarkTone = PutDarkTone(0.8f);

        bool withdraw = false;
        yield return ChoiceAction(new List<string> { "継続", "撤退" },
            endProcess: result => withdraw = result == 1,
            ableCancel: true,
            pivot: TextAnchor.MiddleCenter);
        DeleteChoices();

        if(withdraw) TerminateStage();
        pauseDarkTone.DestroyMyself();
        SwitchPause(false);
        yield break;
    }

    /// <summary>
    /// ステージ動作の強制終了
    /// </summary>
    void TerminateStage()
    {
        sys.nextStage = sys.mainMenu;
        DeleteSysText(locationText);
        locationText = null;
        isContinue = false;
    }

    bool _isContinue = true;
    protected bool isContinue
    {
        get {
            if(isSuccess || isFault) return false;
            return _isContinue;
        }
        set {
            _isContinue = value;
        }
    }
    bool _isSuccess = false;
    protected bool isSuccess
    {
        get {
            if(isFault) return false;
            return _isSuccess;
        }
        set {
            _isSuccess = value;
        }
    }
    bool _isFault = false;
    protected bool isFault
    {
        get {
            if(sysPlayer != null && sysPlayer.isExist && !sysPlayer.isAlive) return true;
            return _isFault;
        }
        set {
            _isFault = value;
        }
    }
    public virtual void StartStageProcess()
    {
        Debug.Log($"Start Stage {this}.");
        VisualizePlayer();
        sysPlayer.position = initialPlayerPosition;
        if(!isSystem) sysPlayer.DeleteArmorBar();

        nowStageAction = StartCoroutine(BaseStageAction());
    }
    protected void EndStageProcess()
    {
        if(nowStageActionMain != null) StopCoroutine(nowStageActionMain);
        if(nowStageAction != null) StopCoroutine(nowStageAction);
        sysPlayer.canRecieveKey = false;
        nowStageAction = null;
        if(sys.playerHPbar != null) sys.playerHPbar.DestroyMyself();
        if(sys.playerBRbar != null) sys.playerBRbar.DestroyMyself();
        if(sys.playerENbar != null) sys.playerENbar.DestroyMyself();
        StartCoroutine(EndStageAction());
    }
    protected IEnumerator EndStageAction()
    {
        if(isFault) yield return FaultAction();
        if(isSuccess) yield return SuccessAction();
        if(!isSystem)
        {
            yield return Fadeout();
            if(isSuccess)
            {
                sys.SetClearFlug(this);
                yield return DisplayResult();
            }
        }

        ResetView();
        DestroyAll(true);
        if(scenery != null) Destroy(scenery.gameObject);
        SaveData.SetInt(Configs.SaveKeys.STORY_PHASE, (int)sys.storyPhase);
        SaveData.Save();

        isContinue = true;
        DestroyMyself();
        sys.StartSystem();
        yield break;
    }

    protected IEnumerator DisplayResult()
    {
        SetScenery(sys.baseObjects.darkScene);
        yield return Fadein(0);
        MainSystems.SetBGM();

        var message = $@"
戦果報告

撃墜率:{sys.shotDownRate.ToPercentage()}
命中率:{sys.accuracy.ToPercentage()}
回避率:{sys.evasionRate.ToPercentage()}
防御率:{sys.protectionRate.ToPercentage()}";

        yield return sys.SetMainWindow(message, 24, Key.Set.decide, Configs.Texts.CHAR_SIZE * 3, Vector2.up * viewSize.y * baseMas.y / 2, TextAnchor.UpperCenter, false);
        yield return WaitMessages("人工頭脳", sys.GetAiComment(), false);

        for(int time = 0; time < Configs.DEFAULT_FADE_TIME; time++)
        {
            var setedAlpha = Easing.quadratic.SubOut(time, Configs.DEFAULT_FADE_TIME - 1);
            sys.mainText.SetAlpha(setedAlpha);
            yield return Wait(1);
        }
        yield return Fadeout(0);
        yield return sys.SetMainWindow("", 0);
        yield return ObtainRewardAction();
        yield break;
    }

    protected virtual IEnumerator OpeningAction()
    {
        yield break;
    }
    protected virtual IEnumerator StageAction()
    {
        yield break;
    }
    protected virtual bool isComplete { get { return !allEnemyObjects.Any(); } }
    protected virtual IEnumerator SuccessAction()
    {
        var text = SetWindowWithText(SetSysText("Success", charSize: 24));
        yield return Wait(12000, Key.Set.decide);
        text.DestroyMyself();
        yield break;
    }
    protected virtual IEnumerator FaultAction()
    {
        var text = SetWindowWithText(SetSysText("撃沈", charSize: 42, textColor: new Color(1, 0.1f, 0.1f)));
        var limit = 2000;
        for(int time = 0; time < limit; time++)
        {
            text.nowAlpha = Easing.quintic.Out(time, limit - 1);
            MainSystems.SetBgmVolume(Easing.quintic.SubOut(time, limit - 1));
            if(Key.Set.decide.Judge()) break;
            yield return Wait(1);
        }
        text.nowAlpha = 1;
        MainSystems.SetBGM();
        yield return Wait(10000, Key.Set.decide);
        text.DestroyMyself();
        yield break;
    }

    public Vector2 ResetView()
    {
        return viewPosition = initialViewPosition;
    }

    private IEnumerator BaseStageAction()
    {
        if(!isSystem) yield return Fadein();
        yield return OpeningAction();

        if(!isSystem)
        {
            sysPlayer.DeleteArmorBar();
            sysPlayer.SetArmorBar();
            sysPlayer.canRecieveKey = true;
            sysPlayer.ResetAllAlignment(viewPosition - sysPlayer.position - CorrectWidthVector(sysPlayer.armRoot));

            sys.playerHPbar = GetBar(BarType.HP, Color.red);
            sys.playerBRbar = GetBar(BarType.BR, Color.cyan);
            sys.playerENbar = GetBar(BarType.EN, Color.yellow);
            sys.playerHPbar.nowSort = Configs.SortLayers.SYSTEM_STATE;
            sys.playerBRbar.nowSort = Configs.SortLayers.SYSTEM_STATE;
            sys.playerENbar.nowSort = Configs.SortLayers.SYSTEM_STATE;
        }

        nowStageActionMain = StartCoroutine(StageAction());
        yield return nowStageActionMain;

        yield return Wait(() => isComplete);
        sysPlayer.canRecieveKey = false;
        isSuccess = true;
    }

    /// <summary>
    /// 全敵性機体リスト
    /// </summary>
    protected static List<Npc> allEnemies => GetAllObject<Npc>(target => target.nowLayer != sysPlayer.nowLayer);
    /// <summary>
    /// 画面内の全敵性機体リスト
    /// </summary>
    protected static List<Npc> allEnemiesInField => allEnemies.Where(target => target.inField).ToList();
    /// <summary>
    /// 全敵性物体リスト
    /// </summary>
    protected static List<Things> allEnemyObjects => GetAllObject<Things>(target => target.nowLayer != sysPlayer.nowLayer);
    /// <summary>
    /// 画面内の全敵性物体リスト
    /// </summary>
    protected static List<Things> allEnemyObjectsInField => allEnemyObjects.Where(target => target.inField).ToList();

    /// <summary>
    /// 1波クリアを待つ
    /// </summary>
    /// <param name="interval">1波の時間制限</param>
    /// <returns>コルーチン</returns>
    protected IEnumerator WaitWave(int interval = 0)
    {
        yield return Wait(interval / 10);
        yield return Wait(() => allEnemiesInField.Any());
        if(interval > 0) yield return Wait(interval, () => !allEnemiesInField.Any());
        else yield return Wait(() => !allEnemiesInField.Any());
        yield break;
    }
    /// <summary>
    /// 呼び出し音鳴らすだけ
    /// </summary>
    /// <param name="callTimes">鳴る回数</param>
    /// <returns>イテレータ</returns>
    protected IEnumerator SoundCall(int callTimes)
    {
        for(int count = 0; count < callTimes; count++)
        {
            var sound = SoundSE(sys.ses.callSE, pitch: 2, isSystem: true);
            yield return Wait(() => sound == null);
        }
        yield break;
    }
    /// <summary>
    /// ステージ中でのメッセージ表示と待機
    /// </summary>
    /// <param name="message">表示メッセージ</param>
    /// <returns>コルーチン</returns>
    protected IEnumerator WaitMessages(string speaker, IEnumerable<string> messages, bool callSound = true)
    {
        var originCanRecieveKey = sysPlayer.canRecieveKey;
        sysPlayer.canRecieveKey = false;

        if(callSound) yield return SoundCall(2);

        for(int index = 0; index < messages.Count(); index++)
        {
            yield return WaitMessage(messages.ToArray()[index], speaker);
        }

        sysPlayer.canRecieveKey = originCanRecieveKey;
        yield break;
    }
    /// <summary>
    /// ステージ中でのメッセージ表示と待機
    /// </summary>
    /// <param name="message">表示メッセージ</param>
    /// <returns>コルーチン</returns>
    protected IEnumerator WaitMessage(string message, string speaker = null)
    {
        if(nextDestroy) yield break;
        if(sys.nowStage != this) yield break;
        var window = SetWindowWithText(SetSysText(message, mainTextPosition, charSize: Configs.Texts.CHAR_SIZE + 1));
        var nameWindow = speaker != null
            ? SetWindowWithText(SetSysText(speaker,
            new Vector2(window.underLeft.x, window.upperRight.y),
            TextAnchor.LowerLeft,
            Configs.Texts.CHAR_SIZE - 1,
            bold: true), 0)
            : null;
        yield return Wait(() => Key.Set.decide.Judge(Key.Timing.OFF));
        yield return Wait(() => Key.Set.decide.Judge());
        window.DestroyMyself(system: true);
        nameWindow?.DestroyMyself(false, system: true);
        yield break;
    }

    /// <summary>
    /// 警告演出
    /// </summary>
    /// <param name="timeRequired">所要時間</param>
    /// <returns>コルーチン</returns>
    protected IEnumerator ProduceWarnings(int timeRequired, int alertNum = 3)
    {
        var effectListCenter = new List<Effect>();
        var effectListUpside = new List<Effect>();
        var effectListLowside = new List<Effect>();

        var redTone = PutColorTone(Color.red, 0);

        var half = timeRequired / 2;
        var verticalDiff = Vector2.up * viewSize.y / 3;
        for(int time = 0; time < timeRequired; time++)
        {
            var sideDiff = Vector2.right * time / 10;

            effectListCenter.SetStrip(sys.baseObjects.warningEffect, Vector2.zero + sideDiff, 2);
            effectListUpside.SetStrip(sys.baseObjects.warningEffect, Vector2.zero + verticalDiff - sideDiff);
            effectListLowside.SetStrip(sys.baseObjects.warningEffect, Vector2.zero - verticalDiff - sideDiff);

            var setAlpha = time < half
                ? Easing.quadratic.Out(time, half)
                : Easing.quadratic.SubOut(time - half, timeRequired - half);
            foreach(var effect in effectListCenter) effect.nowAlpha = setAlpha;
            foreach(var effect in effectListUpside) effect.nowAlpha = setAlpha;
            foreach(var effect in effectListLowside) effect.nowAlpha = setAlpha;

            var max = alertNum * 2 - 1;
            var once = timeRequired / alertNum / 2;
            var phase = Mathf.Min(time / once, max);
            var start = phase * once;
            var end = phase == max ? timeRequired - start : once;
            var toneTime = time - start;
            var toneAlpha = phase % 2 == 0
                ? Easing.quadratic.InOut(toneTime, end)
                : Easing.quadratic.SubInOut(toneTime, end);

            if(phase % 2 == 0 && toneTime == 0) SoundSE(sys.ses.alertSE, pitch: 1.5f, isSystem: true);
            redTone.nowAlpha = toneAlpha;

            yield return Wait(1);
        }

        foreach(var effect in effectListCenter) effect.DestroyMyself();
        foreach(var effect in effectListUpside) effect.DestroyMyself();
        foreach(var effect in effectListLowside) effect.DestroyMyself();
        redTone.DestroyMyself();
        yield break;
    }

    /// <summary>
    /// オブジェクト配置関数
    /// </summary>
    protected Things SetObject(Things obj, Vector2 coordinate)
    {
        if(nextDestroy) return null;
        if(sys.nowStage != this) return null;
        Debug.Log($"{this}\t: {obj}\t: {coordinate}");

        Vector2 precisionCoordinate = fieldArea.Scaling(coordinate) / 2;

        var newObject = Instantiate(obj);
        newObject.nowParent = sysPanel.transform;
        newObject.position = precisionCoordinate;

        return newObject;
    }
    /// <summary>
    /// NPC機体配置関数
    /// </summary>
    protected Npc SetEnemy(Npc npc,
        Vector2 coordinate,
        float? normalCourseAngle = null,
        ulong? levelTweak = null,
        int? activityLimit = null,
        bool onTheWay = true,
        string setLayer = Configs.Layers.ENEMY)
    {
        if(npc == null) return null;
        var setedNpc = (Npc)SetObject(npc, coordinate);
        if(setedNpc == null) return null;

        setedNpc.normalCourse = normalCourseAngle?.ToVector() ?? Vector2.left;
        setedNpc.shipLevel = stageLevel * (levelTweak ?? 1);
        setedNpc.activityLimit = activityLimit ?? 0;
        setedNpc.nowLayer = setLayer;
        setedNpc.onTheWay = onTheWay;

        sys.CountEnemyAppearances();
        if(!onTheWay && activityLimit == null) sys.CountMinimumShotDown();

        return setedNpc;
    }
    /// <summary>
    /// NPC機体配置関数
    /// </summary>
    protected Npc SetEnemy(int npcIndex,
        Vector2 coordinate,
        float? normalCourseAngle = null,
        ulong? levelTweak = null,
        int? activityLimit = null,
        bool onTheWay = true,
        string setLayer = Configs.Layers.ENEMY)
    {
        if(npcIndex < 0) return null;
        if(npcIndex >= enemyList.Count) return null;

        var setedNpc = SetEnemy(enemyList[npcIndex], coordinate, normalCourseAngle, levelTweak, activityLimit, onTheWay, setLayer);
        if(setedNpc?.privateBgm != null) MainSystems.SetBGM(setedNpc.privateBgm);
        return setedNpc;
    }
    /// <summary>
    /// NPC機体配置関数
    /// </summary>
    protected Npc SetEnemy(int npcIndex,
        float coordinateX,
        float coordinateY,
        float? normalCourseAngle = null,
        ulong? levelTweak = null,
        int? activityLimit = null,
        bool onTheWay = true,
        string setLayer = Configs.Layers.ENEMY)
        => SetEnemy(npcIndex, new Vector2(coordinateX, coordinateY), normalCourseAngle, levelTweak, activityLimit, onTheWay, setLayer);
    /// <summary>
    /// 背景設定関数
    /// 初期値はStageの初期背景
    /// </summary>
    protected MeshRenderer SetScenery(MeshRenderer buckGround = null)
    {
        var setBuckGround = (buckGround ?? initialScenery);
        if(setBuckGround == null) return null;

        var baseScenery = GameObject.Find("SceneryRoot");
        foreach(Transform oldScenery in baseScenery.transform)
        {
            Destroy(oldScenery.gameObject);
        }
        scenery = Instantiate(setBuckGround.gameObject, Vector3.forward, transform.rotation).GetComponent<MeshRenderer>();
        scenery.transform.localScale = fieldArea;
        scenery.transform.parent = baseScenery.transform;

        return scenery;
    }
}
