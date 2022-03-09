using UnityEngine;
using Cinemachine;
using Utilities;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;
    private CinemachineVirtualCamera bulletVirtualCamera;
    public float lifetime = 3.0f;
    private Vector3 moveDir;
    private Transform hitTransForm;
    private Vector3 hitPosition;
    private Score score;
    private float moveSpeed;
    private float previousDistance;
    private bool makingHit;
    private bool madeHit;

    public void Setup(Transform hitTransForm, Vector3 hitPosition)
    {
        this.hitPosition = hitPosition;
        this.hitTransForm = hitTransForm;
        if (hitTransForm != null && (hitTransForm.GetComponent<Target>() != null || hitTransForm.GetComponent<Enemy>() != null))
        {
            moveDir = (hitTransForm.position - transform.position).normalized;
            previousDistance = Vector3.Distance(transform.position, hitTransForm.position);
            makingHit = true;
            madeHit = false;
        }
        else
        {
            moveDir = transform.forward;
            makingHit = false;
        }
        moveSpeed = 60f;
//        renderer = GetComponent<Renderer>();
    }

    private void Awake()
    {
        score = GameObject.Find("/Canvas/Score").GetComponent<Score>();
        bulletVirtualCamera = GameObject.Find("/Cameras/BulletCamera").GetComponent<CinemachineVirtualCamera>();
        bulletVirtualCamera.Priority = 50;
        bulletVirtualCamera.Follow = transform;
    }

    private void Update()
    {
        if (makingHit)
        {
            if (!madeHit)
            {
                transform.position += moveSpeed * Time.deltaTime * moveDir;
            }

            float distance = Vector3.Distance(transform.position, hitTransForm.position);

            // if the bullet moved passed the object it hits, it should be removed
            if (distance > previousDistance)
            {
                transform.position += moveSpeed * Time.deltaTime * -moveDir;
                //            Instantiate(vfxHit, targetPosition, Quaternion.identity);
                if (TransformUtilities.CheckHit(hitTransForm, hitPosition))
                {
                    score.IncreaseScore();
                }
                madeHit = true;
                //                renderer.enabled = false;
                bulletVirtualCamera.m_Lens.FieldOfView = 100;
                CinemachineComponentBase componentBase = bulletVirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
                if (componentBase is CinemachineFramingTransposer)
                {
                    (componentBase as CinemachineFramingTransposer).m_CameraDistance = 30;
                }
                gameObject.SetActive(false);
                Invoke("ResetCamera", 0.5f);
            }
            previousDistance = distance;
        }
        else
        {
            transform.position += moveDir * Time.deltaTime * moveSpeed;
            lifetime -= Time.deltaTime;
            if (lifetime < 0f)
            {
                bulletVirtualCamera.Priority = 0;
                Destroy(gameObject);
            }
        }
    }

    private void ResetCamera()
    {
        bulletVirtualCamera.Priority = 0;
        Destroy(gameObject);
    }
}
