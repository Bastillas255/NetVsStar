using System.Collections.Generic;
using UnityEngine;

//Clase que almacenará en formato JSON la información del turno
[System.Serializable]
public class SaveTraceData
{
    //Posición del jugador
    public float std_playerXPos;
    public float std_playerYPos;

    //Posición de la recompensa más cercana
    public float std_Reward1XPos;
    public float std_Reward1YPos;

    public float std_Reward2XPos;
    public float std_Reward2YPos;

    public float std_Reward3XPos;
    public float std_Reward3YPos;

    public float std_Reward4XPos;
    public float std_Reward4YPos;

    public float std_Reward1Grabed;
    public float std_Reward2Grabed;
    public float std_Reward3Grabed;
    public float std_Reward4Grabed;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }

}
