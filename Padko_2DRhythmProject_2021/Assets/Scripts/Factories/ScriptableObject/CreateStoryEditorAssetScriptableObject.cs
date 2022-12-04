using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// CreateStoryEditorAssetScriptableObject


[CreateAssetMenu(fileName = "StoryEditorAssetScriptableObject", menuName = "CreateUI/Create New StoryEditorAsset")]
public class CreateStoryEditorAssetScriptableObject : IScriptableObject
{
    public string storyName;
    public StoryDefinit definit;
}

//Definit
[System.Serializable]
public class StoryDefinit
{
    public SerializableDictionary<string, StoryChara> charas;
    public SerializableDictionary<string, StoryChara> items;
    public SerializableDictionary<string, string> test;
}

[System.Serializable]
public class StoryChara
{
    public SerializableDictionary<string, string> images;//存圖片路徑
    public SerializableDictionary<string, bool> boolAttributes;
    public SerializableDictionary<string, float> floatAttributes;
    public SerializableDictionary<string, string> stringAttributes = new SerializableDictionary<string, string>();
}

//FlowChart

