using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveUserInput
{
    public int up;
    public int down;
    public int left;
    public int right;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }
}
