using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using TMPro;
using System;
using Utilities;

public class Player : MonoBehaviour
{
    [SerializeField] private Rig aimRig;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera sniperVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private Image crosshairAim;
    [SerializeField] private Image crosshairWalk;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform pfBullet;
    [SerializeField] private Transform pfGrenade;
    [SerializeField] private Transform pfRocket;
    [SerializeField] private Transform pfShell;
    [SerializeField] private Transform pfFire;
    [SerializeField] private Transform pfGunFireSmoke;
    [SerializeField] private Transform pfRunSmoke;
    [SerializeField] private Transform pfRocketSmoke;
    [SerializeField] private GameObject pfStairs;
    [SerializeField] private GameObject pfWall;
    [SerializeField] private GameObject pfTempStairs;
    [SerializeField] private GameObject pfTempWall;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private GameObject scope;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform spawnGrenadePosition;
    [SerializeField] private Transform spawnRocketPosition;
    [SerializeField] private Transform spawnFirePosition;
    [SerializeField] private Transform spawnRunSmokePosition;
    [SerializeField] private Transform spawnBulletSmokePosition;
    [SerializeField] private AudioSource soundBuild;
    [SerializeField] private AudioSource soundSniper;
    [SerializeField] private AudioSource soundGunshot;
    [SerializeField] private AudioSource soundRocketLauncher;
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private Transform hitPosition;

    private Animator animator;
    private float timeLastRunSmoke;
    private float timeLastGrenadeThrow;
    private float danceTimeLeft;
    private bool throwingGrenade = false;
    private bool thrownGrenade = false;
    private int activeWeapon = 0;
    private TextMeshProUGUI textScore;
    Vector3 aimDirection;
    private int score = 0;
    private float aimRigWeight;
    private int placingBuild = -1;
    private GameObject newBuild;
    private float buildTileSize = 4.0f;
    private float previousVelocity;
    private float previousYposition;
    private bool playerDied = false;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        textScore = GameObject.Find("/Canvas/Score").GetComponent<TextMeshProUGUI>();
        activeWeapon = 0;
        previousYposition = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float yPosition = transform.position.y;
        float velocity = Math.Abs(previousYposition - yPosition) / Time.deltaTime;
        if (!playerDied && previousVelocity - velocity > 20f)
        {
            animator.Play("Death", 6, 0f);
            playerDied = true;
        }
        previousYposition = yPosition;
        previousVelocity = velocity;
        if  (playerDied)
        {
            animator.SetLayerWeight(6, Mathf.Lerp(animator.GetLayerWeight(6), 1f, Time.deltaTime * 4f));
        }

