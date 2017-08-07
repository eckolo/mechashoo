using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 対公国
/// </summary>
public class MainStage4_4 : MainStage4Origin
{
    protected new const int INTERVAL = 4800;

    public override bool challengeable
    {
        get {
            if(sys.storyPhase >= Configs.StoryPhase.GAME_CLEAR) return true;
            var stage3_2 = sys.stages
                .SingleOrDefault(stage => stage.GetComponent<MainStage3_2>() != null)
                .GetComponent<MainStage3_2>();
            if(!sys.GetClearFlug(stage3_2)) return false;
            if(sys.dominance.theStarEmpire < 0) return false;
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
            @"…偵察地点へ到達。
後続部隊の到着を待って…"
        });
        yield return ProduceCaution(400, 2);
        MainSystems.SetBGM(BGMList[1]);
        yield return WaitMessages("人工頭脳", new[] {
            @"転移反応、小規模部隊です。
…早くないですか。",
            @"まあ威力偵察である以上戦闘は織り込み済みです。
手早く蹴散らしましょう。"
        }, callSound: false);

        yield break;
    }

    protected override IEnumerator StageAction()
    {
        SetEnemy(0, new Vector2(1.2f, 0.7f), -165, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.3f, -0.1f), -165, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.3f, 0.3f), -165, activityLimit: INTERVAL, levelTweak: 5);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -0.7f), 165, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.3f, 0.1f), 165, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(1.3f, -0.3f), 165, levelTweak: 5, onTheWay: false);

        yield return WaitWave(INTERVAL);

        SetEnemy(1, new Vector2(1.2f, -0.6f), levelTweak: 5, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(1, new Vector2(1.3f, 0.6f), levelTweak: 5, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(2, new Vector2(1.3f, 0f), levelTweak: 15, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.3f, -0.3f), levelTweak: 5, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(1, new Vector2(1.3f, 0.3f), levelTweak: 5, activityLimit: INTERVAL, onTheWay: false);

        yield return Wait(() => !allEnemyObjects.Any());
        yield return WaitMessages("人工頭脳", new[] {
            @"小規模ですが、どうやら精鋭部隊のようですね。
中型機が複数混じっています。",
            @"気を付けてください。"
        });

        SetEnemy(1, new Vector2(1.2f, 0.4f), 165, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.3f, -0.1f), 165, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(1.3f, 0.2f), 165, activityLimit: INTERVAL, levelTweak: 15, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.4f), -165, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.3f, 0.1f), -165, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(1.3f, -0.2f), -165, activityLimit: INTERVAL, levelTweak: 15, onTheWay: false);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.3f, 0f), activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.3f, 0.1f), 165, activityLimit: INTERVAL, levelTweak: 5);
        SetEnemy(1, new Vector2(1.3f, -0.1f), -165, activityLimit: INTERVAL, levelTweak: 5);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.3f, 0.2f), 150, activityLimit: INTERVAL, levelTweak: 15, onTheWay: false);
        SetEnemy(2, new Vector2(1.3f, -0.2f), -150, activityLimit: INTERVAL, levelTweak: 15, onTheWay: false);

        yield return WaitWave(INTERVAL);

        SetEnemy(2, new Vector2(1.1f, 0.6f), activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(2, new Vector2(1.1f, -0.6f), activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -1.5f), 135, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.2f, 1.5f), -135, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(-1.1f, 0.8f), -10, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(-1.1f, -0.8f), 10, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, 0), activityLimit: INTERVAL, onTheWay: false);

        yield return WaitWave(INTERVAL);

        SetEnemy(2, new Vector2(1.3f, 0f), levelTweak: 15, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.3f, 0.3f), levelTweak: 15, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.3f, -0.3f), levelTweak: 15, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.3f, 0.6f), levelTweak: 15, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.3f, -0.6f), levelTweak: 15, onTheWay: false);

        yield return Wait(() => !allEnemyObjects.Any());
        yield return ProduceWarnings(600);
        yield return WaitMessages("人工頭脳", new[] {
            @"さて来ましたよ。
大型機の反応です。",
            @"…偵察って何でしたかね。"
        }, callSound: false);

        SetEnemy(enemyList.Count - 1, new Vector2(1.3f, 0), levelTweak: 12, onTheWay: false);

        yield break;
    }

    protected override IEnumerator SuccessAction()
    {
        yield return WaitMessages("人工頭脳", new[] {
            @"敵大型機の撃墜を確認。
敵軍へ圧をかけるという意味では十二分な戦果でしょう。",
            @"…偵察という意味では情報不足が否めませんが。
まあこちらも敵戦力分布の一部が判明したので良しとしましょう。",
            @"しかし今の部隊、どうも妙な位置にいましたね。
公国軍主力位置から考えても役割の薄い箇所です。",
            @"まあ、それはこちらで答えを出すことでもありませんか。",
            @"丁度後続部隊も到着したようです。
入れ替わりで帰投しますよ。"
        }, callSound: false);

        SetEnemy(3, new Vector2(-1.2f, 0.3f), 0, setLayer: Configs.Layers.PLAYER);
        SetEnemy(3, new Vector2(-1.2f, -0.3f), 0, setLayer: Configs.Layers.PLAYER);
        SetEnemy(3, new Vector2(-1.4f, 0.6f), 0, setLayer: Configs.Layers.PLAYER);
        SetEnemy(3, new Vector2(-1.4f, -0.6f), 0, setLayer: Configs.Layers.PLAYER);
        var returningX = viewPosition.x - viewSize.x;
        if(sysPlayer.position.x > returningX)
        {
            var baseAim = sysPlayer.baseAimPosition;
            var armPosition = Vector2.left * Mathf.Abs(baseAim.x) + Vector2.up * baseAim.y;
            var returningPosition = new Vector2(returningX, sysPlayer.position.y);
            yield return sysPlayer.HeadingDestination(returningPosition, sysPlayer.maximumSpeed, () => sysPlayer.Aiming(armPosition + sysPlayer.position, siteSpeedTweak: 2));
            yield return sysPlayer.StoppingAction();
        }

        if(!isCleared && sys.storyPhase < Configs.StoryPhase.END_FOUR_PRACTICE)
        {
            sys.dominance.principality++;
            sys.dominance.theStarEmpire--;
        }
        yield return base.SuccessAction();
        yield break;
    }
}
