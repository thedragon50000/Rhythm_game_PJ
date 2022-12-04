using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "AssetScriptableObject", menuName = "CreateUI/Create New Asset")]
public class CreateAssetScriptableObject : IScriptableObject
{
    public CreateButton button;
    public CreateImage image;
    public CreateInputField inputField;
}


[System.Serializable]
public class CreateButton
{
    public GameObject menuButton;
    public GameObject storyButton;
    public GameObject addButton;
}

[System.Serializable]
public class CreateInputField
{
    public GameObject charaListInput;
    public GameObject charaAttrInput;

}

[System.Serializable]
public class CreateImage
{
    public SerializableDictionary<string, Sprite> storyButtonImage;

    //public SerializableDictionary<string, List<GameObject>> gameObjects;
    //public SerializableDictionary<string, SerializableDictionary<string, Color>> colors;

}

public enum ButtonTypeFlag
{
    none,
    Story
}
