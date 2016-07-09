using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class MainSystems : Mthods
{
    /// <summary>
    ///ステージリスト
    /// </summary>
    [SerializeField]
    private List<Stage> stages = new List<Stage>();
    /// <summary>
    ///現在のステージ番号
    /// </summary>
    [SerializeField]
    private int nowStageNum = 0;
    /// <summary>
    ///現在のステージオブジェクト
    /// </summary>
    public Stage nowStage = null;

    /// <summary>
    ///オープニング再生済みフラグ
    /// </summary>
    [SerializeField]
    private bool opening = false;


    [SerializeField]
    private List<Ship> selectShip = new List<Ship>();

    // Use this for initialization
    IEnumerator Start()
    {
        Application.targetFrameRate = 120;
        yield return testAction();
        yield return openingAction();
        yield return startStage();
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        //setSysText("" + Application.targetFrameRate);
        flamecount++;
    }

    int flamecount = 0;
    IEnumerator countFPS()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            setSysText("fps:" + flamecount + ":" + 1 / Time.deltaTime);
            flamecount = 0;
        }
    }

    {
    }

    IEnumerator openingAction()
    {
        if (!opening) Debug.Log("Exit Opening");
        yield break;
    }

    IEnumerator startStage()
    {
        StartCoroutine(countFPS());
        opening = true;
        nowStage = (Stage)Instantiate(stages[nowStageNum], new Vector2(0, 0), transform.rotation);
        nowStage.transform.parent = transform;
        yield break;
    }
}
