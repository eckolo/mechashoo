using UnityEngine;
using System.Collections;

public class SEroot : Methods
{
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    public override void Update()
    {
        base.Update();
        if(!GetComponent<AudioSource>().isPlaying) selfDestroy();
    }
}
