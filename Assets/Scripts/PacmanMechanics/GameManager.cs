using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //public GameState State;

    private void Awake()
    {
        Instance = this;
    }
    //so if all rewards has been grabbed open door



    //public void UpdateGameState(GameState newState)
    //{
    //    State = newState;

    //    switch (newState)
    //    {
    //        case GameState.Menu:
    //            break;
    //        case GameState.GamePlayer:
    //            break;
    //        case GameState.GameNet:
    //            break;
    //        case GameState.ResultScreen:
    //            break;
    //    }
    //}

    //public enum GameState
    //{
    //    Menu,
    //    GamePlayer,
    //    GameNet,
    //    ResultScreen
    //}
}
