using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Manager : MonoBehaviour
{
    //NN variables
    private int[] layers = new int[] { 11, 10, 10, 2}; //first array is the input layer, second and third are hidden layers, lastly is the output layer 
    private NeuralNetwork net;

    //Variables to Retrive Data from txt files
    private FileManager fm;
    SaveUserInput sui;
    SaveTraceData std;
    float[] stdArray = new float[11];
    List<string> lines;
    string[] traceData;
    string[] userInputData;
    public Vector2 objective;

    //pac stuff
    public GameObject pacPrefab;
    PacManMovement pac;
    [SerializeField]
    private Transform spawnSpot;

    private void Start()
    {
        //this initialization may not be necessary (retrive data from txt)
        fm = new FileManager();
        sui = new SaveUserInput();
        std = new SaveTraceData();
        lines = new List<string>();

        //create random weighted NN
        //InitNeuralNetworks();

        //we create pacman,sending the data of the neural network and the txts
        CreatePacman();
    }
    

    void InitNeuralNetworks()
    {
        NeuralNetwork net = new NeuralNetwork(layers);
        net.Mutate();
    }

    void LoadDataFromFiles()
    {
        Debug.Log("is net alright?1: " + net);
        List<string> inputDataList = fm.GetListOfLines("UserInputs.txt");
        userInputData = inputDataList.ToArray();
        List<string> traceDataList = fm.GetListOfLines("TraceData.txt");
        traceData = traceDataList.ToArray();
    }

    //pac stuff
    private void CreatePacman()
    {
        NeuralNetwork net = new NeuralNetwork(layers);
        net.Mutate();
        LoadDataFromFiles();

        //Creamos un objeto Pacman nuevo
        pac = ((GameObject)Instantiate(pacPrefab, spawnSpot.transform.position, pacPrefab.transform.rotation)).GetComponent<PacManMovement>();
        //Inicializamos un pacman con un objetivo y una red neuronal
        //Las clases se pasan por referencia, por lo tanto, la red neuronal que se pasa debería
        //de recibir el entrenamiento

        objective = Vector2.zero;
        pac.Init(net, objective);
        

        //Tanto traceData como userInputData tienen el mismo largo
        for (int i=1; i<traceData.Length; i++)
        {
            //Chequea si la línea actual es vacía o null, para evitar llamados innecesarios a TrainNN
            if(!string.IsNullOrEmpty(traceData[i]) && !string.IsNullOrEmpty(userInputData[i]))
            {
                //Cada vez se carga la información correspondiente al turno que se está leyendo
                ChangeTurn(i);
                //Se entrena la red neuronal
                pac.TrainNN(stdArray);
            }
        }
        Debug.Log("Training is done");
        //Guardar información de entrenamiento
        DataSaver ds = new DataSaver(net.GetLayers(), net.GetNeurons(), net.GetWeights());
        SaveData nnData = ds.SaveNN();
        fm.WriteToFile("TrainedNNData.txt", nnData.ToJson());
    }

    public void ChangeTurn(int turn)
    {
        //ChangeObjective
        sui.LoadFromJson(userInputData[turn]);
        if (sui.left == 1)
        {
            objective.x = -1;
        }
        if (sui.right == 1)
        {
            objective.x = 1;
        }
        if (sui.up == 1)
        {
            objective.y = 1;
        }
        if (sui.down == 1)
        {
            objective.y = -1;
        }

        //ChangeInputs
        std.LoadFromJson(traceData[turn]);

        stdArray[0] = std.std_playerXPos;
        stdArray[1] = std.std_playerYPos;
        stdArray[2] = std.std_enemyXPos;
        stdArray[3] = std.std_enemyYPos;
        stdArray[4] = std.std_closestRewardXPos;
        stdArray[5] = std.std_closestRewardYPos;
        stdArray[6] = std.std_distPlayerEnemy;
        stdArray[7] = std.std_distPlayerReward;
        stdArray[8] = std.std_distEnemyReward;
        stdArray[9] = std.std_rewardsObtained;
        stdArray[10] = std.std_turnCount;

        //we apply changes back to pacman
        pac.SetObjectiveAndInputs(objective);
    }

}