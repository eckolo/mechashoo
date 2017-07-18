using UnityEngine;
using System.Collections;

public class Fung : Sword
{
    public ActionType SetActionType(ActionType actionType)
    {
        nowAction = actionType;
        return nowAction;
    }
}
