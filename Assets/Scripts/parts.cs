using UnityEngine;
using System.Collections;

public class parts : MonoBehaviour
{
    public bool root = false;
    public bool positive = true;
    public Vector2 parentConnection = new Vector2(1, 0);
    public Vector2 selfConnection = new Vector2(-1, 0);

    private float angle = 0;

    // Use this for initialization
    void Start()
    {
        transform.position = parentConnection - selfConnection;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 0.1f));
        transform.position = transform.parent.transform.position
            + transform.parent.transform.rotation * parentConnection
            - transform.rotation * selfConnection;

    }

    // Update is called once per frame
    void setPosition()
    {

    }
}
