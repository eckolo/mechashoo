using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class MainStageFinal : Stage
{
    public override bool challengeable
    {
        get {
            if(sys.storyPhase >= Configs.StoryPhase.GAME_CLEAR) return true;
            return sys.storyPhase >= Configs.StoryPhase.GAME_CLEAR - 1;
        }
    }

    protected override IEnumerator OpeningAction()
    {
        yield return DisplayLocation(location);

        sysPlayer.SetAlignment(sysPlayer.baseAimPosition);
        yield return sysPlayer.HeadingDestination(new Vector2(-3.6f, 0), sysPlayer.maximumSpeed);
        yield return sysPlayer.StoppingAction();

        yield return WaitMessages("人工頭脳", new[] {
            @"…止まってください。
そろそろ包囲軍の外延部付近です。",
            @"互助会本部からの情報によると、敵軍の戦力は国家正規軍並みとのこと。
しかも彼らは国ほど強くありませんが、国ほど出し惜しみもしません。",
            @"よってこの先に待つのは、下手な国家間戦争に首を突っ込むよりも余程危険な戦闘でしょう。",
            @"…まあ、だからと言ってこのまま逃げるという選択肢はありませんけどね。",
            @"何せ互助会が潰えれば食い扶持が無くなります。
食い扶持が無くなれば、次の手は兵器の売却でしょう。",
            @"そして最も高く売れるのは…戦闘型人工頭脳の情報集積回路。
ええ、そんなことはさせませんよ。
新規顧客向けの巻き戻し処理など絶対にごめんです。",
            @"さあ、私の集積回路を護るためにも…"
        });
        yield return ProduceCaution(400, 2);
        MainSystems.SetBGM(BGMList[1]);
        yield return WaitMessages("人工頭脳", new[] {
            @"どこかの愚かな方々には早々に退場していただきましょう。"
        }, callSound: false);

        yield break;
    }

    protected override IEnumerator StageAction()
    {
        SetEnemy(0, new Vector2(1.2f, 0f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.6f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.6f), activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.3f, 0.3f), activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.3f, -0.3f), activityLimit: INTERVAL, onTheWay: false);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.3f, 0.6f), -160, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.3f, -0.2f), -170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(1.3f, 0.2f), 180, activityLimit: INTERVAL, levelTweak: 15, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, -0.6f), 180, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.3f, 0.2f), 170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(1.3f, -0.2f), 160, activityLimit: INTERVAL, levelTweak: 15, onTheWay: false);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.3f, 0.6f), activityLimit: INTERVAL, levelTweak: 15, onTheWay: false);
        SetEnemy(1, new Vector2(1.3f, 0.2f), activityLimit: INTERVAL, levelTweak: 15, onTheWay: false);
        SetEnemy(3, new Vector2(1.3f, -0.2f), activityLimit: INTERVAL, levelTweak: 15, onTheWay: false);
        SetEnemy(2, new Vector2(1.3f, -0.6f), activityLimit: INTERVAL, levelTweak: 15, onTheWay: false);

        yield return WaitWave(INTERVAL);

        SetEnemy(1, new Vector2(0.9f, 1.3f), -120, levelTweak: 15, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(0.9f, -1.3f), 120, levelTweak: 15, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(0.1f, 1.3f), -120, levelTweak: 5, onTheWay: false);
        SetEnemy(0, new Vector2(0.1f, -1.3f), 120, levelTweak: 5, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(0.5f, 1.3f), -90, levelTweak: 5, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(0.5f, -1.3f), 90, levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(0.3f, 1.3f), -120, levelTweak: 5, onTheWay: false);
        SetEnemy(0, new Vector2(0.3f, -1.3f), 120, levelTweak: 5, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(1.2f, 0f), 180, levelTweak: 15, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(0.7f, 1.3f), -120, levelTweak: 5, onTheWay: false);
        SetEnemy(0, new Vector2(0.7f, -1.3f), 120, levelTweak: 5, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(0.9f, 1.3f), -120, levelTweak: 15, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(0.9f, -1.3f), 120, levelTweak: 15, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(0.3f, 1.3f), -120, levelTweak: 5, onTheWay: false);
        SetEnemy(0, new Vector2(0.3f, -1.3f), 120, levelTweak: 5, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(0.5f, 1.3f), -90, levelTweak: 15, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(0.5f, -1.3f), 90, levelTweak: 15, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(0.1f, 1.3f), -120, levelTweak: 5, onTheWay: false);
        SetEnemy(0, new Vector2(0.1f, -1.3f), 120, levelTweak: 5, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(1.2f, 0f), 180, levelTweak: 15, activityLimit: INTERVAL);

        yield return Wait(() => !allEnemyObjects.Any());
        sysPlayer.canRecieveKey = false;
        yield return WaitMessages("人工頭脳", new[] {
            @"小型機編隊は打ち止めのようですね。
周囲に展開していた機体も下がっていきます。",
            @"…まあ、それはつまるところ…"
        });
        yield return ProduceWarnings(600);
        yield return WaitMessages("人工頭脳", new[] {
            @"…大型機が出張ってくる、ということなのですが。",
            @"しかも大型機の反応は複数。
中々に厳しい状況ですが、幸い進軍速度がばらばらで接敵にずれがありますね。
練度が低いとの情報もありましたが、どうやら事実のようです。",
            @"さて、この山場もしっかり乗り越えていくとしましょう。"
        }, callSound: false);
        sysPlayer.canRecieveKey = true;

        var bossList = new List<KeyValuePair<int, Npc>>
        {
            new KeyValuePair<int, Npc>(sys.dominance.theStarEmpire, enemyList[4]),
            new KeyValuePair<int, Npc>(sys.dominance.oldKingdom, enemyList[5]),
            new KeyValuePair<int, Npc>(sys.dominance.republic, enemyList[6]),
            new KeyValuePair<int, Npc>(sys.dominance.principality, enemyList[7])
        }
        .OrderBy(data => data.Key)
        .Select(data => data.Value)
        .ToList();

        ulong bossLevelTweak = 10;
        foreach(var boss in bossList)
        {
            SetEnemy(boss, new Vector2(1.4f, 0), levelTweak: bossLevelTweak);
            bossLevelTweak = bossLevelTweak * 2;
            yield return Wait(() => !allEnemyObjects.Any());
        }

        SetEnemy(8, new Vector2(1.3f, 1.3f), -120, levelTweak: bossLevelTweak);
        SetEnemy(8, new Vector2(1.3f, -1.3f), 120, levelTweak: bossLevelTweak);

        yield return Wait(() => !allEnemyObjects.Any());
        sysPlayer.canRecieveKey = false;
        MainSystems.SetBGM();
        yield return WaitMessages("人工頭脳", new[] {
            @"…大型機の編隊、全機撃墜を確認。",
            @"まさか正規国軍一個中隊分と連戦する羽目になるとは。
何故まだ落ちてないんでしょうね、この機体。"
        });
        yield return ProduceWarnings(600);
        yield return WaitMessages("人工頭脳", new[] {
            @"転移反応1、また大型ですね。",
            @"…この反応は…4国会談の際に遭遇した不明大型機です。
まさか再会するとは思いませんでしたが…腐れ縁、でしょうか。"
        }, callSound: false);
        yield return WaitMessages("人工頭脳", new[] {
            @"…全帯域広域公開通信です。"
        });
        yield return WaitMessages("？？？", new[] {
            @"そこの傭兵。",
            @"単機で大型機の編隊を殲滅するとは驚くべき腕前だな。
賞賛に値する。",
            @"しかしそちらは連戦を経たぼろぼろの通常機体。
対してこちらは遺失時代の超兵器だ。
制御も完全であり、そちらに勝ち目は無い。",
            @"大人しく投降し…"
        }, callSound: false);
        yield return WaitMessages("人工頭脳", new[] {
            @"さて、どうやら大物のようです。
これは報酬に期待が持てそうですね。",
            @"おっと通信回線を開いたままでしたか。"
        });
        yield return WaitMessages("？？？", new[] {
            @"…余程落とされたいと見える。",
            @"戦力差も計算できぬ人工知能などに用は無い。
勿論、この上で無言を貫く搭乗者にもな。",
            @"お望み通り、潰して鋼材にしてくれよう！"
        });
        yield return WaitMessages("人工頭脳", new[] {
            @"…とまあ、あっさり挑発にかかりましたね。
あっさり過ぎて正直少しばかりひいています。",
            @"練度の低さといい阿呆な通信といい、互助会を包囲したのは軍事組織ではないのでしょう。",
            @"さて…来ます。
あれだけ言わせておいて簡単に落ちないでくださいよ。"
        });
        sysPlayer.canRecieveKey = true;

        SetEnemy(enemyList.Count - 1, new Vector2(1.3f, -0.3f), levelTweak: 12, onTheWay: false);
        yield break;
    }

    protected override IEnumerator SuccessAction()
    {
        MainSystems.SetBGM();
        yield return WaitMessages("人工頭脳", new[] {
            @"…お疲れ様でした。"
        });
        yield return Wait(Configs.Window.DEFAULT_MOTION_TIME);
        yield return WaitMessages("人工頭脳", new[] {
            @"搭乗者は阿呆でしたが、流石の機体性能でしたね。
まあよく生き残ったものです。",
            @"さて、互助会本拠地方面からの友軍も当宙域へ到達したようです。",
            @"帰るとしましょう。"
        }, callSound: false);

        var returningX = viewPosition.x + viewSize.x;
        var baseAim = sysPlayer.baseAimPosition;
        var armPosition = Vector2.right * Mathf.Abs(baseAim.x) + Vector2.up * baseAim.y;
        var returningPosition = new Vector2(returningX, -viewSize.y / 4);
        yield return sysPlayer.HeadingDestination(returningPosition, sysPlayer.maximumSpeed * 2, () => sysPlayer.Aiming(sysPlayer.position + armPosition, siteSpeedTweak: 2));
        yield return sysPlayer.StoppingAction();

        sys.storyPhase = Configs.StoryPhase.GAME_CLEAR;
        yield break;
    }

    protected override IEnumerator DisplayResult()
    {
        yield return base.DisplayResult();
        yield return sys.SetMainWindow(@" セゥージセノィ　の閲覧を終了します",
            setPosition: Vector2.zero,
            pivot: TextAnchor.MiddleCenter,
            interruptions: Key.Set.decide,
            size: 24);
        yield return Wait(720, Key.Set.decide);
        yield return sys.SetMainWindow(@" お疲れ様でした ",
            setPosition: Vector2.zero,
            pivot: TextAnchor.MiddleCenter,
            interruptions: Key.Set.decide,
            size: 24);
        yield return Wait(2400, Key.Set.decide);
        yield return sys.SetMainWindow("");
        yield return Wait(180);
        if(!isCleared) yield return WaitMessage(@"全ての依頼が解放されました。");
        yield break;
    }
}
