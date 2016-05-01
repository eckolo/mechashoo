using UnityEngine;
using System.Collections;

public class Roller : MonoBehaviour
{
    private float digree = 0;
    private int rightWeapon = 0;
    private int leftWeapon = 0;
    private GameObject Player;

    // Use this for initialization
    void Start()
    {
        Player = GameObject.Find("player");
    }

    // Update is called once per frame
    void Update()
    {
        var Ship = GetComponent<Ship>();
        var Root = GetComponent<Root>();
        // 右・左
        float keyValueX = Input.GetAxisRaw("Horizontal");

        // 上・下
        float keyValueY = Input.GetAxisRaw("Vertical");

        // 移動する向きを求める
        Vector2 direction = new Vector2(keyValueX, keyValueY).normalized;

        // 移動の制限
        Ship.Move(Vector2.left, 0.3f);

        digree += keyValueY * -1;
        //transform.rotation = Quaternion.AngleAxis(digree, Vector3.forward);

        if (Input.GetKey(KeyCode.Z))
        {
            foreach (var weapon in Ship.weapons)
            {
                weapon.Action(weapon.gameObject.transform);
            }
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Ship.armPosition.x += keyValueX / 200 * (Ship.positive ? 1 : -1);
            Ship.armPosition.y += keyValueY / 200;
        }

        Vector2 target;
        try
        {
            target = new Vector2(
               (Player.transform.position - transform.position).x * (Ship.positive ? 1 : -1),
               (Player.transform.position - transform.position).y
               );
        }
        catch
        {
            target = (Vector2)transform.position + Vector2.left;
        }
    }
}