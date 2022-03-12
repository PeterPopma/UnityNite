using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameState gameState;

    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Intro);
    }

    public void UpdateGameState(GameState newGameState)
    {
        gameState = newGameState;

        switch (newGameState)
        {
            case GameState.SetPlayerName:
                break;
            case GameState.Intro:
                break;
            case GameState.Game:
                break;
            case GameState.Ended:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
        }

        OnGameStateChanged?.Invoke(newGameState);
    }
}

public enum GameState
{
    SetPlayerName,
    Intro,
    Game,
    Ended
}