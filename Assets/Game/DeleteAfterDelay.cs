using UnityEngine;
using System.Collections;

public class DeleteAfterDelay : MonoBehaviour
{
	public float delay = 0.4f;

	void Update()
	{
		delay -= Time.deltaTime;
		if (delay < 0f)
			GameObject.Destroy(this.gameObject);
	}
}
