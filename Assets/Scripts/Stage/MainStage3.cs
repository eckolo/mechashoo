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
            return sys.storyPhase >= 3;
        }
    }

    protected override IEnumerator OpeningAction()
    {
        yield break;
    }

    protected override IEnumerator StageAction()
    {
        yield break;
    }

    protected override IEnumerator SuccessAction()
    {
        yield break;
    }
}
