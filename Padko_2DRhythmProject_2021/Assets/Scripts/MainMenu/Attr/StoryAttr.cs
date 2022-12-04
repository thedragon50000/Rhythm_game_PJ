using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryAttr 
{
    public int storyDays;
    public int favorability;
    public int storyEnding;
    public List<int> dontRepeatStoryList = new List<int>();//用來讓出現過的事件不要重複
}
