using System.Collections.Generic;
using UnityEngine;

//Clase que almacena datos de entrenamiento de la red neuronal en formato JSON
//Para eso, los arreglos de más de una dimensión se deben "aplanar"
[System.Serializable]
public class SaveData
{
    public int[] saveLayers;
    public float[] saveNeurons;
    public float[] saveWeights;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }
}