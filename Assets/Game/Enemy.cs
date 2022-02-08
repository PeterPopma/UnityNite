using UnityEngine;
using System.Collections;
using System;

public class Enemy : MonoBehaviour {
	public float rot_speed_x = 100f;
	public float rot_speed_y = 100f;
	public float rot_speed_z = 100f;
	public bool local=false;
	private float speedX;
	private float speedZ;
	private bool isHit;

    public bool IsHit { get => isHit; set => isHit = value; }

    // Use this for initialization
    void Start () {
		speedX = 12f * UnityEngine.Random.value - 6f;
		speedZ = 12f * UnityEngine.Random.value - 6f;
		float rotation = (float)(Math.Atan2(speedX, speedZ) * 180 / Math.PI);
		transform.Rotate(transform.up, rotation);
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
			transform.Translate(new Vector3(speedX, 0f, speedZ) * Time.deltaTime, Space.World);
			if (transform.position.x < -400 || transform.position.x > 400 || transform.position.z < -400 || transform.position.z > 400)
			{
				Destroy(gameObject);
			}
        }
	}
}
