using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAssetFactory : MonoBehaviour
{
    [SerializeField]
    public CreateAssetScriptableObject mCreateAsset;
    [SerializeField]
    public CreateStoryEditorAssetScriptableObject mCreateStoryEditorAsset;
    public string mSavePath;

    public delegate void OnClickDelegate();

    public static GameObject SpawnGameObject(GameObject obj, Vector3 pos = default, Transform parent = default, float destoryTime = default)
    {
        var spawnObj = Instantiate(obj);
        if(spawnObj != default)
            spawnObj.transform.SetParent(parent);
        if(pos == default)
            spawnObj.transform.position = Vector3.zero;
        else
        {
            spawnObj.transform.position = pos;
        }
        spawnObj.transform.localScale = Vector3.one;
        spawnObj.transform.rotation = Quaternion.identity;
        if (destoryTime != default)
            Destroy(spawnObj, destoryTime);
        return spawnObj;
    }

    private void Awake()
    {
        mSavePath = Application.streamingAssetsPath + "/Data/Json";
    }


}








