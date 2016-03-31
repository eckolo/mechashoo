using UnityEngine;
using System.Collections;

public class parts : MonoBehaviour
{
    public bool root = false;
    public bool positive = true;
    public Vector2 parentConnection = new Vector2(1, 0);
    public Vector2 selfConnection = new Vector2(-1, 0);
    private Vector2 parentConnectionLocal = new Vector2(1, 0);
    private Vector2 selfConnectionLocal = new Vector2(-1, 0);

    private float angle = 0;

    // Use this for initialization
    void Start()
    {
    }

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
        transform.position = transform.parent.transform.position
            + transform.parent.transform.rotation * parentConnectionLocal
            - transform.rotation * selfConnectionLocal;

    }
}
