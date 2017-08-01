using UnityEngine;
using System.Collections;
using System.Linq;

public class MainStage3_1 : Stage
{
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
