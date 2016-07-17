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

    public int setParts(Parts setedParts)
    {
        if (setedParts == null) return -1;

        childPartsList.Add(setedParts);
        setedParts.setParent(gameObject.GetComponent<Material>());

        return childPartsList.Count - 1;
    }
    public Parts getParts(int sequenceNum)
    {
        if (sequenceNum < 0) return null;
        if (sequenceNum >= childPartsList.Count) return null;

        return childPartsList[sequenceNum];
    }
    public int getPartsNum()
    {
        return childPartsList.Count;
    }

    /// <summary>
    ///最寄りの非味方機体検索関数
    /// </summary>
    protected Ship getNearTarget()
    {
        Terms term = target
            => target.GetComponent<Ship>() != null
            && target.gameObject.layer != gameObject.layer;
        List<Roots> shipList = getNearObject(term);

        if (shipList.Count <= 0) return null;
        return shipList[0].GetComponent<Ship>();
    }

    /// <summary>
    ///PartsListの削除関数
    ///引数無しで全消去
    /// </summary>
    public void deleteParts(int? sequenceNum = null)
    {
        if (sequenceNum != null) deleteSimpleParts((int)sequenceNum);
        for (int partsNum = 0; partsNum < childPartsList.Count; partsNum++)
        {
            deleteSimpleParts(partsNum);
        }
        childPartsList = new List<Parts>();
    }
    /// <summary>
    ///PartsListから指定した番号のPartsを削除する
    /// </summary>
    private void deleteSimpleParts(int sequenceNum)
    {
        if (sequenceNum < 0) return;
        if (sequenceNum >= childPartsList.Count) return;

        childPartsList[sequenceNum].selfDestroy();
        childPartsList[sequenceNum] = null;
    }
}
