using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using TMPro;
using System;

public class Player : MonoBehaviour
{
    [SerializeField] private Rig aimRig;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private Image crosshairAim;
    [SerializeField] private Image crosshairWalk;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform pfGrenade;
    [SerializeField] private Transform pfRocket;
    [SerializeField] private Transform pfShell;
    [SerializeField] private Transform pfFire;
    [SerializeField] private Transform pfGunFireSmoke;
    [SerializeField] private Transform pfRunSmoke;
    [SerializeField] private Transform pfRocketSmoke;
    [SerializeField] private Transform pfStairs;
    [SerializeField] private Transform pfWall;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform spawnGrenadePosition;
    [SerializeField] private Transform spawnRocketPosition;
    [SerializeField] private Transform spawnFirePosition;
    [SerializeField] private Transform spawnRunSmokePosition;
    [SerializeField] private Transform spawnBulletSmokePosition;
    [SerializeField] private Transform spawnBuildPosition;
    [SerializeField] private AudioSource soundGunshot;
    [SerializeField] private AudioSource soundRocketLauncher;
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private Transform hitPosition;

    private Animator animator;
    private float timeLastRunSmoke;
    private float timeLastGrenadeThrow;
    private bool throwingGrenade = false;
    private bool thrownGrenade = false;
    private int activeWeapon = 0;
    private TextMeshProUGUI textScore;
    Vector3 aimDirection;
    private int score = 0;
    private float aimRigWeight;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        textScore = GameObject.Find("/Canvas/Score").GetComponent<TextMeshProUGUI>();
        activeWeapon = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;

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
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            if (activeWeapon == 0)
            { 
                crosshairAim.gameObject.SetActive(true);
            }
            crosshairWalk.gameObject.SetActive(false);
            if (!throwingGrenade)
            {
                aimRigWeight = 1f;
                if (starterAssetsInputs.move.y>0)
                {
                    // aim-run
                    animator.SetLayerWeight(4, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
                }
                else
                {
                    // aim standing
                    animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
                    animator.SetLayerWeight(4, 0f);
                }
            }
        }
        else
        {
            aimRigWeight = 0f;
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            crosshairAim.gameObject.SetActive(false);
            crosshairWalk.gameObject.SetActive(true);
            if (!throwingGrenade)
            {
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
                animator.SetLayerWeight(4, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
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

        if (starterAssetsInputs.stairs)
        {
            starterAssetsInputs.stairs = false;
            double rotation = Math.Atan2(aimDirection.x, aimDirection.z) * 180 / Math.PI;
            
            Vector3 currentSquarePosition = new Vector3((float)(spawnBuildPosition.position.x - spawnBuildPosition.position.x % 4.0),
                (float)(spawnBuildPosition.position.y - spawnBuildPosition.position.y % 4.0),
                (float)(spawnBuildPosition.position.z - spawnBuildPosition.position.z % 4.0));

            if (rotation > -45 && rotation <= 45)
            {
                Quaternion myQuaternion = Quaternion.Euler(Vector3.up * 90);
                myQuaternion *= Quaternion.AngleAxis(45, -Vector3.forward);
                Instantiate(pfStairs, currentSquarePosition, myQuaternion);
            }
            else if (rotation > 45 && rotation <= 135)
            {
                Instantiate(pfStairs, currentSquarePosition, Quaternion.AngleAxis(45, Vector3.forward));
            }
            else if (rotation > -135 && rotation <= -45)
            {
                Instantiate(pfStairs, currentSquarePosition, Quaternion.AngleAxis(45, -Vector3.forward));
            }
            else
            {
                Quaternion myQuaternion = Quaternion.Euler(Vector3.up * 90);
                myQuaternion *= Quaternion.AngleAxis(45, Vector3.forward);
                Instantiate(pfStairs, currentSquarePosition, myQuaternion);
            }
        }

        if (starterAssetsInputs.wall)
        {
            starterAssetsInputs.wall = false;
            double rotation = Math.Atan2(aimDirection.x, aimDirection.z) * 180 / Math.PI;
            Vector3 buildPosition = spawnBuildPosition.position;// gameObject.transform.position;
            Vector3 currentSquarePosition = new Vector3((float)(buildPosition.x - buildPosition.x % 4.0),
                (float)(buildPosition.y - buildPosition.y % 4.0),
                (float)(buildPosition.z - buildPosition.z % 4.0));

            Instantiate(pfWall, currentSquarePosition, Quaternion.AngleAxis(90, Vector3.forward));
        }

        if (starterAssetsInputs.floor)
        {
            starterAssetsInputs.floor = false;
            double rotation = Math.Atan2(aimDirection.x, aimDirection.z) * 180 / Math.PI;
            Vector3 buildPosition = spawnBuildPosition.position;// gameObject.transform.position;
            Vector3 currentSquarePosition = new Vector3((float)(buildPosition.x - buildPosition.x % 4.0),
                (float)(buildPosition.y - buildPosition.y % 4.0),
                (float)(buildPosition.z - buildPosition.z % 4.0));

            Instantiate(pfWall, currentSquarePosition, Quaternion.Euler(0, 0, 0));
        }

        if (starterAssetsInputs.toggleWeapon)
        {
            starterAssetsInputs.toggleWeapon = false;
            if (activeWeapon == 0)
            {
                activeWeapon = 1;
                weapons[0].SetActive(false);
                weapons[1].SetActive(true);
            }
            else
            {
                activeWeapon = 0;
                weapons[0].SetActive(true);
                weapons[1].SetActive(false);
            }
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
                if (hitTransForm!=null)
                {
                    if (hitTransForm.GetComponent<Target>() != null)
                    {
                        Rigidbody rigidbody = hitTransForm.gameObject.GetComponent<Rigidbody>();
                        rigidbody.constraints = RigidbodyConstraints.None;
                        rigidbody.AddExplosionForce(700f, new Vector3(hitTransForm.transform.position.x, hitTransForm.transform.position.y, hitTransForm.transform.position.z), 3f);
                        Vector3 randomRotation = new Vector3(UnityEngine.Random.Range(30f, 600f), UnityEngine.Random.Range(30f, 600f), UnityEngine.Random.Range(30f, 600f));
                        rigidbody.AddRelativeTorque(randomRotation, ForceMode.Impulse);
                        rigidbody.useGravity = true;
                    }
                    if (hitTransForm.GetComponent<Enemy>() != null && hitTransForm.gameObject.GetComponent<Rigidbody>()==null)
                    {
                        Enemy enemy = hitTransForm.gameObject.GetComponent<Enemy>();
                        enemy.IsHit = true;
                        Rigidbody rigidbody = hitTransForm.gameObject.AddComponent<Rigidbody>();
                        rigidbody.mass = 0.1f;
                        rigidbody.AddExplosionForce(120f, new Vector3(hitTransForm.transform.position.x, hitTransForm.transform.position.y - 1, hitTransForm.transform.position.z), 4f);
                        Vector3 randomRotation = new Vector3(UnityEngine.Random.Range(1300f, 3000f), UnityEngine.Random.Range(0f, 0f), UnityEngine.Random.Range(1300f, 3000f));
                        rigidbody.AddTorque(randomRotation, ForceMode.Force);
                        rigidbody.useGravity = true;
                        score++;
                        textScore.text = "Kills: " + score.ToString();
                    }
                }
            }
            if (activeWeapon == 1)
            {
                //                Instantiate(pfBullet, spawnBulletPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                soundRocketLauncher.Play();
                Instantiate(pfRocket, spawnRocketPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                Instantiate(pfRocketSmoke, transform.position, Quaternion.identity);
            }
        }
        else
        {
            // back to normal 
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0f, Time.deltaTime * 10f));
        }
    }
}
