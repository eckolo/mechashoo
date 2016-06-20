using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Material : Roots
{
    //制御下のPartsリスト
    [SerializeField]
    private List<Parts> childPartsList = new List<Parts>();
    //左右逆転してないか
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

    //mainのベクトルをsubに合わせて補正する
    protected Vector2 correctVector(Vector2 main, Vector2 sub, float degree = 0.5f)
    {
        var adjustedSub = sub.normalized * main.magnitude;
        return main * degree + adjustedSub * (1 - degree);
    }
}