        Vector3 mouseWorldPosition;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Transform hitTransForm = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 9999f, aimColliderLayerMask))
        {
            hitTransForm = raycastHit.transform;
            mouseWorldPosition = raycastHit.point;
        }
        else
        {
            // we didn't hit anything, so take a point in the direction of the ray
            mouseWorldPosition = ray.GetPoint(300);
        }

        hitPosition.position = mouseWorldPosition;

        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        aimDirection = (worldAimTarget - transform.position).normalized;

        // Turn player towards aim point (only x and z axis)
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        
        // Set aimdirection (including y axis)
        aimDirection = (mouseWorldPosition - transform.position).normalized;

        if (starterAssetsInputs.aim)
        {
            // right mouse = cancel build
            if (placingBuild != -1)
            {
                starterAssetsInputs.aim = false;
                placingBuild = -1;
                Destroy(newBuild);
            }
            else
            {
                if (activeWeapon == 2)
                {
                    sniperVirtualCamera.gameObject.SetActive(true);
                    scope.SetActive(true);
                }
                else
                { 
                    aimVirtualCamera.gameObject.SetActive(true);
                }
                thirdPersonController.SetSensitivity(aimSensitivity);
                if (activeWeapon == 0)
                {
                    crosshairAim.gameObject.SetActive(true);
                }
                crosshairWalk.gameObject.SetActive(false);
                if (!throwingGrenade)
                {
                    aimRigWeight = 1f;
                    if (starterAssetsInputs.move.y > 0)
                    {
                        // aim-run
                        animator.SetLayerWeight(4, Mathf.Lerp(animator.GetLayerWeight(4), 1f, Time.deltaTime * 10f));
                    }
                    else
                    {
                        // aim standing
                        animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
                        animator.SetLayerWeight(4, 0f);
                    }
                }
            }
        }
        else
        {
            aimRigWeight = 0f;
            aimVirtualCamera.gameObject.SetActive(false);
            sniperVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            crosshairAim.gameObject.SetActive(false);
            crosshairWalk.gameObject.SetActive(true);
            scope.SetActive(false);

            if (!throwingGrenade)
            {
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
                animator.SetLayerWeight(4, Mathf.Lerp(animator.GetLayerWeight(4), 0f, Time.deltaTime * 10f));
            }
        }

        aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 10f);

        if (starterAssetsInputs.move.magnitude > 0)
        {
            if (Time.time >= timeLastRunSmoke + 0.1f)
            {
                timeLastRunSmoke = Time.time;
                Instantiate(pfRunSmoke, spawnRunSmokePosition.position, Quaternion.LookRotation(spawnRunSmokePosition.position, Vector3.up));
            }
        }

        if (danceTimeLeft>0)
        {
            animator.SetLayerWeight(5, Mathf.Lerp(animator.GetLayerWeight(5), 1f, Time.deltaTime * 5f));
            danceTimeLeft -= Time.deltaTime;
        }
        else
        {
            animator.SetLayerWeight(5, Mathf.Lerp(animator.GetLayerWeight(5), 0f, Time.deltaTime * 5f));
        }

        if (starterAssetsInputs.dance)
        {
            danceTimeLeft = 4f;
            starterAssetsInputs.dance = false;
        }

        if (starterAssetsInputs.grenade)
        {
            aimRig.weight = 0;
            timeLastGrenadeThrow = Time.time;
            throwingGrenade = true;
            weapons[activeWeapon].SetActive(false);
            starterAssetsInputs.grenade = false;
            animator.Play("Throw", 3, 0f);
            animator.SetLayerWeight(1, 0f);
            animator.SetLayerWeight(2, 0f);
            animator.SetLayerWeight(3, 1f);
            animator.SetLayerWeight(4, 0f);
        }

        if (placingBuild == 0)      // stairs
        {
            UpdateStairsPosition(newBuild);
        }

        if (placingBuild == 1)      // wall
        {
            UpdateWallPosition(newBuild);
        }

        if (placingBuild == 2)      // floor
        {
            UpdateFloorPosition(newBuild);
        }

        if (starterAssetsInputs.stairs)
        {
            starterAssetsInputs.stairs = false;
            placingBuild = 0;
            Destroy(newBuild);
            newBuild = Instantiate(pfTempStairs);
        }

        if (starterAssetsInputs.wall)
        {
            placingBuild = 1;
            starterAssetsInputs.wall = false;
            Destroy(newBuild);
            newBuild = Instantiate(pfTempWall);
        }

        if (starterAssetsInputs.floor)
        {
            placingBuild = 2;
            starterAssetsInputs.floor = false;
            Destroy(newBuild);
            newBuild = Instantiate(pfTempWall);
        }

        if (starterAssetsInputs.toggleWeapon)
        {
            starterAssetsInputs.toggleWeapon = false;
            weapons[activeWeapon].SetActive(false);
            activeWeapon++;
            if (activeWeapon > 2)
            {
                activeWeapon = 0;
            }
            weapons[activeWeapon].SetActive(true);
        }

        if (throwingGrenade)
        {
            if (thrownGrenade == false && Time.time >= timeLastGrenadeThrow + 0.5f)
            {
                thrownGrenade = true;
                Instantiate(pfGrenade, spawnGrenadePosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
            }
            if (Time.time >= timeLastGrenadeThrow + 0.9f)
            {
                weapons[activeWeapon].SetActive(true);
                throwingGrenade = false;
                thrownGrenade = false;
            }
        }
        else
        {
            // back to normal 
            animator.SetLayerWeight(3, Mathf.Lerp(animator.GetLayerWeight(3), 0f, Time.deltaTime * 10f));
        }

        if (starterAssetsInputs.shoot)
        {
            starterAssetsInputs.shoot = false;

            // left mouse = confirm build
            if (placingBuild != -1)
            {
                ConfirmBuild();
            }
            else
            {
                animator.SetLayerWeight(1, 0f);
                animator.SetLayerWeight(2, 1f);
                animator.SetLayerWeight(3, 0f);
                animator.SetLayerWeight(4, 0f);
                if (activeWeapon == 0)
                {
                    soundGunshot.Play();
                    Instantiate(pfShell, spawnBulletPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                    Instantiate(pfFire, spawnFirePosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                    Instantiate(pfGunFireSmoke, spawnBulletSmokePosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                    // add bullet trail
                    //                var scriptInstance = Instantiate(bulletTrail, spawnBulletPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up)).GetComponent<BulletProjectileRaycast>();
                    //                if (scriptInstance != null)
                    //                {
                    //                    scriptInstance.Setup(mouseWorldPosition);
                    //                }
                    if (TransformUtilities.CheckEnemyHit(hitTransForm))
                    {
                        IncreaseScore();
                    }
                }
                if (activeWeapon == 1)
                {
                    soundRocketLauncher.Play();
                    Instantiate(pfRocket, spawnRocketPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                    Instantiate(pfRocketSmoke, transform.position, Quaternion.identity);
                }
                if (activeWeapon == 2)
                {
                    Instantiate(pfBullet, spawnBulletPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                    soundSniper.Play();
                }
            }
        }
        else
        {
            // back to normal 
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0f, Time.deltaTime * 10f));
        }
    }

    public void IncreaseScore()
    {
        score++;
        textScore.text = "Kills: " + score.ToString();
    }

    private void ConfirmBuild()
    {
        soundBuild.Play();
        if (placingBuild == 0)      // stairs
        {
            GameObject newStairs = Instantiate(pfStairs);
            UpdateStairsPosition(newStairs);
        }

        if (placingBuild == 1)      // wall
        {
            GameObject newWall = Instantiate(pfWall);
            UpdateWallPosition(newWall);
        }

        if (placingBuild == 2)      // floor
        {
            GameObject newFloor = Instantiate(pfWall);
            UpdateFloorPosition(newFloor);
        }
    }

    private void UpdateStairsPosition(GameObject stairs)
    {
        double rotation = Math.Atan2(transform.forward.x, transform.forward.z) * 180 / Math.PI;
        Quaternion myQuaternion;

        float positionX = transform.position.x + transform.forward.x * 4f + 2f;
        float positionY = transform.position.y;
        float positionZ = transform.position.z + transform.forward.z * 4f + 2f;
        Vector3 currentSquarePosition = new Vector3((float)(positionX - positionX % buildTileSize),
            4f + (positionY - positionY % buildTileSize),
            (float)(positionZ - positionZ % buildTileSize));

        if (rotation > -45 && rotation <= 45)
        {
            myQuaternion = Quaternion.Euler(Vector3.up * 90);
            myQuaternion *= Quaternion.AngleAxis(45, -Vector3.forward);
        }
        else if (rotation > 45 && rotation <= 135)
        {
            myQuaternion = Quaternion.AngleAxis(45, Vector3.forward);
        }
        else if (rotation > -135 && rotation <= -45)
        {
            myQuaternion = Quaternion.AngleAxis(45, -Vector3.forward);
        }
        else
        {
            myQuaternion = Quaternion.Euler(Vector3.up * 90);
            myQuaternion *= Quaternion.AngleAxis(45, Vector3.forward);
        }

        stairs.transform.SetPositionAndRotation(currentSquarePosition, myQuaternion);        
    }

    private void UpdateWallPosition(GameObject wall)
    {
        double rotation = Math.Atan2(transform.forward.x, transform.forward.z) * 180 / Math.PI;

        Quaternion myQuaternion;

        float positionX = transform.position.x + transform.forward.x * 2f;
        float positionY = transform.position.y;
        float positionZ = transform.position.z + transform.forward.z * 2f;
        float adjustX = 0;
        float adjustZ = 0;

        if (rotation > -45 && rotation <= 45)
        {
            myQuaternion = Quaternion.Euler(Vector3.up * 90);
            myQuaternion *= Quaternion.AngleAxis(90, -Vector3.forward);
            adjustZ = 2f;
        }
        else if (rotation > 45 && rotation <= 135)
        {
            myQuaternion = Quaternion.AngleAxis(90, Vector3.forward);
            adjustX = 2f;
        }
        else if (rotation > -135 && rotation <= -45)
        {
            myQuaternion = Quaternion.AngleAxis(90, -Vector3.forward);
            adjustX = 2f;
        }
        else
        {
            myQuaternion = Quaternion.Euler(Vector3.up * 90);
            myQuaternion *= Quaternion.AngleAxis(90, Vector3.forward);
            adjustZ = 2f;
        }

        Vector3 currentSquarePosition = new Vector3(adjustX + positionX - positionX % buildTileSize,
            4f + positionY - positionY % buildTileSize,
            adjustZ + positionZ - positionZ % buildTileSize);

        wall.transform.SetPositionAndRotation(currentSquarePosition, myQuaternion);
    }

    private void UpdateFloorPosition(GameObject floor)
    {
        float positionX = transform.position.x + transform.forward.x * 4f + 2f;
        float positionY = transform.position.y;
        float positionZ = transform.position.z + transform.forward.z * 4f + 2f;
        Vector3 currentSquarePosition = new Vector3((float)(positionX - positionX % buildTileSize),
            2f + positionY - positionY % buildTileSize,
            (float)(positionZ - positionZ % buildTileSize));

        floor.transform.SetPositionAndRotation(currentSquarePosition, Quaternion.Euler(0, 0, 0));
    }
}
