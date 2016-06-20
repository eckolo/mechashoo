using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// プレイヤーの操作機体クラス
/// </summary>
public class Player : Ship
{
    [SerializeField]
    private float digree = 0;
    /// <summary>
    ///各種アクションのフラグ
    /// </summary>
    [SerializeField]
    private bool actionRight = false;
    [SerializeField]
    private bool actionLeft = false;
    [SerializeField]
    private bool actionBody = false;
    /// <summary>
    ///各種キーのAxes名
    /// </summary>
    const string rightActName = "ShotRight";
    const string leftActName = "ShotLeft";
    const string bodyActName = "ShotBody";

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (armPosition.x < 0) widthPositive = !widthPositive;
        // 右・左
        float keyValueX = Input.GetAxisRaw("Horizontal");

        // 上・下
        float keyValueY = Input.GetAxisRaw("Vertical");

        // 移動する向きを求める
        Vector2 direction = new Vector2(keyValueX, keyValueY).normalized;
        // 移動する速度を求める
        float innerSpeed = Input.GetKey(KeyCode.LeftShift) ? 0 : speed;

        // 移動
        setVerosity(direction, innerSpeed, true);

        digree += keyValueY * -1;
        //transform.rotation = Quaternion.AngleAxis(digree, Vector3.forward);

        if (Input.GetButtonDown(rightActName)) actionRight = !actionRight;
        if (Input.GetButtonDown(leftActName)) actionLeft = !actionLeft;
        if (Input.GetButtonDown(bodyActName)) actionBody = !actionBody;

        if (actionRight)
        {
            getHand(getParts(0)).actionWeapon();
        }
        if (actionLeft)
        {
            getHand(getParts(1)).actionWeapon();
        }
        if (actionBody)
        {
            foreach (var weaponNum in weaponNumList)
            {
                if (getParts(weaponNum) != null) getParts(weaponNum).GetComponent<Weapon>().Action();
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            armPosition.x += keyValueX / 200 * (widthPositive ? 1 : -1);
            armPosition.y += keyValueY / 200;
        }

        armPosition = getParts(armNumList[0]).setManipulatePosition(armPosition);
        armPosition = getParts(armNumList[1]).setManipulatePosition(armPosition);
        var differenceAngle = -45 * Vector2.Angle(Vector2.left, armPosition) / 180;
        getParts(armNumList[1]).transform.Rotate(0, 0, differenceAngle);
        getParts(armNumList[1]).childParts.transform.Rotate(0, 0, differenceAngle * -1);
    }
}
