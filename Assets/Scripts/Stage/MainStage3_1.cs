using UnityEngine;
using System.Collections;
using System.Linq;

public class MainStage3_1 : Stage
{
    public override bool challengeable
    {
        get {
            if(sys.storyPhase >= Configs.StoryPhase.GAME_CLEAR) return true;
            return sys.storyPhase >= 3;
        }
    }

    protected override IEnumerator OpeningAction()
    {
        yield return DisplayLocation(location);

        sysPlayer.InvertWidth(Vector2.left);
        sysPlayer.SetAlignment(sysPlayer.baseAimPosition);
        yield return sysPlayer.HeadingDestination(new Vector2(-3.6f, 0), sysPlayer.maximumSpeed);
        StartCoroutine(sysPlayer.StoppingAction());

        yield return WaitMessages("人工頭脳", new[] {
            @"止まってください。"
        });
        yield return WaitMessages("人工頭脳", new[] {
            @"襲撃部隊本部より電信がありました。",
            @"護衛部隊の一部がこちらの背後を取るよう転移を行ったとのこと。",
            @"数では正規軍であるあちらが圧倒しています。
挟撃を受ければこちらの隊の壊滅は免れないでしょう。",
            @"…と、いうことで足止めの指令が下りました。
最後尾についたのがよろしくなかったようですね。",
            @"何はともあれ足止めの役は全うするとしましょう。
…査定に響かない程度には、ですが。"
        }, false);
        yield return sysPlayer.AimingAction(sysPlayer.baseAimPosition.Invert());
        yield break;
    }

