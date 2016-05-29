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

        // 移動の制限
        if (!Input.GetKey(KeyCode.LeftShift)) limitMove(direction);

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
        /*
        var weaponListOrigin = GameObject.Find("System").GetComponent<PartsList>().WeaponList;
        if (Input.GetKeyDown(KeyCode.A))
        {
            leftWeapon = (leftWeapon + 1) % weaponListOrigin.ToArray().Length;
            GetComponent<Ship>().setArm(weaponListOrigin[leftWeapon], 0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            rightWeapon = (rightWeapon + 1) % weaponListOrigin.ToArray().Length;
            GetComponent<Ship>().setArm(weaponListOrigin[rightWeapon], 1);
        }
        */
    }

    // 機体の移動
    void limitMove(Vector2 direction)
    {
        // 画面左下のワールド座標をビューポートから取得
        Vector2 lowerLeft = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));

        // 画面右上のワールド座標をビューポートから取得
        Vector2 upperRight = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        // プレイヤーの座標を取得
        Vector2 pos = transform.position;

        // 移動量を加える
        pos += direction * GetComponent<Ship>().speed * Time.deltaTime;

        // プレイヤーの位置が画面内に収まるように制限をかける
        pos.x = Mathf.Clamp(pos.x, lowerLeft.x, upperRight.x);
        pos.y = Mathf.Clamp(pos.y, lowerLeft.y, upperRight.y);

        Move(pos - (Vector2)transform.position, speed);
    }
}
