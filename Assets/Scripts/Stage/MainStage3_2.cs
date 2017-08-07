using UnityEngine;
using System.Collections;
using System.Linq;

public class MainStage3_2 : Stage
{
    public override bool challengeable
    {
        get {
            if(sys.storyPhase >= Configs.StoryPhase.GAME_CLEAR) return true;
            var stage2 = sys.stages
                .SingleOrDefault(stage => stage.GetComponent<MainStage2>() != null)
                .GetComponent<MainStage2>();
            var checkWeapon = stage2.GetRewardWeaponData(RewardTermType.MOST_SHOOTING_DOWN);
            var checkShip = stage2.GetRewardShipData(RewardTermType.MOST_SHOOTING_DOWN);
            if(checkWeapon != null && !sys.possessionWeapons.Contains(checkWeapon)) return false;
            if(checkShip != null && !sys.possessionShips.Contains(checkShip)) return false;
            return sys.storyPhase >= 3;
        }
    }

    protected override IEnumerator OpeningAction()
    {
        yield return DisplayLocation(location);

        yield return sysPlayer.HeadingDestination(new Vector2(-3.6f, 0), sysPlayer.maximumSpeed);
        yield return sysPlayer.StoppingAction();
        yield return Wait(Configs.Window.DEFAULT_MOTION_TIME);

        yield return WaitMessages("人工頭脳", new[] {
            @"司令部へ連絡。",
            @"定時確認報告、索敵範囲内に敵影無し。
定刻まで現地点にて待機します。"
        });
        yield return Wait(Configs.Window.DEFAULT_MOTION_TIME);
        yield return WaitMessages("護衛軍司令部", new[] {
            @"定時確認了解。
警戒任務を続行されたし。"
        });
        yield return Wait(Configs.Window.DEFAULT_MOTION_TIME);
        yield return WaitMessages("人工頭脳", new[] {
            @"警戒任務続行了解。
通信を終了します。"
        });
        yield return Wait(INTERVAL_A_LITTLE);
        yield return WaitMessages("人工頭脳", new[] {
            @"…この定時確認、不要ではないでしょうか。",
            @"計器情報をそのまま送信すれば、より詳細な情報の取得は容易です。
また平時から人員を稼働させるというのも褒められた所業ではありません。",
            @"加えて演算能力の不要な連絡業務に人工知能が従事している事実。
これは人工頭脳の過剰労働に該当すると考えられます。",
            @"…サボりたいとの意図ではありません。
様式に従うためだけの無駄な手続きに対する意見具申です。
決して、過剰というほど演算していないなどということも勿論…"
        }, callSound: false);
        yield return ProduceCaution(400, 2);
        MainSystems.SetBGM(BGMList[1]);
        yield return WaitMessages("人工頭脳", new[] {
            @"…反応有り、戦闘機体多数。",
            @"識別信号照会、護衛軍管轄外であることを確認。
十中八九襲撃者でしょう。",
            @"さて、お待ちかねの仕事の時間です。
労働の喜びを胸にきりきり働くとしましょう。"
        }, callSound: false);

        yield break;
    }

