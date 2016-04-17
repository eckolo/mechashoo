using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public Vector2 armPosition = new Vector2(0, 0);
    public Vector2 wingPosition = new Vector2(0, 0);

    private float digree = 0;
    private Ship ship;

    // Use this for initialization
    void Start()
    {
        ship = GetComponent<Ship>();
    }

    // Update is called once per frame
    void Update()
    {
        // 右・左
        float keyValueX = Input.GetAxisRaw("Horizontal");

        // 上・下
        float keyValueY = Input.GetAxisRaw("Vertical");

        // 移動する向きを求める
        Vector2 direction = new Vector2(keyValueX, keyValueY).normalized;

        // 移動の制限
        if (!Input.GetKey(KeyCode.LeftShift)) Move(direction);

        digree += keyValueY * -1;
        //transform.rotation = Quaternion.AngleAxis(digree, Vector3.forward);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            foreach (var weapon in GetComponent<Ship>().weapons)
            {
                weapon.GetComponent<Weapon>().Action(weapon.transform);
            }
        }

        var Root = GetComponent<Root>();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            armPosition.x += keyValueX / 100;
            armPosition.y += keyValueY / 100;
        }

        armPosition = Root.setManipulatePosition(armPosition, Root.childPartsList[0]);

        var baseWingPosition = new Vector2(-6, 1).normalized * 2 / 3;
        wingPosition.x = (!Input.GetKey(KeyCode.LeftShift) && keyValueY != 0)
            ? wingPosition.x - keyValueY / 100
            : wingPosition.x * 9 / 10;
        wingPosition.y = (!Input.GetKey(KeyCode.LeftShift) && keyValueX != 0)
            ? wingPosition.y + keyValueX / 100
            : wingPosition.y * 9 / 10;
        if (wingPosition.magnitude > 1) wingPosition = wingPosition.normalized;
        wingPosition = Root.setManipulatePosition(wingPosition + baseWingPosition, Root.childPartsList[1], false) - baseWingPosition;
    }

    // 機体の移動
    void Move(Vector2 direction)
    {
        // 画面左下のワールド座標をビューポートから取得
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));

        // 画面右上のワールド座標をビューポートから取得
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        // プレイヤーの座標を取得
        Vector2 pos = transform.position;

        // 移動量を加える
        pos += direction * GetComponent<Ship>().speed * Time.deltaTime;

        // プレイヤーの位置が画面内に収まるように制限をかける
        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);

        ship.Move(pos - (Vector2)transform.position, ship.speed);
    }
}
