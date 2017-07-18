using System.Collections;
using System.Linq;

public class OneShot : Stage
{
    protected override IEnumerator StageAction()
    {
        SetEnemy(0, 1.2f, 0);

        yield return Wait(() => !allEnemies.Any());
    }

    protected override bool isComplete => !allEnemyObjects.Any();
    public override bool challengeable => true;
}
