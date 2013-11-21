using UnityEngine;
using System.Collections;

public class scratch : MonoBehaviour {
	private Vector3 InitialPosition;
	private Quaternion InitialRotation;
	public AudioClip pocketSound;
	// Use this for initialization
	void Start () {
		InitialPosition = this.gameObject.transform.position;
		InitialRotation = this.gameObject.transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		if (this.gameObject.transform.position.y < (InitialPosition.y - 0.5f)){
			audio.PlayOneShot(pocketSound);
			this.gameObject.transform.position = InitialPosition;	
			this.gameObject.transform.localRotation = InitialRotation;
			rigidbody.velocity = new Vector3(0, 0, 0);
		}
	}
}
