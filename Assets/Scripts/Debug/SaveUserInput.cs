using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveUserInput
{
    public float[] rewardSelection;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }
}
