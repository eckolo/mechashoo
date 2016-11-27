using UnityEngine;
using System.Collections;

public class SEroot : Methods {
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if(!GetComponent<AudioSource>().isPlaying)
            selfDestroy();
    }
}
