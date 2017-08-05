using UnityEngine;
using System.Collections;

public abstract class MainStage4Origin : Stage
{
    public override bool challengeable
    {
        get {
            if(sys.storyPhase >= Configs.StoryPhase.GAME_CLEAR) return true;
            return sys.storyPhase >= 4;
        }
    }

    protected override IEnumerator SuccessAction()
    {
        if(!isCleared && sys.storyPhase < Configs.StoryPhase.END_FOUR_PRACTICE) sys.storyPhase++;
        yield break;
    }
}
