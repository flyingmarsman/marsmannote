using UnityEngine;
using System.Collections;

public class Done_Mover : MonoBehaviour
{
	public float speed;

	void Start ()
	{
        gameObject.GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}
}
