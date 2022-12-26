using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using System.IO;

public class Manager : MonoBehaviour
{
    public GameObject boomerPrefab;
    public GameObject hex;

    private bool isTraning = false;
    [SerializeField][Tooltip("population size MUST be even, else it will be set as 20 (og value:50)")]
    private int populationSize = 50;
    [SerializeField]
    [Tooltip("how much time for individuals life, default is 15f")]
    private float genLifespan;
    private int generationNumber = 0;
    private int[] layers = new int[] { 1, 10, 10, 1 }; //1 input and 1 output for boomerangs/ 2 input 2 outputs for Pac
    private List<NeuralNetwork> nets;
    private bool leftMouseDown = false;
    private List<Boomerang> boomerangList = null;
    [SerializeField]
    private float maxFit = 0; //Almacena el mayor fitness que se ha tenido de todas las redes neuronales
    [SerializeField]
    private float avgFit = 0;
    private SaveData save;
    private NeuralNetwork savedNN;
    private FileManager fm;

    //pac stuff
    public GameObject pacPrefab;
    [SerializeField]
    private Transform spawnSpot;
    [SerializeField]
    private bool pacTest;
    private List<PacManMovement> pacList = null;
    Vector3 spawnSpotVector;

    //ghost
    public GameObject ghostPrefab;
    [SerializeField]
    private Transform ghostSpawn;
    private List<Unit> ghostList = null;

    //text stuff
    public GameObject genTextObject;
    private TextMeshProUGUI genText;

    public GameObject timeTextObject;
    private TextMeshProUGUI timeText;

    public GameObject avgFitnessObject;
    private TextMeshProUGUI avgFitness;

    public GameObject bestFitnessObject;
    private TextMeshProUGUI bestFitness;
    
    public GameObject timerTextObject;
    private TextMeshProUGUI timerText;

    private void Start()
    {
        genText = genTextObject.GetComponent<TextMeshProUGUI>();
        timeText = timeTextObject.GetComponent<TextMeshProUGUI>();
        avgFitness = avgFitnessObject.GetComponent<TextMeshProUGUI>();
        bestFitness = bestFitnessObject.GetComponent<TextMeshProUGUI>();
        timerText = timerTextObject.GetComponent<TextMeshProUGUI>();
        fm = new FileManager();
        

        if (pacTest)
        {                      //2, 10, 10, 2
            layers = new int[] { 13, 10, 10, 2 }; //Capas de neuronas //2 is x&y values of closest reward, then two are for enemy x&y + 9 of wall view
        }
    }

    void Timer()
    {
        isTraning = false;
    }

    void Update()
    {
        if (isTraning == false)
        {
            
            if (generationNumber == 0)
            {
                InitBoomerangNeuralNetworks();
                
            }
            else
            {


                avgFit = 0;
                nets.Sort(); //Se ordena según parámetros definidos en NeuralNetwork CompareTo
                maxFit = nets[populationSize - 1].GetFitness(); //Obtiene el fitness de la red neuronal con mejor desempeño
                bestFitness.text = "Best Fit.: "+ (int)maxFit;

                for (int i = 0; i < populationSize; i++)
                {
                    avgFit += nets[i].GetFitness();
                }
                avgFit = avgFit / populationSize;
                avgFitness.text = "Avg.Fit.: " + (int)avgFit;

                for (int i = 0; i < populationSize / 2; i++)
                {
                    //nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]); //"Respalda" las redes con mejor desempeño en la mitad de peor desempeño
                    nets[i] = nets[i + (populationSize / 2)];
                    nets[i].Mutate();
                    //Reinicia la mitad ya respaldada de redes neuronales
                    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                }

                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].SetFitness(0f); //Reinicia el fitness de todas las redes neuronales
                }
            }

            
            generationNumber++;
            genText.text = "Gen: " + generationNumber;

            isTraning = true;
            Invoke("Timer", genLifespan);

            //float timer = genLifespan;
            //timer -= Time.deltaTime;
            //timerText.text = "Timer: "+ timer;

