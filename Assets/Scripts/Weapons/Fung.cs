using UnityEngine;
using System.Collections;

public class Fung : Sword
{
    public ActionType setActionType(ActionType actionType)
    {
        nowAction = actionType;
        return nowAction;
    }
}
