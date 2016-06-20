using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Enemy : Ship
{
    //現在のモーションを示す番号
    public int nowActionNum = 0;
    //行動選択肢の最大数
    protected int maxActionChoices = 1;
    //モーションの切り替わりタイミングフラグ
    protected bool timingSwich = true;

    public override void Update()
    {
        base.Update();
        Action(nowActionNum);
    }

    protected virtual int setNextMotion(int actionNum)
    {
        if (actionNum != 0) return actionNum;
        return Random.Range(0, maxActionChoices + 1);
    }

    public override bool Action(int actionNum = 0)
    {
        if (!timingSwich) return false;
        timingSwich = false;

        return base.Action(actionNum);
    }
    protected override IEnumerator baseMotion(int actionNum)
    {
        yield return base.baseMotion(actionNum);

        nowActionNum = setNextMotion(actionNum);
        timingSwich = true;

        yield break;
    }

    protected override IEnumerator Motion(int actionNum)
    {
        if (actionNum != 0) setVerosity(Vector2.left, 1);
        yield break;
    }
}