    protected override IEnumerator StageAction()
    {
        SetEnemy(1, new Vector2(1.2f, 0.3f), levelTweak: 5, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.7f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.5f), activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(1, new Vector2(1.3f, 0.4f), 190, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.2f, -0.6f), 170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.1f), activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.2f, 0.8f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.3f, -0.3f), 200, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, 0.5f), 190, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, -0.7f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, 0.1f), 160, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -0.3f), 170, activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(1, new Vector2(1.3f, 0.5f), 200, levelTweak: 5);
        SetEnemy(1, new Vector2(1.2f, -0.2f), levelTweak: 5);
        SetEnemy(2, new Vector2(1.4f, 0.1f), 190, levelTweak: 10, onTheWay: false);

        yield return Wait(() => !allEnemyObjects.Any());
        yield return WaitMessages("人工頭脳", new[] {
            @"さらに敵機の反応多数。",
            @"どうやら別動隊の進軍航路を引き当てたようです。
流石のクジ運ですね。",
            @"しっかり数を減らすとしましょう。"
        });

        SetEnemy(0, new Vector2(1.3f, 0.5f), 185, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.6f), 186, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, -0.9f), 171, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, -0.3f), 177, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.8f), 188, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, 0.5f), 185, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, -0.7f), 173, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.2f), 178, activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.2f, 0.5f), activityLimit: INTERVAL, levelTweak: 5, onTheWay: false);
        SetEnemy(0, new Vector2(1.2f, -0.7f), activityLimit: INTERVAL, levelTweak: 5, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -1.3f), 135);
        SetEnemy(1, new Vector2(1.2f, 1.4f), -135);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(-0.6f, 1.3f), -150);
        SetEnemy(2, new Vector2(-0.7f, -1.2f), 150);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, -0.2f), levelTweak: 10, onTheWay: false);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.3f, -0.25f), 175, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.2f), 176, levelTweak: 2, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.3f, -0.45f), 171, levelTweak: 3, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, 0.35f), 187, levelTweak: 4, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.1f), 178, levelTweak: 5, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.3f, 0.25f), 185, levelTweak: 6, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, 0.15f), 183, levelTweak: 7, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, 0.4f), 188, levelTweak: 8, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.3f, -0.45f), 171, levelTweak: 9, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, -0.05f), 179, levelTweak: 10, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, -0.5f), 170, levelTweak: 11, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.3f, 0.45f), 189, levelTweak: 12, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.3f, -0.05f), 179, levelTweak: 13, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, 0.4f), 188, levelTweak: 14, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.3f, 0.35f), levelTweak: 15, onTheWay: false);
        SetEnemy(1, new Vector2(1.3f, -0.25f), levelTweak: 15, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, 0.1f), levelTweak: 15, onTheWay: false);

        yield return Wait(() => !allEnemyObjects.Any());
        MainSystems.SetBGM();
        yield return ProduceWarnings(600);
        yield return WaitMessages("人工頭脳", new[] {
            @"大型の敵機反応を確認。",
            @"出力計測…数値が異常です。
大型機としても過大な性能を示しています。
襲撃部隊の隠し玉でしょうか。",
            @"それにしては主戦場と離れていますが…",
            @"…来ます。
まずはこの場を乗り切りましょう。"
        }, callSound: false);

        SetEnemy(enemyList.Count - 1, new Vector2(1.3f, 0), levelTweak: 12, onTheWay: false);

        yield break;
    }

    protected override IEnumerator SuccessAction()
    {
        yield return WaitMessages("人工頭脳", new[] {
            @"…周辺宙域に敵影無し。
無事、大型戦闘機の撃退に成功。",
            @"…逃がした、とも言いますが。",
            @"戦闘記録を調べるに、あの大型機、どうやら襲撃者とは別の所属のようですね。
大型機と襲撃部隊との間で戦闘記録が数件見られます。",
            @"また、襲撃部隊はこの宙域を大きく迂回して進軍したようです。
恐らくは大型機の戦闘に巻き込まれることを避けたのでしょう。",
            @"足止めという意味では、当初想定を上回る戦果と言えますね。",
            @"…さて、主要施設付近には近づくなとのお達しでしたし、我々の仕事はここまででしょう。
あの正体不明大型機に関しては、別途情報収集を行っておきます。",
            @"お疲れ様でした。"
        });

        var returningX = viewPosition.x - viewSize.x;
        if(sysPlayer.position.x > returningX)
        {
            var baseAim = sysPlayer.baseAimPosition;
            var armPosition = Vector2.left * Mathf.Abs(baseAim.x) + Vector2.up * baseAim.y;
            var returningPosition = new Vector2(returningX, sysPlayer.position.y);
            yield return sysPlayer.HeadingDestination(returningPosition, sysPlayer.maximumSpeed, () => sysPlayer.Aiming(armPosition + sysPlayer.position, siteSpeedTweak: 2));
            yield return sysPlayer.StoppingAction();
        }

        yield return WaitMessages("人工頭脳", new[] {
            @"…しかし、やはりあの大型逃がしたのは勿体無かったですね。",
            @"報酬の大幅上乗せが期待できる相手だったのですが…"
        });

        if(!isCleared && sys.storyPhase < 4)
        {
            sys.dominance.theStarEmpire++;
            sys.dominance.principality--;
        }
        sys.storyPhase = 4;
        yield break;
    }
}
