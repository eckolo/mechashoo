using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 概ね当たり判定を持つ物体全般
/// </summary>
public class Material : Roots
{
    /// <summary>
    ///制御下のPartsリスト
    /// </summary>
    [SerializeField]
    private List<Parts> childPartsList = new List<Parts>();
    /// <summary>
    ///左右逆転してないか
    /// </summary>
    public bool widthPositive = true;

    public int setParts(Parts setedParts)
    {
        if (setedParts == null) return -1;

        childPartsList.Add(setedParts);
        setedParts.setParent(gameObject.GetComponent<Material>());

        return childPartsList.Count - 1;
    }
    public Parts getParts(int sequenceNum)
    {
        return childPartsList[sequenceNum];
    }
    public int getPartsNum()
    {
        return childPartsList.Count;
    }

    /// <summary>
    ///mainのベクトルをsubに合わせて補正する
    /// </summary>
    protected Vector2 correctVector(Vector2 main, Vector2 sub, float degree = 0.5f)
    {
        var adjustedSub = sub.normalized * main.magnitude;
        return main * degree + adjustedSub * (1 - degree);
    }

    protected Ship getNearTarget()
    {
        Terms term = target
            => target.GetComponent<Ship>() != null
            && target.gameObject.layer != gameObject.layer;
        List<Roots> shipList = getNearObject(term);

        if (shipList.Count <= 0) return null;
        return shipList[0].GetComponent<Ship>();
    }
}
