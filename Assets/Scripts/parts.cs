using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Parts : MonoBehaviour
{
    public Vector2 parentConnection = new Vector2(1, 0);
    public Vector2 selfConnection = new Vector2(-1, 0);
    public Vector2 parentConnectionLocal = new Vector2(1, 0);
    public Vector2 selfConnectionLocal = new Vector2(-1, 0);

    public Parts childParts;



    // Update is called once per frame
    void Update()
    {
        parentConnectionLocal = new Vector2(
            parentConnection.x * transform.parent.transform.lossyScale.x,
            parentConnection.y * transform.parent.transform.lossyScale.y
            );
        selfConnectionLocal = new Vector2(
            selfConnection.x * transform.lossyScale.x,
            selfConnection.y * transform.lossyScale.y
            );
        transform.position = transform.parent.transform.position + transform.parent.transform.rotation * parentConnectionLocal - transform.rotation * selfConnectionLocal;

    }
}
