using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDisable()
    {
        if (GameManager.Instance.gameState.Equals(GameState.Intro))
        {
            GameManager.Instance.UpdateGameState(GameState.Game);
        }
    }
}
