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
            if(sys.dominance.principality < 0) return false;
            return base.challengeable;
        }
    }

    protected override IEnumerator OpeningAction()
    {
        yield return DisplayLocation(location);

        yield return sysPlayer.HeadingDestination(new Vector2(-3.6f, 0), sysPlayer.maximumSpeed);
        yield return sysPlayer.StoppingAction();

        yield return WaitMessages("人工頭脳", new[] {
            @"哨戒地点6番確認。
定刻までこちらで待機、その後哨戒地点4番への移動となります。",
            @"…この戦闘の発端となった、会談襲撃を公国が主導したとする声明ですが。",
            @"確かに"
        });
        yield return WaitMessages("人工頭脳", new[] {
            @"哨戒地点6番確認。
定刻までこちらで待機、その後哨戒地点4番への移動となります。",
            @"…この戦闘の発端となった、会談襲撃を公国が主導したとする声明ですが。",
            @"確かに"
        });

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
