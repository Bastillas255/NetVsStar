using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase que obtiene un SaveData y restaura la información que contiene a su formato original
public class DataLoader : MonoBehaviour
{
    SaveData dl_data;

    float[][] dl_neurons;
    float[][][] dl_weigths;

    public DataLoader(string json)
    {
        dl_data = JsonUtility.FromJson<SaveData>(json);
    }


}
