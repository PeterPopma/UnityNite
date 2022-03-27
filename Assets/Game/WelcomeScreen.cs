using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WelcomeScreen : MonoBehaviour
{
    [SerializeField] GameObject inputField;

    public void SetName()
    {
        string name = inputField.GetComponent<TextMeshProUGUI>().text;
        if (name != null && name.Length>0)
        {
            GameManager.Instance.PreferredName = name;
            GameManager.Instance.UpdateGameState(GameState.InitGame);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
