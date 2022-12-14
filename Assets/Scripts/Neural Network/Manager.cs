using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject boomerPrefab;
    public GameObject hex;

    private bool isTraning = false;
    [SerializeField][Tooltip("population size MUST be even, else it will be set as 20 (og value:50)")]
    private int populationSize = 50;
    [SerializeField]
    private int generationNumber = 0;
    private int[] layers = new int[] { 1, 10, 10, 1 }; //originally 1 input and 1 output, now is 1 input 2 outputs
    private List<NeuralNetwork> nets;
    private bool leftMouseDown = false;
    private List<Boomerang> boomerangList = null;
    //pac stuff
    public GameObject pacPrefab;
    [SerializeField]
    private Transform spawnSpot;
    private List<PacManMovement> pacList = null;
    [SerializeField]
    private bool pacTest;

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
                nets.Sort();
                for (int i = 0; i < populationSize / 2; i++)
                {
                    nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]);
                    nets[i].Mutate();

                    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                }

                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].SetFitness(0f);
                }
            }


            generationNumber++;

            isTraning = true;
            Invoke("Timer", 15f);
            if (pacTest==true)
                CreatePacBodies();
            else
                CreateBoomerangBodies();
        }


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
            PacManMovement pac = ((GameObject)Instantiate(pacPrefab, new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), 0), pacPrefab.transform.rotation)).GetComponent<PacManMovement>();
            pac.Init(nets[i], hex.transform);
            pacList.Add(pac);
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
}
