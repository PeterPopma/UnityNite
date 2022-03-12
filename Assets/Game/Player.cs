using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using System;
using Utilities;
using System.Collections;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] private Rig aimRig;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private float sniperSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private GameObject pfBullet;
    [SerializeField] private GameObject pfLaser;
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
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform spawnGrenadePosition;
    [SerializeField] private Transform spawnRocketPosition;
    [SerializeField] private Transform spawnFirePosition;
    [SerializeField] private Transform spawnRunSmokePosition;
    [SerializeField] private Transform spawnBulletSmokePosition;
    [SerializeField] private GameObject[] weapons;

    private Image scope;
    private EnemySpawner enemySpawnerScript;
    private AudioSource soundBuild;
    private AudioSource soundDance;
    private AudioSource soundDied;
    private AudioSource soundSniper;
    private AudioSource soundGunshot;
    private AudioSource soundPickaxe;
    private AudioSource soundFootstepLeft;
    private AudioSource soundFootstepRight;
    private AudioSource soundLaser;
    private AudioSource soundRocketLauncher;
    private Image crosshairAim;
    private Image crosshairWalk;
    private CinemachineVirtualCamera aimVirtualCamera;
    private CinemachineVirtualCamera sniperVirtualCamera;
    private CinemachineVirtualCamera playerFollowVirtualCamera;
    private Vector3 hitPosition;
    private Score score;
    private Animator animator;
    private float timeLastRunSmoke;
    private float timeLastGrenadeThrow;
    private float danceTimeLeft;
    private bool throwingGrenade = false;
    private bool thrownGrenade = false;
    private int activeWeapon = 0;
    Vector3 aimDirection;
    private float aimRigWeight;
    private int placingBuild = -1;
    private GameObject newBuild;
    private float buildTileSize = 4.0f;
    private float previousVelocity;
    private float previousYposition;
    private bool playerDied = false;
    bool playingFootstep = false;
    bool makingLeftFootstep = false;
    bool playerHasLanded = false;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    private TextMeshProUGUI textWeapon;
    private Image imageWeapon;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        score = GameObject.Find("/Canvas/Score").GetComponent<Score>();
        textWeapon = GameObject.Find("/Canvas/Weapon/Description").GetComponent<TextMeshProUGUI>();
        imageWeapon = GameObject.Find("/Canvas/Weapon/Image").GetComponent<Image>();
        aimVirtualCamera = GameObject.Find("/Cameras/AimCamera").GetComponent<CinemachineVirtualCamera>();
        sniperVirtualCamera = GameObject.Find("/Cameras/SniperCamera").GetComponent<CinemachineVirtualCamera>();
        playerFollowVirtualCamera = GameObject.Find("/Cameras/PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        crosshairAim = GameObject.Find("/Canvas/CrosshairAim").GetComponent<Image>();
        crosshairWalk = GameObject.Find("/Canvas/CrosshairWalk").GetComponent<Image>();
        scope = GameObject.Find("/Canvas/Scope").GetComponent<Image>();
        soundBuild = GameObject.Find("/Sound/Build").GetComponent<AudioSource>();
        soundDance = GameObject.Find("/Sound/Dance").GetComponent<AudioSource>();
        soundDied = GameObject.Find("/Sound/Died").GetComponent<AudioSource>();
        soundSniper = GameObject.Find("/Sound/Sniper").GetComponent<AudioSource>();
        soundGunshot = GameObject.Find("/Sound/Gunshot").GetComponent<AudioSource>();
        soundPickaxe = GameObject.Find("/Sound/Pickaxe").GetComponent<AudioSource>();
        soundFootstepLeft = GameObject.Find("/Sound/FootstepLeft").GetComponent<AudioSource>();
        soundFootstepRight = GameObject.Find("/Sound/FootstepRight").GetComponent<AudioSource>();
        soundLaser = GameObject.Find("/Sound/Laser").GetComponent<AudioSource>();
        soundRocketLauncher = GameObject.Find("/Sound/RocketLauncher").GetComponent<AudioSource>();
        enemySpawnerScript = GameObject.Find("/Scripts/EnemySpawner").GetComponent<EnemySpawner>();
        enemySpawnerScript.Player = gameObject;
        activeWeapon = 0;
        previousYposition = transform.position.y;

        // set cameras to follow this player
        aimVirtualCamera.Follow = gameObject.transform.Find("PlayerCameraRoot").transform;
        sniperVirtualCamera.Follow = gameObject.transform.Find("PlayerCameraRoot").transform;
        playerFollowVirtualCamera.Follow = gameObject.transform.Find("PlayerCameraRoot").transform;
    }
        

    // Update is called once per frame
    void Update()
    {
        float yPosition = transform.position.y;
        float velocity = Math.Abs(previousYposition - yPosition) / Time.deltaTime;
        if (!playerHasLanded)
        {
            animator.SetLayerWeight(8, 1f);

            if (gameObject.transform.position.y < Terrain.activeTerrain.SampleHeight(gameObject.transform.position) + 5)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, Terrain.activeTerrain.SampleHeight(gameObject.transform.position), gameObject.transform.position.z);
                playerHasLanded = true;
                animator.SetLayerWeight(8, 0f);
                previousVelocity = velocity = 0;
                yPosition = transform.position.y;
            }
        }
        else if (!playerDied && previousVelocity - velocity > 20f)
        {
            animator.Play("RifleDeathA", 6, 0f);
            playerDied = true;
            soundDied.Play();
        }
        previousYposition = yPosition;
        previousVelocity = velocity;
        if (playerDied)
        {
            animator.SetLayerWeight(6, Mathf.Lerp(animator.GetLayerWeight(6), 1f, Time.deltaTime * 4f));
        }

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Transform hitTransForm = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 9999f, aimColliderLayerMask))
        {
            hitTransForm = raycastHit.transform;
            hitPosition = raycastHit.point;
        }
        else
        {
            // we didn't hit anything, so take a point in the direction of the ray
            hitPosition = ray.GetPoint(300);
        }

        Vector3 aimLocationXZ = new Vector3(hitPosition.x, hitPosition.y, hitPosition.z);
        aimLocationXZ.y = transform.position.y;
        aimDirection = (hitPosition - spawnBulletPosition.position).normalized;

        // Turn player towards aim point (only x and z axis)
        transform.forward = Vector3.Lerp(transform.forward, (aimLocationXZ - transform.position).normalized, Time.deltaTime * 20f);

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
                    thirdPersonController.SetSensitivity(sniperSensitivity);
                    sniperVirtualCamera.gameObject.SetActive(true);
                    scope.enabled = true;
                }
                else
                {
                    thirdPersonController.SetSensitivity(aimSensitivity);
                    aimVirtualCamera.gameObject.SetActive(true);
                }
                if (activeWeapon == 0 || activeWeapon == 1 || activeWeapon == 3)
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
            scope.enabled = false;

            if (!throwingGrenade)
            {
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
                animator.SetLayerWeight(4, Mathf.Lerp(animator.GetLayerWeight(4), 0f, Time.deltaTime * 10f));
            }
        }

        aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 10f);

        if (starterAssetsInputs.move.magnitude > 0)
        {
            if (thirdPersonController.Grounded && !playingFootstep)
            {
                StartCoroutine("playFootStep");
            }
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
            soundDance.Play();
            danceTimeLeft = 5.5f;
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
            animator.SetLayerWeight(5, 0f);
            animator.SetLayerWeight(6, 0f);
            animator.SetLayerWeight(7, 0f);
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
            ToggleWeapon();
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
                    if (TransformUtilities.CheckHit(hitTransForm, hitPosition))
                    {
                        score.IncreaseScore(100);
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
                    starterAssetsInputs.aim = false;
                    GameObject bullet = Instantiate(pfBullet, spawnBulletPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                    Bullet bulletScript = bullet.GetComponent<Bullet>();
                    bulletScript.Setup(hitTransForm, hitPosition);
                    soundSniper.Play();
                }
                if (activeWeapon == 3)
                {
                    GameObject laser = Instantiate(pfLaser, spawnBulletPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                    Laser laserScript = laser.GetComponent<Laser>();
                    laserScript.Setup(hitTransForm, hitPosition);
                    soundLaser.Play();
                }
                if (activeWeapon == 4)
                {
                    animator.Play("SSAttack", 7, 0f);
                    // Pickaxe uses different animation
                    animator.SetLayerWeight(2, 0f);
                    animator.SetLayerWeight(7, 1f);
                    soundPickaxe.Play(); 
                    if (animator.GetLayerWeight(7) > 0 && TransformUtilities.CheckHit(hitTransForm, hitPosition))
                    {
                        score.IncreaseScore(100);
                    }
                }
            }
        }
        else
        {
            // back to normal 
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0f, Time.deltaTime * 10f));
            animator.SetLayerWeight(7, Mathf.Lerp(animator.GetLayerWeight(7), 0f, Time.deltaTime * 1f));
        }
    }

    private void ToggleWeapon()
    {
        starterAssetsInputs.toggleWeapon = false;
        weapons[activeWeapon].SetActive(false);
        activeWeapon++;
        if (activeWeapon >= weapons.Length)
        {
            activeWeapon = 0;
        }
        weapons[activeWeapon].SetActive(true);
        switch(activeWeapon)
        {
            case 0:
                imageWeapon.sprite = Resources.Load<Sprite>("akm_complete");
                textWeapon.text = "AKM";
                break;
            case 1:
                imageWeapon.sprite = Resources.Load<Sprite>("rocket_complete");
                textWeapon.text = "Rocketlauncher";
                break;
            case 2:
                imageWeapon.sprite = Resources.Load<Sprite>("sniper_complete");
                textWeapon.text = "Sniper";
                break;
            case 3:
                imageWeapon.sprite = Resources.Load<Sprite>("laser_complete");
                textWeapon.text = "Laser";
                break;
            case 4:
                imageWeapon.sprite = Resources.Load<Sprite>("axe_complete");
                textWeapon.text = "Pickaxe";
                break;
        }
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

    IEnumerator playFootStep()
    {
        playingFootstep = true;
        
        AudioSource footstep = makingLeftFootstep ? soundFootstepLeft : soundFootstepRight;
        makingLeftFootstep = !makingLeftFootstep;


        // Pick a random pitch to play it at
        float randomPitch = UnityEngine.Random.Range(1, 3);
        footstep.pitch = (int)randomPitch;

        // Play the sound
        footstep.Play();
        yield return new WaitForSeconds(footstep.clip.length);
        playingFootstep = false;
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
