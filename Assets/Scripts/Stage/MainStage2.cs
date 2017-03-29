using UnityEngine;
using System.Collections;
using System.Linq;

public class MainStage2 : Stage
{
    const int INTERVAL = 2400;

    protected override IEnumerator openingAction()
    {
        yield return sysPlayer.headingDestination(new Vector2(-3.6f, 0), sysPlayer.maximumSpeed);
        yield return sysPlayer.stoppingAction();

        yield return waitMessages("人工頭脳", new[] {
            @"…周辺警戒部隊の索敵範囲に入りました。",
            @"まずは外周部の警戒部隊を蹴散らしましょう。"
        });

        yield return base.openingAction();
    }

    protected override IEnumerator stageAction()
    {
        var enemyCount = enemyList.Count - 1;

        setEnemy(0, new Vector2(1.2f, 0.7f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, 0.5f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.2f, 0.3f), activityLimit: INTERVAL);

        yield return waitWave(INTERVAL);

        setEnemy(0, new Vector2(1.2f, -0.3f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, -0.5f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.2f, -0.7f), activityLimit: INTERVAL);

        yield return waitWave(INTERVAL);

        setEnemy(0, new Vector2(1.1f, 0.75f), 190, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, 0.5f), 190, onTheWay: false, activityLimit: INTERVAL * 2);
        setEnemy(0, new Vector2(1.25f, 0.4f), 190, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.25f, -0.4f), 170, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, -0.5f), 170, onTheWay: false, activityLimit: INTERVAL * 2);
        setEnemy(0, new Vector2(1.1f, -0.75f), 170, activityLimit: INTERVAL);

        yield return waitWave(INTERVAL);

        yield return wait(() => !allEnemies.Any());
        yield return waitMessages("人工頭脳", new[] {
            @"警戒部隊の掃討は完了です、お疲れ様でした。",
            @"…本部より電信。",
            @"強奪班が捕捉され、護衛部隊の本隊に追跡を受けている模様です。",
            @"本部より追加電信。
強奪班に現宙域を通過する逃走経路を指示、本機は追跡部隊を迎え撃てとのことです。",
            @"なお報酬は上乗せするとのこと。
問題はありませんね。",
            @"…計器に反応。
来ました、迎撃してください。"
        });

        setEnemy(1, new Vector2(1.3f, 0), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, 0.3f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, -0.3f), activityLimit: INTERVAL);

        yield return waitWave(INTERVAL);

        setEnemy(2, new Vector2(1.5f, 0), activityLimit: INTERVAL);
        setEnemy(1, new Vector2(1.1f, 0.5f), 190, activityLimit: INTERVAL);
        setEnemy(1, new Vector2(1.1f, -0.5f), 170, activityLimit: INTERVAL);

        yield return waitWave(INTERVAL);

        setEnemy(2, new Vector2(1.1f, -0.5f), activityLimit: INTERVAL);
        setEnemy(2, new Vector2(1.3f, 0), activityLimit: INTERVAL);
        setEnemy(2, new Vector2(1.5f, 0.5f), activityLimit: INTERVAL);
        setEnemy(1, new Vector2(1f, -1.5f), 120, activityLimit: INTERVAL);
        setEnemy(1, new Vector2(1f, 1.6f), 240, activityLimit: INTERVAL);

        yield return waitWave(INTERVAL);

        setEnemy(2, new Vector2(1.8f, 0), activityLimit: INTERVAL);
        setEnemy(2, new Vector2(2, 0.9f), activityLimit: INTERVAL);
        setEnemy(2, new Vector2(2, -0.9f), activityLimit: INTERVAL);
        setEnemy(1, new Vector2(1.4f, 0.6f), activityLimit: INTERVAL);
        setEnemy(1, new Vector2(1.4f, -0.6f), activityLimit: INTERVAL);
        setEnemy(1, new Vector2(1.3f, 0.3f), 190, activityLimit: INTERVAL);
        setEnemy(1, new Vector2(1.3f, -0.3f), 170, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, 0.6f), 200, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, -0.6f), 160, activityLimit: INTERVAL);

        yield return waitWave(INTERVAL);

        setEnemy(0, new Vector2(1.1f, 0), onTheWay: false);
        setEnemy(0, new Vector2(1.3f, 0.6f), onTheWay: false);
        setEnemy(0, new Vector2(1.3f, -0.6f), onTheWay: false);
        setEnemy(1, new Vector2(-1.3f, 0.8f), -10, onTheWay: false);
        setEnemy(1, new Vector2(-1.3f, -0.8f), 10, onTheWay: false);
        setEnemy(2, new Vector2(2f, 0), onTheWay: false);
        setEnemy(2, new Vector2(-1.8f, -1.5f), 45);
        setEnemy(2, new Vector2(-1.8f, 1.5f), -45);

        yield return wait(() => !allEnemies.Any());
        yield return waitMessages("人工頭脳", new[] {
            @"計器に反応…大。",
            @"護衛部隊主力の大型機と推測。
急速に接近しています、注意してください。"
        });

        setEnemy(enemyCount, new Vector2(1.3f, 0), levelCorrection: 12);
        yield break;
    }
}
