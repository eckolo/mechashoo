using UnityEngine;
using System.Collections;

public class Roller : Ship
{
    //private float digree = 0;
    //private int rightWeapon = 0;
    //private int leftWeapon = 0;
    private GameObject Player;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        Player = GameObject.Find("player");
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        // 移動の制限
        Move(Player.transform.position - transform.position, 0.3f);
        /*
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
        */
    }
}