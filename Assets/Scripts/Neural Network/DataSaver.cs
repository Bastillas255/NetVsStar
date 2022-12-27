using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Clase que se encargará de aplanar arrays 2d y 3d
public class DataSaver// : MonoBehaviour
{
    public int[] ds_layers;
    public float[][] ds_neurons;
    public float[][][] ds_weights;

    private float[] flatNeurons;
    private float[] flatWeigths;

    SaveData data = new SaveData();

    //Constructor que almacena una referencia de los arreglos originales
    public DataSaver(int[] layers, float[][] neurons, float[][][] weights)
    {
        ds_layers = layers;
        ds_neurons = neurons;
        ds_weights = weights;
    }

    public SaveData SaveNN()
    {
        //Aplana los arreglos de las neuronas y los pesos
        flatten2DArray(ds_neurons);
        flatten3DArray(ds_weights);

        //Guarda los datos en la clase SaveData
        data.saveLayers = ds_layers;
        data.saveNeurons = flatNeurons;
        data.saveWeights = flatWeigths;

        //Retorna la clase SaveData con los datos ya cargados y listos para su uso
        return data;
    }

    //Aplanamiento de arreglos de más de una dimensión

    //Arreglos 2D con largos variables
    private void flatten2DArray(float[][] neuronsMatrix)
    {
        //Obtiene el largo del array unidimensional sumando todos los largos de la matriz
        int array1DLength = neuronsMatrix.Sum(subArray => subArray.Length);

        //Inicializa su respectivo array y un índice para llenarlo
        flatNeurons = new float[array1DLength];
        int flatIndex = 0;

        for(int i = 0; i < neuronsMatrix.Length; i++)
        {
            for(int j = 0; j < neuronsMatrix[i].Length; j++)
            {
                //Llena el array unidimensional con los datos de la matriz
                flatNeurons[flatIndex] = neuronsMatrix[i][j];
                flatIndex++;
            }
        }

    }

    //Arreglos 3D con largos variables
    private void flatten3DArray(float[][][] weigthsMatrix)
    {
        //Obtiene el largo del array unidimensional sumando los largos que existan en las tres dimensiones de la matriz
        int array1DLength = weigthsMatrix.Sum(innerArray => innerArray.Sum(evenInnerArray => evenInnerArray.Length));

        //Inicializa su correspondiente array unidimensional y su índice
        flatWeigths = new float[array1DLength];
        int flatIndex = 0;

        for(int i = 0; i < weigthsMatrix.Length; i++)
        {
            for(int j = 0; j < weigthsMatrix[i].Length; j++)
            {
                for(int k = 0; k < weigthsMatrix[i][j].Length; k++)
                {
                    //Llena el array con los datos de la matriz tridimensional
                    flatWeigths[flatIndex] = weigthsMatrix[i][j][k];
                    flatIndex++;
                }
            }
        }
    }

}