    protected override IEnumerator StageAction()
    {
        SetEnemy(0, new Vector2(1.2f, -0.4f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, 0.4f), activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.3f, 0), levelTweak: 5, activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.2f, 0.7f), -165, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.3f, -0.1f), -165, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.3f, 0.3f), -165, levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.7f), 165, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(0, new Vector2(1.3f, 0.1f), 165, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(1, new Vector2(1.3f, -0.3f), 165, levelTweak: 5, activityLimit: INTERVAL, onTheWay: false);

        yield return WaitWave(INTERVAL);

        SetEnemy(1, new Vector2(1.2f, -0.8f), 170, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.2f, 0.8f), -170, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.3f, -0.4f), 175, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(0, new Vector2(1.3f, 0.4f), -175, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, 0), levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.3f), 175, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(0, new Vector2(1.2f, 0.3f), -175, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -0.6f), 170, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.2f, 0.6f), -170, activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(1, new Vector2(1.2f, -0.2f), 165, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(1, new Vector2(1.2f, 0.2f), -165, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(0, new Vector2(1.3f, -0.4f), 150, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(0, new Vector2(1.3f, 0.4f), -150, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -0.8f), activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.2f, 0.8f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.8f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, 0.8f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -0.8f), activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.2f, 0.8f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.3f), -175, onTheWay: false);
        SetEnemy(0, new Vector2(1.2f, 0.3f), 175, onTheWay: false);
        SetEnemy(1, new Vector2(1.3f, 0), levelTweak: 5, onTheWay: false);

        yield return Wait(() => !allEnemyObjects.Any());
        yield return WaitMessages("人工頭脳", new[] {
            @"…悪い知らせです。",
            @"後方、つまり我々の同行していた隊との間に転移の反応を確認。
こちらが迎撃に部隊を分けるところまで織り込み済みだったというわけでしょう。",
            @"これで足止めの意味は薄くなりました。
既に護衛軍の挟撃策は成ったも同然ですから。",
            @"残る問題点は…こちらがどう生き残るか、ですかね。",
            @"…前後からの挟撃です。
精一杯足搔くとしましょう。"
        });

        SetEnemy(0, new Vector2(1.2f, 0), levelTweak: 5, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.3f, -0.5f), activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.3f, 0.5f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(-1.2f, 0), 0, levelTweak: 5, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(-1.3f, -0.6f), 0, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(-1.3f, 0.6f), 0, activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(2, new Vector2(-1.2f, 0.7f), -15, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(-1.3f, -0.1f), -15, activityLimit: INTERVAL);
        SetEnemy(3, new Vector2(-1.3f, 0.3f), -15, levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -0.8f), activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.2f, 0.8f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.8f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, 0.8f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -0.8f), activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.2f, 0.8f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(-1.2f, -0.7f), 15, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(2, new Vector2(-1.3f, 0.1f), 15, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(3, new Vector2(-1.3f, -0.3f), 15, levelTweak: 5, activityLimit: INTERVAL, onTheWay: false);

        yield return WaitWave(INTERVAL);

        SetEnemy(1, new Vector2(1.3f, -1.3f), 150, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.3f, 1.3f), -150, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.7f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, -0.7f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(-0.7f, 1.3f), -60, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(-0.7f, -1.3f), 60, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1f, -1.3f), 135, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1f, 1.3f), -135, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(-0.7f, 1.3f), -60, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(-0.7f, -1.3f), 60, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, 0.7f), onTheWay: false);
        SetEnemy(1, new Vector2(1.2f, -0.7f), onTheWay: false);
        SetEnemy(3, new Vector2(-1.2f, -0.2f), 0, levelTweak: 5, onTheWay: false);

        yield return Wait(() => !allEnemyObjects.Any());
        MainSystems.SetBGM();
        yield return ProduceWarnings(600);
        yield return WaitMessages("人工頭脳", new[] {
            @"大型の敵機反応が急速接近。",
            @"護衛軍…にしては識別信号が妙です。
慎重な対応を。"
        });

        SetEnemy(enemyList.Count - 2, new Vector2(1.3f, 1f), -150, levelTweak: 1, onTheWay: true);

        yield return Wait(() => !allEnemyObjects.Any());
        MainSystems.SetBGM();
        yield return WaitMessages("人工頭脳", new[] {
            @"…逃げましたね。",
            @"何だったのでしょうか。",
            @"しかし、今の大型機が轢いて行ったおかげで敵軍に穴が空きました。
少し粘れば脱出に十分な間隙も確保できるでしょう。"
        });
        MainSystems.SetBGM(BGMList[1]);
        yield return WaitMessages("人工頭脳", new[] {
            @"…次、来ます。
こんなところで落ちないでくださいね。"
        }, callSound: false);

        SetEnemy(3, new Vector2(-1.3f, 0f), 0, levelTweak: 10, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(-1.3f, 0.1f), 15, levelTweak: 5, activityLimit: INTERVAL);
        SetEnemy(3, new Vector2(-1.3f, -0.1f), -15, levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(-1.3f, 0.2f), 30, levelTweak: 5, activityLimit: INTERVAL);
        SetEnemy(3, new Vector2(-1.3f, -0.2f), -30, levelTweak: 5, activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(1, new Vector2(1.2f, 0f), levelTweak: 5, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(0.8f, 1.3f), -90, levelTweak: 5, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(0.8f, -1.3f), 90, levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(-0.4f, 1.3f), -90, levelTweak: 5, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(-0.4f, -1.3f), 90, levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(0.4f, 1.3f), -90, levelTweak: 5, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(0.4f, -1.3f), 90, levelTweak: 5, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(-1.3f, 0.2f), -30, levelTweak: 5, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(2, new Vector2(-1.3f, -0.2f), 30, levelTweak: 5, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(-0.8f, 1.3f), -90, levelTweak: 5, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(-0.8f, -1.3f), 90, levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(0, 1.3f), -90, levelTweak: 5, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(0, -1.3f), 90, levelTweak: 5, activityLimit: INTERVAL);

        yield return Wait(() => !allEnemyObjects.Any());
        yield return ProduceWarnings(600);
        yield return WaitMessages("人工頭脳", new[] {
            @"護衛軍の大型機を補足。
当初後方へ転移してきた部隊の後詰でしょう。",
            @"手早くくず鉄にして脱出しますよ。"
        });

        SetEnemy(enemyList.Count - 1, new Vector2(1.3f, -0.3f), levelTweak: 12, onTheWay: false);

        yield break;
    }

    protected override IEnumerator SuccessAction()
    {
        yield return WaitMessages("人工頭脳", new[] {
            @"前方宙域に大きな間隙が開きました。
部隊中枢機を撃墜したことで指揮系統が乱れているのでしょうか。",
            @"…どちらにせよ、今が好機です。",
            @"索敵、経路探査はお任せを。
全速力で突っ切ってください。"
        });

        var returningX = viewPosition.x + viewSize.x;
        if(sysPlayer.position.x > returningX)
        {
            var baseAim = sysPlayer.baseAimPosition;
            var armPosition = Vector2.right * Mathf.Abs(baseAim.x) + Vector2.up * baseAim.y;
            var returningPosition = new Vector2(returningX, -viewSize.y / 4);
            yield return sysPlayer.HeadingDestination(returningPosition, sysPlayer.maximumSpeed * 2, () => sysPlayer.Aiming(sysPlayer.position + armPosition, siteSpeedTweak: 2));
        }

        sys.storyPhase = 4;
        if(!isCleared)
        {
            sys.dominance.theStarEmpire--;
            sys.dominance.oldKingdom++;
        }
        yield break;
    }
}
