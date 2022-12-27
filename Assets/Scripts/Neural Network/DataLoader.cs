using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase que obtiene un string en formato JSON y restaura la información que contiene a su formato original para una red neuronal
public class DataLoader : MonoBehaviour
{
    private SaveData dl_data;

    private NeuralNetwork nn;

    private float[][] dl_neurons;
    private float[][][] dl_weigths;

    //Constructor que recibe un string en formato JSON y almacena los datos en un SaveData
    public DataLoader(string json)
    {
        dl_data = JsonUtility.FromJson<SaveData>(json);
        nn = new NeuralNetwork(dl_data.saveLayers);
    }

    public int[] GetLayers()
    {
        List<int> list = new List<int>();
        list.AddRange(dl_data.saveLayers);

        return list.ToArray();
    }

    public float[][] GetNeurons()
    {
        float[][] auxMatrix = nn.GetNeurons();
        int flatIndex = 0;

        for(int i = 0; i < auxMatrix.Length; i++)
        {
            for(int j = 0; j < auxMatrix[i].Length; j++)
            {
                auxMatrix[i][j] = dl_data.saveNeurons[flatIndex];
                flatIndex++;
            }
        }
        return auxMatrix;
    }

    public float[][][] GetWeigths()
    {
        float[][][] auxMatrix = nn.GetWeights();
        int flatIndex = 0;

        for(int i = 0; i < auxMatrix.Length; i++)
        {
            for(int j = 0; j < auxMatrix[i].Length; j++)
            {
                for(int k = 0; k < auxMatrix[i][j].Length; k++)
                {
                    auxMatrix[i][j][k] = dl_data.saveWeights[flatIndex];
                    flatIndex++;
                }
            }
        }
        return auxMatrix;
    }

}
