using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CheckForTraining : MonoBehaviour
{
    public Button buttonTrain;
    public Button buttonPlay;
    void Update()
    {
        if(File.Exists(Application.dataPath + "/TrainedNNData.txt"))
        {
            buttonPlay.interactable = true;
            buttonTrain.interactable = false;
        }
        else
        {
            buttonPlay.interactable = false;
            buttonTrain.interactable = true;
        }
    }
}
