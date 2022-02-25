using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    private int score;
    private TextMeshProUGUI textScore;

    private void Awake()
    {
        textScore = GameObject.Find("/Canvas/Score").GetComponent<TextMeshProUGUI>();
    }

    public void IncreaseScore()
    {
        score++;
        textScore.text = "Kills: " + score;
    }
}
