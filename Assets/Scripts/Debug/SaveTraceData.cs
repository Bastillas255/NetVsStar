using System.Collections.Generic;
using UnityEngine;

//Clase que almacenará en formato JSON la información del turno
[System.Serializable]
public class SaveTraceData
{
    //Posición del jugador
    public float std_playerXPos;
    public float std_playerYPos;

    //Posición del enemigo
    public float std_enemyXPos;
    public float std_enemyYPos;

    //Posición de la recompensa más cercana
    public float std_closestRewardXPos;
    public float std_closestRewardYPos;
    public int std_distPlayerEnemy;
    public int std_distPlayerReward;
    public int std_distEnemyReward;
    public int std_rewardsObtained;
    public int std_turnCount;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }

}
