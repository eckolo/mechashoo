using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Ship
{
    [SerializeField]
    private float digree = 0;

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        var Root = GetComponent<Root>();

        if (armPosition.x < 0) positive = !positive;
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

        if (Input.GetKey(KeyCode.Z))
        {
            foreach (var armNum in armNumList)
            {
                instructAction(armNum);
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            armPosition.x += keyValueX / 200 * (positive ? 1 : -1);
            armPosition.y += keyValueY / 200;
        }

        armPosition = Root.setManipulatePosition(armPosition, Root.childPartsList[armNumList[0]]);
        Root.childPartsList[armNumList[1]].transform.rotation = Root.childPartsList[armNumList[0]].transform.rotation;
        Root.childPartsList[armNumList[1]].childParts.transform.rotation = Root.childPartsList[armNumList[0]].childParts.transform.rotation;
        var differenceAngle = -45 * Vector2.Angle(Vector2.left, armPosition) / 180;
        Root.childPartsList[armNumList[1]].transform.Rotate(0, 0, differenceAngle);
        Root.childPartsList[armNumList[1]].childParts.transform.Rotate(0, 0, differenceAngle * -1);
    }
}
