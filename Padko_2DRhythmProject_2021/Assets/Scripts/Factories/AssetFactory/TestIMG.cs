using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestIMG : MonoBehaviour
{
    public static TestIMG instance;
    public void IMGSwitch()
    {
        //var definit = FactoryManager.Instance.localAssetFactory.mCreateStoryEditorAsset.definit;
        //var tex2d = definit.charas[0].images["³ß"];
        //var sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0.5f, 0.5f));
        //GetComponent<Image>().sprite = sprite; 
    }

    void Awake()
    {
        //instance = this;
        //var definit = FactoryManager.Instance.localAssetFactory.mCreateStoryEditorAsset.definit;
        //if (definit.charas[0].images["³ß"] != null)
        //{
        //    var tex2d = definit.charas[0].images["³ß"];
        //    var sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0.5f, 0.5f));
        //    GetComponent<Image>().sprite = sprite;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
