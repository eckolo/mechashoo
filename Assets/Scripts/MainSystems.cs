using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class MainSystems : MonoBehaviour
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

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 120;
        StartCoroutine(countFPS());
        if (!opening) openingAction();
        opening = true;
        nowStage = (Stage)Instantiate(stages[nowStageNum], new Vector2(0, 0), transform.rotation);
        nowStage.transform.parent = transform;
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

    void openingAction()
    {
    }

    /// <summary>
    ///システムテキストへの文字設定
    /// </summary>
    protected void setSysText(string setText)
    {
        GameObject.Find("SystemText").GetComponent<Text>().text = setText;
    }
    /// <summary>
    ///システムテキストの取得
    /// </summary>
    protected string getSysText()
    {
        return GameObject.Find("SystemText").GetComponent<Text>().text;
    }
}
