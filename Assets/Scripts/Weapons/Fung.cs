using UnityEngine;
using System.Collections;

public class Fung : Sword
{
    public ActionType setAction(ActionType setedAction)
    {
        nowAction = setedAction;
        return nowAction;
    }
}
