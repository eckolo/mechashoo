using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class MainSystems : Stage
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
    ///HPバーオブジェクトの雛形
    /// </summary>
    public Bar basicBar = null;

    /// <summary>
    ///オープニング再生済みフラグ
    /// </summary>
    [SerializeField]
    private bool opening = false;

    /// <summary>
    ///各種キーのAxes名
    /// </summary>
    const string decisionName = "ShotRight";


    [SerializeField]
    private List<Ship> selectShip = new List<Ship>();

    // Use this for initialization
    public override IEnumerator Start()
    {
        Application.targetFrameRate = 120;
        yield return testAction();
        yield return openingAction();
        yield return startStage();

        GameObject.Find("player").GetComponent<Player>().canRecieveKey = true;

        yield break;
    }

    // Update is called once per frame
    public override void Update()
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

    IEnumerator testAction()
    {
        bool toNext = false;
        int selected = 0;

        while (!toNext)
        {
            selected = selected % selectShip.Count;
            setSysText("< " + selectShip[selected].gameObject.name + " >");
            GameObject.Find("player").GetComponent<Player>().copyShipStatus(selectShip[selected]);

            toNext = false;
            bool inputRightKey = false;
            bool inputLeftKey = false;

            while (!toNext && !inputRightKey && !inputLeftKey)
            {
                toNext = Input.GetKeyDown(KeyCode.Z);
                inputRightKey = Input.GetKeyDown(KeyCode.RightArrow);
                inputLeftKey = Input.GetKeyDown(KeyCode.LeftArrow);

                yield return null;
            }

            if (inputRightKey) selected += 1;
            if (inputLeftKey) selected += selectShip.Count - 1;

            yield return new WaitForSeconds(0.1f);
        }

        yield break;
    }

    IEnumerator openingAction()
    {
        if (!opening) Debug.Log("Exit Opening");
        yield break;
    }

    IEnumerator startStage()
    {
        if (stages.Count <= 0) yield break;

        StartCoroutine(countFPS());
        opening = true;
        nowStage = (Stage)Instantiate(stages[nowStageNum], new Vector2(0, 0), transform.rotation);
        nowStage.transform.parent = transform;

        yield break;
    }
}
