using UnityEngine;
using System.Collections;

public class Roller : MonoBehaviour
{
    //private float digree = 0;
    //private int rightWeapon = 0;
    //private int leftWeapon = 0;
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

        // 移動の制限
        Ship.Move(Player.transform.position - transform.position, 0.3f);
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