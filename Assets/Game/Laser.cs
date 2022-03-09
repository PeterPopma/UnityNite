using UnityEngine;
using Utilities;

public class Laser : MonoBehaviour
{
    [SerializeField] float lifetime = 3.0f;
    private Vector3 moveDir;
    private Transform hitTransForm;
    private Vector3 hitPosition;
    private Score score;
    private float previousDistance;
    private bool makingHit;
    private bool madeHit;

    public void Setup(Transform hitTransForm, Vector3 hitPosition)
    {
        this.hitTransForm = hitTransForm;
        this.hitPosition = hitPosition;

        if (hitTransForm != null && (hitTransForm.GetComponent<Target>() != null || hitTransForm.GetComponent<Enemy>() != null))
        {
            previousDistance = Vector3.Distance(transform.position, hitTransForm.position);
            makingHit = true;
        }
    }

    private void Awake()
    {
        score = GameObject.Find("/Canvas/Score").GetComponent<Score>();
    }

    private void Update()
    {
        /*
        if (makingHit)
        {

            float distance = Vector3.Distance(transform.position, hitTransForm.position);

            // if the laser moved passed the object it hits, it should be removed
            if (distance > previousDistance)
            {
                transform.position += moveSpeed * Time.deltaTime * -moveDir;
                if (TransformUtilities.CheckHit(hitTransForm, hitPosition))
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
 //           transform.position += moveDir * Time.deltaTime * moveSpeed;

        }
        */
        lifetime -= Time.deltaTime;
        if (lifetime < 0f)
        {
            if (TransformUtilities.CheckHit(hitTransForm, hitPosition))
            {
                score.IncreaseScore();
            }
            Destroy(gameObject);
        }
    }
}
