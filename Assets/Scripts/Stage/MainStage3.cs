using UnityEngine;
using System.Collections;
using System.Linq;

public class MainStage3 : Stage
{
    const int INTERVAL = 2400;
    const int INTERVAL_A_LITTLE = INTERVAL / 10;

    public override bool challengeable
    {
        get {
            return sys.storyPhase == 2;
        }
    }

    protected override IEnumerator openingAction()
    {
        yield return sysPlayer.headingDestination(new Vector2(-3.6f, 0), sysPlayer.maximumSpeed);
        yield return sysPlayer.stoppingAction();

        yield break;
    }

    protected override IEnumerator successAction()
    {
        var baseAim = sysPlayer.baseAimPosition;
        var armPosition = Vector2.left * Mathf.Abs(baseAim.x) + Vector2.up * baseAim.y;
        var returningPosition = sysPlayer.position + Vector2.left * viewSize.x * 2 / 3;
        yield return sysPlayer.headingDestination(returningPosition, sysPlayer.maximumSpeed, () => sysPlayer.aiming(armPosition + sysPlayer.position, siteSpeedTweak: 2));
        yield return sysPlayer.stoppingAction();

        sys.storyPhase = 3;
        yield break;
    }

    protected override IEnumerator stageAction()
    {
        var enemyCount = enemyList.Count - 1;
        
        setEnemy(0, new Vector2(1.1f, 0.5f), activityLimit: INTERVAL);

        yield break;
    }
}
