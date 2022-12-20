using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager : MonoBehaviour
{

    //Ruta que tendrá el archivo dentro del proyecto
    string path;
    //Ruta de archivos de guardado de las matrices de una red neuronal ya entrenada
    string layersSavePath;
    string neuronsSavePath;
    string weightsSavePath;

    ///<summary>
    ///Crea el archivo, comprueba si es que existe tal archivo, de  no existir lo crea
    ///</summary>
    public void CreateText()
    {
        path = Application.dataPath + "/Traza.txt";
        
        //Crea el archivo si es que no existe
        if(!File.Exists(path))
        {
            //OJO, esta función reemplaza todo el archivo
            File.WriteAllText(path, "Turno Comportamiento JugadorX JugadorY EnemigoX EnemigoY RecompensasRestantes DistJE DistJR DistER ParedesJE ParedesJR ParedesER\n\n");
        }
    }

    ///<summary>
    ///Añade texto al archivo, comprueba primero si existe
    ///</summary>
    ///<param name="content">String con contenido que se añadirá al archivo de texto</param>
    public void AddText(string content)
    {
        if(!File.Exists(path))
        {
            CreateText();
        }
        File.AppendAllText(path, content+"\n"); //Esta función agrega contenido al archivo de texto, adicionalmente se agrega un salto de línea
    }
    // Start is called before the first frame update
    void Start()
    {
        CreateText();
    }

    ///<summmary>
    ///A partir de las matrices generadas por una red neuronal, crea archivos de guardado para estas.
    ///</summary>
    ///<param name="layersSave">Arreglo de capas de neuronas</param>
    ///<param name="neuronsSave">Matriz bidimensional de neuronas</param>
    ///<param name="weightsSave">Matriz tridimensional de pesos</param>
    public void CreateTrainingSave(int[] layersSave, float[][] neuronsSave, float[][][] weightsSave)
    {
        //Define las rutas de los archivos para guardar el entrenamiento de la ren neuronal
        layersSavePath = Application.dataPath + "/LayersSave.txt";
        neuronsSavePath = Application.dataPath + "/NeuronsSave.txt";
        weightsSavePath = Application.dataPath + "/WeightsSave.txt";

        //Si ninguno de los archivos existe, los crea, se pregunta por separado en caso de existir un archivo previamente
        if(!File.Exists(layersSavePath)) File.WriteAllText(layersSavePath,"");
        if(!File.Exists(neuronsSavePath)) File.WriteAllText(neuronsSavePath,"");
        if(!File.Exists(weightsSavePath)) File.WriteAllText(weightsSavePath,"");

        File.AppendAllText(layersSavePath, layersToString(layersSave));
        File.AppendAllText(neuronsSavePath, neuronsToString(neuronsSave));
        File.AppendAllText(weightsSavePath, weightsToString(weightsSave));
    }

    ///<summary>
    ///Convierte un arreglo de enteros a string
    ///</summary>
    ///<param="layersSave">Array de enteros que representa las capas de neuronas</param>
    ///<returns>String del arreglo de números</returns>
    private string layersToString(int[] layersSave)
    {
        string content = "";
        for(int i=0; i<layersSave.Length;i++)
        {
            content = content + layersSave[i] + " ";
        }
        return content;
    }

    ///<summary>
    ///Convierte una matriz bidimensional de float a string
    ///</summary>
    ///<param="neuronsSave">Matriz bidimensional de float que representa las neuronas</param>
    ///<returns>String de la matriz bidimensional de números</returns>
    private string neuronsToString(float[][] neuronsSave)
    {
        string content = "";
        for(int i=0;i<neuronsSave.Length;i++)
        {
            for(int j=0;j<neuronsSave[i].Length;j++)
            {
                content = content + neuronsSave[i][j] + " ";
            }
            content = content + "\n";
        }

        return content;
    } 

    ///<summary>
    ///Convierte una matriz tridimensional de float a string
    ///</summary>
    ///<param="weightsSave">Matriz tridimensional de float que representa las conexiones entre neuronas</param>
    ///<returns>String de la matriz tridimensional de números</returns>
    private string weightsToString(float[][][] weightsSave)
    {
        string content = "";
        for(int i=0;i<weightsSave.Length;i++)
        {
            for(int j=0;j<weightsSave[i].Length;j++)
            {
                for(int k=0;k<weightsSave[i][j].Length;k++)
                {
                    content = content + weightsSave[i][j][k] + " ";
                }
                content = content + "\n";
            }
            content = content + "\n";
        }
        return content;
    }
}