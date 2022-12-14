using System.Collections.Generic;
using System;

public class NeuralNetwork : IComparable<NeuralNetwork>
{
    //Las capas de neuronas, si se usa un array [2,2,1] habrá una capa input de 2 neuronas, una capa oculta de 2 neuronas y una capa output de 1 neurona
    private int[] layers; 
    //Matriz de neuronas
    private float[][] neurons;
    //Pesos, representación de las conexiones que tendrán las neuronas, no toma en cuenta las neuronas de Input.
    //[Capa][Neurona][Conexión con neurona de capa anterior]
    private float[][][] weights; 

    private float fitness = 0; //El valor de Fitness de la red

    ///<summary>
    ///Inicializa la red neuronal con pesos aleatorios
    ///</summary>
    ///<param name="layers">Capas de la red neuronal</param>
    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for(int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        //Genera la matriz
        InitNeurons();
        InitWeigths();
    }

    ///<summary>
    ///Copia una red neuronal ya existente
    ///</summary>
    ///<param name="copyNetwork">Red neuronal a copiar</param>
    public NeuralNetwork(NeuralNetwork copyNetwork)
    {
        this.layers = new int[copyNetwork.layers.Length];
        for(int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i];
        }

        InitNeurons();
        InitWeigths();

        CopyWeights(copyNetwork.weights);
    }

    ///<summary>
    ///Constructor en base a clonar datos de una Red Neuronal, en caso de usar un archivo en lugar de una clase existente
    ///</summary>
    ///<param name="copyLayers">Array de capas o variable "layers"</param>
    ///<param name="copyNeurons">Array 2D de neuronas o variable "neurons"</param>
    ///<param name="copyWeights">Array 3D de pesos o variable "weights"</param>
    ///<param name="copyFitness">Valor de Fitness</param>
    public NeuralNetwork(int[] copyLayers, float[][] copyNeurons, float[][][] copyWeights, float copyFitness)
    {
        CopyLayers(copyLayers);
        CopyNeurons(copyNeurons);
        CopyWeights(copyWeights);
        SetFitness(copyFitness);
    }

    private void CopyWeights(float[][][] copyWeights)
    {
        for(int i = 0; i < weights.Length; i++)
        {
            for(int j = 0; j < weights[i].Length; j++)
            {
                for(int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }

    ///<summary>
    ///Crea la matriz de neuronas
    ///</summary>
    private void InitNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();

        for(int i = 0; i < layers.Length; i++) //Corre por todas las capas
        {
            neuronsList.Add(new float[layers[i]]); //Añade una capa a la lista de neuronas
        }

        neurons = neuronsList.ToArray(); //Convierte la lista en un array
    }

    ///<summary>
    ///Crea la matriz de pesos
    ///</summary>
    private void InitWeigths()
    {
        List<float[][]> weightsList = new List<float[][]>(); //Lista de pesos que después se transformará en un array

        //Itera sobre todas las neuronas que tienen una conexión de input
        for(int i = 1; i < layers.Length; i++) //Empieza con la primera capa oculta en lugar de la capa de Input
        {
            List<float[]> layerWeightsList = new List<float[]>();

            int neuronsInPreviousLayer = layers[i-1];

            //Itera sobre todas las neuronas en la capa actual
            for(int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];

                //Itera sobre todas las neuronas en la capa anterior y asigna pesos aleatorios al azar entre 0.5 y -0.5
                for(int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    //Dar pesos aleatorios a las neuronas
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                layerWeightsList.Add(neuronWeights); //Añade los pesos de las neuronas de la capa actual a la lista de pesos
            }

            weightsList.Add(layerWeightsList.ToArray()); //Convierte los pesos de la lista de pesos de la capa actual a un array 2D y lo añade a la lista de pesos "general"
        }

        weights = weightsList.ToArray(); //Convierte la lista en un Array 3D
    }

    private void CopyLayers(int[] copyLayers)
    {
        layers = new int[copyLayers.Length];
        for(int i = 0; i < copyLayers.Length; i++)
        {
            layers[i] = copyLayers[i];
        }
    }

    private void CopyNeurons(float[][] copyNeurons)
    {
        InitNeurons();
        for(int i=0; i<neurons.Length; i++)
        {
            for(int j=0; j<neurons[i].Length; j++)
            {
                neurons[i][j] = copyNeurons[i][j];
            }
        }
    }

    public int[] GetLayers()
    {
        return this.layers;
    }
    public float[][] GetNeurons()
    {
        return this.neurons;
    }
    public float[][][] GetWeights()
    {
        return this.weights;
    }

    ///<summary>
    ///Realiza un FeedForward a la red neuronal
    ///</summary>
    ///<param name="inputs">Los valores que recibe cada neurona de la capa de Input</param>
    ///<returns></returns>
    public float[] FeedForward(float[] inputs)
    {
        //Añade los inputs a la matriz de neuronas
        for(int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        //Itera sobre todas las neuronas y calcula los valores 
        for(int i = 1; i < layers.Length; i++)
        {
            for(int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0.25f;
                for(int k = 0; k < neurons[i-1].Length; k++)
                {
                    value += weights[i-1][j][k] * neurons[i-1][k]; //Suma de todas las conexiones que tiene esta neurona con la capa anterior
                }

                neurons[i][j] = (float)Math.Tanh(value); //Aplica función de activación: Tangente Hiperbólica
            }
        }
        return neurons[neurons.Length-1]; //Retorna la capa de output
    }

    public void Mutate()
    {
        for(int i = 0; i < weights.Length; i++)
        {
            for(int j = 0; j < weights[i].Length; j++)
            {
                for(int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];

                    //Realiza una mutación al valor del peso

                    //Elige al azar una mutación
                    float randomNumber = UnityEngine.Random.Range(0f, 10f);

                    //4 tipos distintos de mutaciones
                    if(randomNumber <= 2f)
                    {
                        //Invierte el signo del peso
                        weight *= -1f;
                    } else if(randomNumber <= 4f)
                    {
                        //Elige un peso aleatorio entre -1 y 1
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    } else if(randomNumber <= 6f)
                    {
                        //Aumenta al azar entre un 0% y un 100%
                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
                        weight *= factor;
                    } else if(randomNumber <= 8f)
                    {
                        //Disminuye al azar entre un 0% y un 100%
                        float factor = UnityEngine.Random.Range(0f, 1f);
                        weight *= factor;
                    }

                    weights[i][j][k] = weight;
                }
            }
        }
    }

    public void AddFitness(float fit)
    {
        fitness += fit;
    }

    public void SetFitness(float fit)
    {
        fitness = fit;
    }

    public float GetFitness()
    {
        return fitness;
    }

    ///<summary>
    ///Compara dos redes neuronales basado en sus fitness
    ///</summary>
    ///<param name="other">Red neuronal a comparar con esta</param>
    ///<returns>1 si el fitness de esta red es mayor al de la otra, -1 si es menor y 0 si son iguales</returns>
    public int CompareTo(NeuralNetwork other)
    {
        if(other == null) return 1;

        if(fitness > other.fitness)
        {
            return 1;
        }
        else if(fitness < other.fitness)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}
