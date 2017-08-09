using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 対央国
/// </summary>
public class MainStage4_1 : MainStage4Origin
{
    public override bool challengeable
    {
        get {
            if(sys.storyPhase >= Configs.StoryPhase.GAME_CLEAR) return true;
            var stage3_1 = sys.stages
                .SingleOrDefault(stage => stage.GetComponent<MainStage3_1>() != null)
                .GetComponent<MainStage3_1>();
            if(!sys.GetClearFlug(stage3_1)) return false;
            if(sys.dominance.principality < 0) return false;
            return base.challengeable;
        }
    }

    protected override IEnumerator OpeningAction()
    {
        yield return DisplayLocation(location);

        yield return sysPlayer.HeadingDestination(new Vector2(-3.6f, 0), sysPlayer.maximumSpeed);
        yield return sysPlayer.StoppingAction();
        yield return Wait(Configs.Window.DEFAULT_MOTION_TIME);

        yield return WaitMessages("人工頭脳", new[] {
            @"司令部へ報告。",
            @"哨戒地点11-6番確認、特に異常ありません。
定刻までこちらで待機します。"
        });
        yield return Wait(Configs.Window.DEFAULT_MOTION_TIME);
        yield return WaitMessages("護衛軍司令部", new[] {
            @"了解、待機後は哨戒地点23-4番へ移動されたし。"
        });
        yield return Wait(Configs.Window.DEFAULT_MOTION_TIME);
        yield return WaitMessages("人工頭脳", new[] {
            @"待機後哨戒地点23-4番へ移動、了解。
通信を終了します。"
        });
        yield return Wait(INTERVAL_A_LITTLE);
        yield return WaitMessages("人工頭脳", new[] {
            @"…この戦闘の発端となった、会談襲撃を公国が主導したとする声明ですが。",
            @"確かに襲撃軍は、公国で主に運用される機体で構成されていました。",
            @"しかし同様の機体を入手すること自体は不可能ではありません。
また、例の古王国についての噂もあります。",
            @"…この戦争は、央星帝国の勇み足だったのではないか、とも考えられるのです。",
            @"始まってしまった今となっては、我々が考えても仕方の無いことなのかもしれませんが。"
        }, callSound: false);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return WaitMessages("護衛軍司令部", new[] {
            @"各機へ通達。",
            @"哨戒地点10-5番より敵部隊と遭遇の報有り。
中規模、大型を数機含むとのこと。
付近を哨戒中の機体は至急向かわれたし。"
        });
        yield return WaitMessages("人工頭脳", new[] {
            @"10-5番、目と鼻の先ですね。
大型数機、となると哨戒任務中の小型機編隊では荷が重いでしょう。",
            @"同僚に恩を売るまたとない機会…"
        }, callSound: false);
        yield return ProduceCaution(400, 2);
        MainSystems.SetBGM(BGMList[1]);
        yield return WaitMessages("人工頭脳", new[] {
            @"計器に反応、付近に転移反応。
識別信号は央星帝国軍のものです。",
            @"…嫌な間隔で来ましたね。
単なる威力偵察、ではないかもしれませんよ。",
            @"こちらで司令部に連絡します。
ひとまずこの場で迎撃を。"
        }, callSound: false);

        yield break;
    }

