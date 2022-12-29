using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using System.IO;

public class Manager : MonoBehaviour
{
    //NN variables
    private int[] layers = new int[] { 11, 10, 10, 2}; //first array is the input layer, second and third are hidden layers, lastly is the output layer 
    private NeuralNetwork net;

    //Variavles to Retrive Data from txt files
    private SaveData save;
    private NeuralNetwork savedNN;
    private FileManager fm;
    SaveUserInput sui;
    SaveTraceData std;
    List<string> lines;
    string[] aux;
    public Vector2 objective;

    //pac stuff
    public GameObject pacPrefab;
    PacManMovement pac;
    [SerializeField]
    private Transform spawnSpot;

    //this variables may not be necessary
    [SerializeField]
    private float minimumFitness=1f;
    int currentTurn=0;


    private void Start()
    {
        //this initialization may not be necessary (retrive data from txt)
        fm = new FileManager();
        sui = new SaveUserInput();
        lines = new List<string>();

        //create random weighted NN
        InitNeuralNetworks();

        
        //we create pacman,sending the data of the neural network and the txts
        CreatePacman();

        //Lets train him
        pac.TrainNN();
    }

    //void Update()
    //{
    //    //if over minimum fitness start training the same net but with the objective of the next turn
    //    if (net.GetFitness()> minimumFitness)
    //    {
    //        currentTurn++;
    //        ChangeObjective(currentTurn);
    //    }
    //    //timer has passed and now we decide what to do with net
    //    net.Mutate();

    //    //before this line there was the mass killing of unfit nets,
    //    net.SetFitness(0f); //Reinicia el fitness de todas las redes neuronales

    //    //SaveAndLoad();
    //}

    //private void SaveAndLoad()
    //{
    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        nets.Sort();
    //        DataSaver ds = new DataSaver(nets[populationSize - 1].GetLayers(), nets[populationSize - 1].GetNeurons(), nets[populationSize - 1].GetWeights());
    //        save = ds.SaveNN();
    //        fm.WriteToFile("NNInternalData.txt", save.ToJson());
    //    }

    //    if (Input.GetKeyDown(KeyCode.L))
    //    {
    //        DataLoader dl = new DataLoader(fm.ReadFile("NNTrainingData.txt"));

    //        //DataSaver ds = new DataSaver(dl.GetLayers(), dl.GetNeurons(), dl.GetWeigths());
    //        //SaveData testSave = ds.SaveNN();
    //        //fm.WriteToFile("NNTEST.txt", testSave.ToJson());
    //        //LoadByJson();
    //    }
    //}


    


    void InitNeuralNetworks()
    {
        NeuralNetwork net = new NeuralNetwork(layers);
        net.Mutate();
    }

    //pac stuff
    private void CreatePacman()
    {
        
        //we create a new pacman object
        pac = ((GameObject)Instantiate(pacPrefab, spawnSpot.transform.position, pacPrefab.transform.rotation)).GetComponent<PacManMovement>();

        //and we read the txt data from UsertInputs and put them on "sui"
        lines = fm.GetListOfLines("UserInputs.txt");
        aux = lines.ToArray();
        sui.LoadFromJson(aux[1]);

        if (sui.left==1)
        {
            objective.x = -1;
        }
        if (sui.right==1)
        {
            objective.x = 1;
        }
        if (sui.up==1)
        {
            objective.y = 1;
        }
        if (sui.down==1)
        {
            objective.y = -1;
        }

        //also the data from TraceData.txt is written on "std"
        lines = fm.GetListOfLines("TraceData.txt");
        aux = lines.ToArray();

        std.LoadFromJson(aux[1]);

        float[] stdArray = new float[11];
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

        //we use de constructor to send all the data to pacman
        pac.Init(net, objective, stdArray);
    }

    public void ChangeTurn(int turn)
    {
        //ChangeObjective
        sui.LoadFromJson(aux[turn]);
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
        std.LoadFromJson(aux[turn]);
        float[] stdArray = new float[11];
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
        pac.SetObjectiveAndInputs(objective, stdArray);
    }

}