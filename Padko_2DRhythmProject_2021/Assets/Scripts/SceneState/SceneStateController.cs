using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneStateController : MonoBehaviour
{
    private ISceneState mState;
    private AsyncOperation mAO;
    private bool mIsRunStart = false;
    public void SetState(ISceneState state, bool isLoadScene=true)
    {
        
        if (mState != null)
        {
            mState.StateEnd();
        }
        mState = state;
        if(isLoadScene)
        {
            mAO = SceneManager.LoadSceneAsync(mState.SceneName);
            mIsRunStart = false;
        }
        else
        {
            mState.StateStart();
            mIsRunStart = true;
        }
    }

    public void StateUpdate()
    {
        if (mAO != null && !mAO.isDone) return;
        if(!mIsRunStart && mAO != null && mAO.isDone)
        {
            mState.StateStart();
            mIsRunStart = true;
        }
        if(mState != null)
        {
            mState.StateUpdate();
        }
    }
}
