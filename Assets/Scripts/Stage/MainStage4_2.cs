using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 対古王国
/// </summary>
public class MainStage4_2 : MainStage4Origin
{
    public override bool challengeable
    {
        get {
            if(sys.dominance.republic < 0) return false;
            return base.challengeable;
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
        yield return base.SuccessAction();
        yield break;
    }
}
