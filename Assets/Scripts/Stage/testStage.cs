using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class testStage : Stage
{
    public List<Ship> enemyList = new List<Ship>();

    protected override IEnumerator stageAction()
    {
        setEnemy(enemyList[0].GetComponent<Npc>(), new Vector2(0, 0.5f));
        for (ulong i = 0; i < 1200 - stageLevel; i++) yield return null;
        stageLevel += 1;

        yield return stageAction();
    }
}
