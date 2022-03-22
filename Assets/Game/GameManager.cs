using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState gameState;
    public static event Action<GameState> OnGameStateChanged;

    private CinemachineVirtualCamera introCamera;
    private CinemachineVirtualCamera aimVirtualCamera;
    private CinemachineVirtualCamera sniperVirtualCamera;
    private TextMeshProUGUI textScore;
    private TextMeshProUGUI textKills;
    private TextMeshProUGUI textFrameRate;
    private TextMeshProUGUI textTimeLeft;
    private TextMeshProUGUI textWeapon;
    private TextMeshProUGUI textPlayers;
    private TextMeshProUGUI textEndScreen;
    private TextMeshProUGUI textEndScreenTime;
    private Image imageWeapon;
    private Image imageScore;
    private Image imageTimeleft;
    private Image imageEndScreen;
    private TimeLeft scriptTimeLeft;
    private EndScreen scriptEndScreen;
    private Animator introAnimator;
    private GameObject player;
    private Player playerScript;
    private Title scriptTitle;

    public GameObject Player { get => player; set => player = value; }
    public Player PlayerScript { get => playerScript; set => playerScript = value; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        textScore = GameObject.Find("/Canvas/Score").GetComponent<TextMeshProUGUI>();
        textKills = GameObject.Find("/Canvas/Kills").GetComponent<TextMeshProUGUI>();
        textFrameRate = GameObject.Find("/Canvas/Framerate").GetComponent<TextMeshProUGUI>();
        textTimeLeft = GameObject.Find("/Canvas/TimeLeft").GetComponent<TextMeshProUGUI>();
        textWeapon = GameObject.Find("/Canvas/Weapon/Description").GetComponent<TextMeshProUGUI>();
        textPlayers = GameObject.Find("/Canvas/Players").GetComponent<TextMeshProUGUI>();
        textEndScreen = GameObject.Find("/Canvas/EndScreen/Text").GetComponent<TextMeshProUGUI>();
        textEndScreenTime = GameObject.Find("/Canvas/EndScreen/TextTime").GetComponent<TextMeshProUGUI>();
        imageWeapon = GameObject.Find("/Canvas/Weapon/Image").GetComponent<Image>();
        imageScore = GameObject.Find("/Canvas/ScoreBackground").GetComponent<Image>();
        imageTimeleft = GameObject.Find("/Canvas/TimeLeftBackground").GetComponent<Image>();
        imageEndScreen = GameObject.Find("/Canvas/EndScreen/Background").GetComponent<Image>();
        scriptTimeLeft = GameObject.Find("/Canvas/TimeLeft").GetComponent<TimeLeft>();
        scriptEndScreen = GameObject.Find("/Canvas/EndScreen").GetComponent<EndScreen>();
        scriptTitle = GameObject.Find("/Canvas/Title").GetComponent<Title>();
        aimVirtualCamera = GameObject.Find("/Cameras/AimCamera").GetComponent<CinemachineVirtualCamera>();
        sniperVirtualCamera = GameObject.Find("/Cameras/SniperCamera").GetComponent<CinemachineVirtualCamera>();
        introCamera = GameObject.Find("/Cameras/IntroCamera").GetComponent<CinemachineVirtualCamera>();
        introAnimator = introCamera.GetComponent<Animator>();
        UpdateGameState(GameState.InitGame);
    }

    public void UpdateGameState(GameState newGameState)
    {
        gameState = newGameState;

        switch (newGameState)
        {
            case GameState.SetPlayerName:
                break;
            case GameState.InitGame:
                break;
            case GameState.Intro:
                imageEndScreen.enabled = false;
                textEndScreen.enabled = false;
                textEndScreenTime.enabled = false;
                textScore.enabled = false;
                textKills.enabled = false;
                textFrameRate.enabled = false;
                textTimeLeft.enabled = false;
                textWeapon.enabled = false;
                textPlayers.enabled = false;
                imageWeapon.enabled = false;
                imageScore.enabled = false;
                imageTimeleft.enabled = false;
                introAnimator.Rebind();
                introAnimator.Update(0f);
                aimVirtualCamera.gameObject.SetActive(true);
                sniperVirtualCamera.gameObject.SetActive(true);
                introCamera.gameObject.SetActive(true);
                scriptTitle.ResetTitle();
                break;
            case GameState.Game:
                playerScript.ResetPlayerStats();
                scriptTimeLeft.InitTimer();
                textScore.enabled = true;
                textKills.enabled = true;
                textFrameRate.enabled = true;
                textTimeLeft.enabled = true;
                textWeapon.enabled = true;
                textPlayers.enabled = true;
                imageWeapon.enabled = true;
                imageScore.enabled = true;
                imageTimeleft.enabled = true;

                // original position: Vector3(19.2399998,38.1899986,89.5899963)
                //Vector3 spawnLocation = new Vector3(50 * UnityEngine.Random.value - 25, 700, 50 * UnityEngine.Random.value - 25);
                playerScript.PlayerDied = false;
                playerScript.PlayerHasLanded = false;
                Vector3 spawnLocation = new Vector3(1000 * UnityEngine.Random.value - 500, 700, 1000 * UnityEngine.Random.value - 500);
                player.transform.position = spawnLocation;
                playerScript.Animator.SetLayerWeight(6, 0);

                scriptTimeLeft.enabled = true;
                break;
            case GameState.Ended:
                scriptTimeLeft.enabled = false;
                imageEndScreen.enabled = true;
                textEndScreen.enabled = true;
                textEndScreenTime.enabled = true;
                scriptEndScreen.ResetEndScreenTime();
                Transform transformEnemies = GameObject.Find("/Enemies").transform;
                foreach (Transform child in transformEnemies)
                {
                    Destroy(child.gameObject);
                }
                playerScript.PhotonView.RPC("SetFinalScore", RpcTarget.Others, playerScript.PhotonView.ViewID, playerScript.Kills, playerScript.Score, playerScript.ShotsFired, playerScript.ShotsHit, playerScript.DisplayName);
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
    InitGame,
    Intro,
    Game,
    Ended
}