    protected override IEnumerator StageAction()
    {
        SetEnemy(0, new Vector2(1.3f, 0.5f), 185, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -0.5f), 175, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, 0.9f), 189, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -0.9f), 171, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, 0.3f), 183, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -0.3f), 177, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, 0.7f), 187, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -0.7f), 173, activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(3, new Vector2(1.2f, 0.7f), activityLimit: INTERVAL, levelTweak: 5, onTheWay: false);
        SetEnemy(3, new Vector2(1.2f, -0.7f), activityLimit: INTERVAL, levelTweak: 5, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -1.4f), 135);
        SetEnemy(1, new Vector2(1.2f, 1.4f), -135);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(-0.6f, 1.3f), -150);
        SetEnemy(0, new Vector2(-0.6f, -1.3f), 150);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(4, new Vector2(1.2f, 0), levelTweak: 10, onTheWay: false);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.2f, 0.8f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, 0), 200, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.3f, 0.4f), 190, levelTweak: 10, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.8f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, 0), 160, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.3f, -0.4f), 170, levelTweak: 10, onTheWay: false);

        yield return Wait(() => !allEnemyObjects.Any());
        yield return WaitMessages("人工頭脳", new[] {
            @"…やはり来ましたか。",
            @"哨戒地点10-5番方面より敵機の増援です。
どうやらいくつかに部隊を分け各方面へ転進してきた模様。",
            @"このままでは確実に挟撃を受けますね。
…いや、今更小規模部隊の挟撃程度で心配はしませんが。",
            @"相手が逃げ帰るまでひとしきり暴れるとしましょう。"
        });

        SetEnemy(0, new Vector2(1.2f, 0.7f), -165, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.3f, -0.1f), -165, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(1.3f, 0.3f), -165, levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.7f), 165, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.3f, 0.1f), 165, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(1.3f, -0.3f), 165, levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(-1.2f, 0.7f), -15, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(-1.3f, -0.1f), -15, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(-1.3f, 0.3f), -15, levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(-1.2f, -0.7f), 15, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(-1.3f, 0.1f), 15, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(-1.3f, -0.3f), 15, levelTweak: 5, activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(1, new Vector2(1.2f, -0.7f), activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.3f, 0.1f), activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(1.3f, -0.3f), levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(-1.2f, -0.2f), 0, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(-1.2f, 0.2f), 0, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(-1.2f, -0.6f), 0, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(-1.2f, 0.6f), 0, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(-1.2f, -0.4f), 0, activityLimit: INTERVAL);
        SetEnemy(3, new Vector2(-1.2f, 0.4f), 0, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, -0.7f), onTheWay: false);
        SetEnemy(2, new Vector2(1.3f, 0.1f), onTheWay: false);
        SetEnemy(4, new Vector2(1.3f, -0.3f), levelTweak: 5, onTheWay: false);

        yield return WaitWave(INTERVAL);

        SetEnemy(2, new Vector2(1.2f, 0.5f), levelTweak: 1, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(-1.2f, -0.5f), 0, levelTweak: 1, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, 0.7f), levelTweak: 2, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(-1.2f, -0.7f), 0, levelTweak: 2, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(0.8f, 1.2f), -90, levelTweak: 3, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(-0.8f, -1.2f), 90, levelTweak: 3, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(0.1f, 1.2f), -90, levelTweak: 4, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(-0.1f, -1.2f), 90, levelTweak: 4, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(-0.1f, 1.2f), -90, levelTweak: 5, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(0.1f, -1.2f), 90, levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(-1.2f, 0.9f), 0, levelTweak: 6, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(1.2f, -0.9f), levelTweak: 6, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(-1.2f, 0), 0, levelTweak: 7, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(1.2f, 0), levelTweak: 7, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(-1.2f, -0.1f), 0, levelTweak: 8, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(1.2f, 0.1f), levelTweak: 8, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(-0.9f, -1.2f), 90, levelTweak: 9, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(0.9f, 1.2f), -90, levelTweak: 9, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(-0.8f, -1.2f), 90, levelTweak: 10, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(0.8f, 1.2f), -90, levelTweak: 10, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(-0.7f, -1.2f), 90, levelTweak: 11, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(0.7f, 1.2f), -90, levelTweak: 11, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, 1.2f), -150, levelTweak: 12, onTheWay: false);
        SetEnemy(2, new Vector2(-1.2f, 1.2f), -30, levelTweak: 12, onTheWay: false);
        SetEnemy(2, new Vector2(1.2f, -1.2f), 150, levelTweak: 12, onTheWay: false);
        SetEnemy(2, new Vector2(-1.2f, -1.2f), 30, levelTweak: 12, onTheWay: false);

        yield return Wait(() => !allEnemyObjects.Any());
        MainSystems.SetBGM();
        yield return ProduceWarnings(600);
        yield return WaitMessages("人工頭脳", new[] {
            @"大型の敵機反応を確認。
どうやら1機だけのようです。",
            @"相当数の小型を落としましたからね。
こちらを脅威と認識したのでしょう。",
            @"これを撃退すれば一区切りです。
気合い入れていきましょう。"
        }, callSound: false);

        SetEnemy(enemyList.Count - 1, new Vector2(1.2f, 1.2f), -120, levelTweak: 12, onTheWay: false);

        yield break;
    }

    protected override IEnumerator SuccessAction()
    {
        yield return WaitMessages("人工頭脳", new[] {
            @"…周辺宙域に敵影無し。
敵部隊は撤退した模様です。",
            @"恐らくあの大型機が殿だったのでしょう。"
        });
        yield return Wait(Configs.Window.DEFAULT_MOTION_TIME);
        yield return WaitMessages("護衛軍司令部", new[] {
            @"各機へ通達、敵強襲部隊の撤退を確認。
哨戒部隊は入れ替えと再編のため、全機一時帰投されたし。"
        });
        yield return Wait(Configs.Window.DEFAULT_MOTION_TIME);
        yield return WaitMessages("人工頭脳", new[] {
            @"一時帰投、了解。
通信を終了します。"
        });
        yield return Wait(Configs.Window.DEFAULT_MOTION_TIME);
        yield return WaitMessages("人工頭脳", new[] {
            @"ということで一旦基地へ帰りましょう。
お疲れ様でした。"
        }, callSound: false);

        var returningX = viewPosition.x - viewSize.x;
        var baseAim = sysPlayer.baseAimPosition;
        var armPosition = Vector2.left * Mathf.Abs(baseAim.x) + Vector2.up * baseAim.y;
        var returningPosition = new Vector2(returningX, sysPlayer.position.y);
        yield return sysPlayer.HeadingDestination(returningPosition, sysPlayer.maximumSpeed, () => sysPlayer.Aiming(armPosition + sysPlayer.position, siteSpeedTweak: 2));
        yield return sysPlayer.StoppingAction();

        if(!isCleared && sys.storyPhase < Configs.StoryPhase.END_FOUR_PRACTICE)
        {
            sys.dominance.principality++;
            sys.dominance.theStarEmpire--;
        }
        yield return base.SuccessAction();
        yield break;
    }
}
