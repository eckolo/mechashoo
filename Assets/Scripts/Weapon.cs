using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    public bool canAction;
    private bool canStartAction;
    public List<Motion> motions = new List<Motion>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Action(Transform origin, int actionNumber = 0)
    {
        motions[actionNumber].actionroot(origin);
    }
}
