using UnityEngine;
using System.Collections;
using System;

public class Enemy : MonoBehaviour {
	[SerializeField] float maxDistance = 100f;
	private AudioSource[] soundOuch = new AudioSource[4];
	private GameObject player;
	private float speedX;
	private float speedZ;
	private bool isHit;
	private Animator animator;
	private float timeDied;
	private bool onGround = false;

	public bool IsHit { get => isHit; set => isHit = value; }
    public float SpeedX { get => speedX; set => speedX = value; }
    public float SpeedZ { get => speedZ; set => speedZ = value; }

    // Use this for initialization
    void Start () {
		float rotation = (float)(Math.Atan2(SpeedX, SpeedZ) * 180 / Math.PI);
		transform.Rotate(transform.up, rotation);
		animator = GetComponent<Animator>();
		player = GameObject.Find("/Player");
		soundOuch[0] = GameObject.Find("/Sound/Ouch").GetComponent<AudioSource>();
		soundOuch[1] = GameObject.Find("/Sound/Ouch2").GetComponent<AudioSource>();
		soundOuch[2] = GameObject.Find("/Sound/Ouch3").GetComponent<AudioSource>();
		soundOuch[3] = GameObject.Find("/Sound/Ouch4").GetComponent<AudioSource>();
	}

	public void Hit()
    {
        isHit = true;
		timeDied = Time.time;
		soundOuch[UnityEngine.Random.Range(0, 4)].Play();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		/*
		if (local)
		{
			// transform.RotateAroundLocal(transform.up, Time.fixedDeltaTime*rot_speed_x);
			transform.Rotate(transform.up, Time.fixedDeltaTime * rot_speed_x, Space.World);
		}
		else
		{
			transform.Rotate(Time.fixedDeltaTime * new Vector3(rot_speed_x, rot_speed_y, rot_speed_z), Space.World);
		}
		*/
	}

    private void Update()
    {
		if (!isHit)
		{
			Vector3 pos = transform.position;
			pos.y = Terrain.activeTerrain.SampleHeight(transform.position) + 0f;
			transform.position = pos;
			transform.Translate(new Vector3(SpeedX, 0f, SpeedZ) * Time.deltaTime, Space.World);
			if (Math.Abs(transform.position.x - player.transform.position.x) > maxDistance || Math.Abs(transform.position.z - player.transform.position.z) > maxDistance)
			{
				Destroy(gameObject);
			}
        }

		if (isHit)
        {
			if (!onGround && Time.time - timeDied > 2f)
			{
				onGround = true;
			}
			if (onGround)
            {
				animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 1f, Time.deltaTime * 10f));
			}
			else
            {
				animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
			}
		}

		if (isHit && Time.time - timeDied > 3f)
        {
			Destroy(gameObject);
		}
	}

	public static Transform RecursiveFindChild(Transform parent, string childName)
	{
		Transform result = null;

		foreach (Transform child in parent)
		{
			if (child.name == childName)
				result = child.transform;
			else
				result = RecursiveFindChild(child, childName);

			if (result != null) break;
		}

		return result;
	}
}
