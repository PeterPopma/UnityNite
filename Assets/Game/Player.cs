using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
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
    [SerializeField] private Transform pfRunSmoke;
    [SerializeField] private Transform pfRocketSmoke;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform spawnGrenadePosition;
    [SerializeField] private Transform spawnFirePosition;
    [SerializeField] private Transform spawnRunSmokePosition;
    [SerializeField] private AudioSource soundGunshot;
    [SerializeField] private AudioSource soundRocketLauncher;
    [SerializeField] private GameObject[] weapons;

    private Animator animator;
    private float timeLastRunSmoke;
    private float timeLastGrenadeThrow;
    private bool throwingGrenade = false;
    private bool thrownGrenade = false;
    private int activeWeapon = 0;
    private TextMeshProUGUI textScore;
    Vector3 aimDirection;
    private int score = 0;

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
            crosshairAim.gameObject.SetActive(true);
            crosshairWalk.gameObject.SetActive(false);
            if (!throwingGrenade)
            {
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            }
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            crosshairAim.gameObject.SetActive(false);
            crosshairWalk.gameObject.SetActive(true);
            if (!throwingGrenade)
            {
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            }
        }

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
            timeLastGrenadeThrow = Time.time;
            throwingGrenade = true;
            weapons[activeWeapon].SetActive(false);
            starterAssetsInputs.grenade = false;
            animator.SetLayerWeight(1, 0f);
            animator.SetLayerWeight(2, 0f);
            animator.Play("Throw", 3, 0f);
            animator.SetLayerWeight(3, 1f);
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
            if (activeWeapon == 0) 
            {
                soundGunshot.Play();
                Instantiate(pfShell, spawnBulletPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                Instantiate(pfFire, spawnFirePosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                // add bullet trail
                var scriptInstance = Instantiate(bulletTrail, spawnBulletPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up)).GetComponent<BulletProjectileRaycast>();
                if (scriptInstance != null)
                {
                    scriptInstance.Setup(mouseWorldPosition);
                }
                if (hitTransForm!=null)
                {
                    if (hitTransForm.GetComponent<BulletTarget>() != null)
                    {
                        Rigidbody rigidbody = hitTransForm.gameObject.GetComponent<Rigidbody>();
                        rigidbody.AddExplosionForce(500f, new Vector3(hitTransForm.transform.position.x, hitTransForm.transform.position.y, hitTransForm.transform.position.z), 3f);
                        Vector3 randomRotation = new Vector3(Random.Range(30f, 600f), Random.Range(30f, 600f), Random.Range(30f, 600f));
                        rigidbody.AddRelativeTorque(randomRotation, ForceMode.Impulse);
                        rigidbody.useGravity = true;
                        score++;
                        textScore.text = "Score: " + score.ToString("00000");


//                        BulletProjectileRaycast scriptInstance = Instantiate(pfBulletTrail).GetComponent<>("BulletProjectileRaycast");
  //                      scriptInstance.Setup(hitTransForm.position);
                    }
                }
            }
            if (activeWeapon == 1)
            {
                //                Instantiate(pfBullet, spawnBulletPosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
                soundRocketLauncher.Play();
                Instantiate(pfRocket, spawnGrenadePosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
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
