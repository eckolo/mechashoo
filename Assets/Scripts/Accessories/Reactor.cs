using UnityEngine;
using System.Collections;

public class Reactor : Accessory
{
    /// <summary>
    /// 最大速度
    /// </summary>
    public float maxSpeed = 1;
    /// <summary>
    /// 最低速度
    /// </summary>
    public float minSpeed = 1;
    /// <summary>
    /// 出力
    /// </summary>
    public float horsepower = 1;
    /// <summary>
    /// 本体の回転可否フラグ
    /// </summary>
    [SerializeField]
    public bool rollable = false;
}
