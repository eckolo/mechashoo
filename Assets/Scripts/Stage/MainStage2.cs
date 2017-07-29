using UnityEngine;
using System.Collections;
using System.Linq;

public class MainStage2 : Stage
{
    const int INTERVAL = 2400;
    const int INTERVAL_A_LITTLE = INTERVAL / 10;

    public override bool challengeable
    {
        get {
            return sys.storyPhase == 1;
        }
    }

    protected override IEnumerator OpeningAction()
    {
        yield return DisplayLocation(location);

        yield return sysPlayer.HeadingDestination(new Vector2(-3.6f, 0), sysPlayer.maximumSpeed);
        yield return sysPlayer.StoppingAction();

        yield return WaitMessages("人工頭脳", new[] {
            @"…周辺警戒部隊の索敵範囲に入りました。",
            @"まずは外周部の警戒部隊を蹴散らしましょう。"
        });

        yield break;
    }

    protected override IEnumerator StageAction()
    {
        SetEnemy(0, new Vector2(1.1f, 0.7f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, 0.3f), activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.2f, -0.3f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.1f, -0.7f), activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.2f, -0.4f), 170, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.1f, -0.75f), 170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.1f, 0.75f), 190, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, 0.4f), 190, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.1f, 0), onTheWay: false, activityLimit: INTERVAL * 2);

        yield return Wait(() => !allEnemyObjects.Any());
        yield return WaitMessages("人工頭脳", new[] {
            @"警戒部隊の掃討は完了です、お疲れ様でした。",
            @"…本部より電信。",
            @"強奪班が捕捉され、護衛部隊の本隊に追跡を受けている模様です。",
            @"本部より追加電信。
強奪班には現宙域を通過する逃走経路を指示、本機は追跡部隊を迎え撃てとのこと。",
            @"なお報酬は上乗せするとの確約を受けました。
であれば何も問題ありませんね。",
            @"…計器に反応。
来ました、迎撃してください。"
        });

        SetEnemy(0, new Vector2(1.1f, 0.3f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.1f, -0.3f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.1f, 0), activityLimit: INTERVAL, onTheWay: false);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.1f, 0.5f), 190, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.1f, -0.5f), 170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.1f, 0), activityLimit: INTERVAL, onTheWay: false);

        yield return WaitWave(INTERVAL);

        SetEnemy(2, new Vector2(1.1f, -0.5f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.1f, 0.5f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.1f, 0), activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(1, new Vector2(1.1f, 0.3f), 190, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.1f, -0.3f), 170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.9f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, -0.9f), activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.1f, 0.6f), onTheWay: false);
        SetEnemy(0, new Vector2(1.1f, -0.6f), onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -1.5f), 135);
        SetEnemy(1, new Vector2(1.2f, 1.5f), -135);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(-1.1f, 0.8f), -10);
        SetEnemy(2, new Vector2(-1.1f, -0.8f), 10);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, 0), onTheWay: false);

        yield return Wait(() => !allEnemyObjects.Any());
        yield return ProduceWarnings(600);
        yield return WaitMessages("人工頭脳", new[] {
            @"計器に反応…大。",
            @"護衛部隊主力の大型機と推測。
急速に接近しています、注意してください。"
        }, callSound: false);

        SetEnemy(enemyList.Count - 1, new Vector2(1.3f, 0), levelTweak: 12, onTheWay: false);
        yield break;
    }

    protected override IEnumerator SuccessAction()
    {
        yield return WaitMessages("人工頭脳", new[] {
            @"…周辺宙域に敵影無し。
強奪班も無事撤退完了したようですね。",
            @"足止め想定の大型機を撃墜したため報酬も増額とのこと。
演習設備の増設などお勧めします。",
            @"さて、追撃部隊に追いつかれる前に帰投しましょうか。
お疲れ様でした。"
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

        sys.storyPhase = 2;
        yield break;
    }
}
