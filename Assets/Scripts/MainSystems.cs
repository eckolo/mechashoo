using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MainSystems : MonoBehaviour
{
    /// <summary>
    ///ステージリスト
    /// </summary>
    [SerializeField]
    private List<Stage> stages = new List<Stage>();

    // Use this for initialization
    void Start()
    {
        ((Stage)Instantiate(stages[0], new Vector2(0, 0), transform.rotation)).transform.parent = transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
