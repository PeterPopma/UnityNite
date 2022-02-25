using UnityEngine;
using Utilities;

public class Laser : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;
    public float lifetime = 3.0f;
    private Vector3 moveDir;
    private Transform hitTransForm;
    private Score score;
    private float moveSpeed;
    private float previousDistance;
    private bool makingHit;
    private bool madeHit;

    public void Setup(Transform hitTransForm)
    {
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

            // if the laser moved passed the object it hits, it should be removed
            if (distance > previousDistance)
            {
                transform.position += moveSpeed * Time.deltaTime * -moveDir;
                //            Instantiate(vfxHit, targetPosition, Quaternion.identity);
                if (TransformUtilities.CheckHit(hitTransForm))
                {
                    score.IncreaseScore();
                }
                madeHit = true;
                gameObject.SetActive(false);
            }
            previousDistance = distance;
        }
        else
        {
            transform.position += moveDir * Time.deltaTime * moveSpeed;
            lifetime -= Time.deltaTime;
            if (lifetime < 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