            if (pacTest == true) { 
                CreatePacBodies();
                CreateGhostBodies();
            }
            else
                CreateBoomerangBodies();
        }
        
        timeText.text = "Time: "+(int)(Time.fixedUnscaledTime / 3600) % 60 + ":"+ (int)(Time.fixedUnscaledTime/60)%60 + ":"+ (int)Time.fixedUnscaledTime%60;

        if (Input.GetMouseButtonDown(0))
        {
            leftMouseDown = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            leftMouseDown = false;
        }

        if (leftMouseDown == true)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hex.transform.position = mousePosition;
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            nets.Sort();
            DataSaver ds = new DataSaver(nets[populationSize-1].GetLayers(), nets[populationSize-1].GetNeurons(), nets[populationSize-1].GetWeights());
            save = ds.SaveNN();
            fm.WriteToFile("NNTrainingData.txt", save.ToJson());

            //SaveByJson(nets[populationSize-1]);
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            //LoadByJson();
        }

    }


    private void CreateBoomerangBodies()
    {
        if (boomerangList != null)
        {
            for (int i = 0; i < boomerangList.Count; i++)
            {
                GameObject.Destroy(boomerangList[i].gameObject);
            }

        }

        boomerangList = new List<Boomerang>();

        for (int i = 0; i < populationSize; i++)
        {
            Boomerang boomer = ((GameObject)Instantiate(boomerPrefab, new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), 0), boomerPrefab.transform.rotation)).GetComponent<Boomerang>();
            boomer.Init(nets[i], hex.transform);
            boomerangList.Add(boomer);
        }

    }
    //pac stuff
    private void CreatePacBodies()
    {
        if (pacList != null)
        {
            for (int i = 0; i < pacList.Count; i++)
            {
                GameObject.Destroy(pacList[i].gameObject);
            }

        }

        pacList = new List<PacManMovement>();

        for (int i = 0; i < populationSize; i++)
        {
            
            PacManMovement pac = ((GameObject)Instantiate(pacPrefab, spawnSpot.transform.position, pacPrefab.transform.rotation)).GetComponent<PacManMovement>();
            
            pac.Init(nets[i], hex.transform);//wich number should be here?
            pacList.Add(pac);
        }

    }

    private void CreateGhostBodies()
    {
        if (ghostList != null)
        {
            for (int i = 0; i < ghostList.Count; i++)
            {
                GameObject.Destroy(ghostList[i].gameObject);
            }

        }

        ghostList = new List<Unit>();

        for (int i = 0; i < populationSize; i++) //targets.Length was populationSize
        {
            Unit ghost = ((GameObject)Instantiate(ghostPrefab, ghostSpawn.transform.position, ghostPrefab.transform.rotation)).GetComponent<Unit>();

            ghost.Chase(pacList[i].gameObject.transform);//we should give the corresponding pac transform
            ghostList.Add(ghost);
        }

    }


    void InitBoomerangNeuralNetworks()
    {
        //population must be even, just setting it to 20 incase it's not
        if (populationSize % 2 != 0)
        {
            populationSize = 20;
        }

        nets = new List<NeuralNetwork>();


        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate();
            nets.Add(net);
        }
    }

    //we created the rewards on intitialization of pacman bodies

    //needs a fix, instead of rewards this should be a list of positions, once a position is reached is also cleaned off the list

    //private void CreateRewards()
    //{
    //    if (rewardList != null)
    //    {
    //        for (int i = 0; i < rewardList.Count; i++)
    //        {
    //            GameObject.Destroy(rewardList[i].gameObject);
    //        }
    //    }

    //    rewardList = new List<GameObject>();

    //    for (int i = 0; i < rewardNumber; i++)
    //    {
    //        GameObject reward = ((GameObject)Instantiate(rewardPrefab, rewardSpawns[i].position, rewardPrefab.transform.rotation));
    //        rewardList.Add(reward);
    //    }
    //}
}
