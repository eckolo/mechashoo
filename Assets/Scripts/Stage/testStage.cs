using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class testStage : Stage
{
    public List<Ship> enemyList = new List<Ship>();

    protected override IEnumerator stageAction()
    {
        for (int i = 0; i < 12000; i++)
        {
            if (i % 1000 == 0) setObject(enemyList[0], new Vector2(0, 0.5f));
            yield return null;
        }
    }
}
