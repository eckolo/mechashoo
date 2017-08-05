using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 対公国
/// </summary>
public class MainStage4_4 : MainStage4Origin
{
    public override bool challengeable
    {
        get {
            if(sys.dominance.theStarEmpire < 0) return false;
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
