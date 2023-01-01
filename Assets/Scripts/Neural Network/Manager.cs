using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Manager : MonoBehaviour
{
    //NN variables
    private int[] layers = new int[] { 14, 10, 10, 4}; //first array is the input layer, second and third are hidden layers, last is the output layer 
    private NeuralNetwork net;

    //Variables to Retrive Data from txt files
    private FileManager fm;
    SaveTraceData std;
    float[] stdArray = new float[14];
    string[] traceData;

    //pac stuff
    public GameObject pacPrefab;
    PacManMovement pac;
    [SerializeField]
    private Transform spawnSpot;

    private void Start()
    {
        //this initialization may not be necessary (retrive data from txt)
        fm = new FileManager();
        std = new SaveTraceData();

        //we create pacman,sending the data of the neural network and the txts
        CreatePacman();
    }

    //pac stuff
    private void CreatePacman()
    {
        //InitNeuralNetworks
        NeuralNetwork net = new NeuralNetwork(layers);
        net.Mutate();
        LoadDataFromFiles();

        //Creamos un objeto Pacman nuevo
        pac = ((GameObject)Instantiate(pacPrefab, spawnSpot.transform.position, pacPrefab.transform.rotation)).GetComponent<PacManMovement>();
        //Inicializamos un pacman con un objetivo y una red neuronal
        //Las clases se pasan por referencia, por lo tanto, la red neuronal que se pasa debería
        //de recibir el entrenamiento

        pac.Init(net);

        //Tanto traceData como userInputData tienen el mismo largo
        for (int i=1; i<traceData.Length; i++)
        {
            //Chequea si la línea actual es vacía o null, para evitar llamados innecesarios a TrainNN
            if(!string.IsNullOrEmpty(traceData[i]))
            {
                for (int j = 0; j < traceData.Length; j += 2)
                {
                    if (traceData[0] == traceData[j])//checks if the data is from a turn when a reward is taken
                    {
                        if (traceData[1] == traceData[j + 1])
                        {
                            
                            //Cada vez se carga la información correspondiente al turno que se está leyendo
                            ChangeTurn(i);
                            //Se entrena la red neuronal
                            pac.TrainNN(stdArray);
                        }
                        
                    }
                }
                
               
            }
        }
        Debug.Log("Training is done");
        //Guardar información de entrenamiento
        DataSaver ds = new DataSaver(net.GetLayers(), net.GetNeurons(), net.GetWeights());
        SaveData nnData = ds.SaveNN();
        fm.WriteToFile("TrainedNNData", nnData.ToJson());
    }

    void LoadDataFromFiles()
    {
        List<string> traceDataList = fm.GetListOfLines("TraceData.txt");
        traceData = traceDataList.ToArray();
    }

    public void ChangeTurn(int turn)
    {
        //ChangeInputs
        std.LoadFromJson(traceData[turn]);

        stdArray[0] = std.std_playerXPos;
        stdArray[1] = std.std_playerYPos;

        stdArray[2] = std.std_Reward1XPos;
        stdArray[3] = std.std_Reward1YPos;

        stdArray[4] = std.std_Reward2XPos;
        stdArray[5] = std.std_Reward2YPos;

        stdArray[6] = std.std_Reward3XPos;
        stdArray[7] = std.std_Reward3YPos;

        stdArray[8] = std.std_Reward4XPos;
        stdArray[9] = std.std_Reward4YPos;

        stdArray[10] = std.std_Reward1Grabed;
        stdArray[11] = std.std_Reward2Grabed;
        stdArray[12] = std.std_Reward3Grabed;
        stdArray[13] = std.std_Reward4Grabed;
    }
}