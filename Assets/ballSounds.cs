using UnityEngine;
using System.Collections;

public class ballSounds : MonoBehaviour {

	public AudioClip hitSound;
	public AudioClip pocketSound;
	private Vector3 InitialLocation;
	
	void Start(){
		InitialLocation = transform.position;
	}
	
	void OnCollisionEnter(Collision collider){
		if (collider.gameObject.name.Contains("ball-") && collider.relativeVelocity.magnitude > 1){
			audio.PlayOneShot(hitSound);
		}
	}	
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < InitialLocation.y){
			if (transform.position.y < (InitialLocation.y - 0.2f) && this.gameObject.renderer.enabled && this.gameObject.name != "ball-cue"){
				audio.PlayOneShot(pocketSound);
				this.gameObject.renderer.enabled = false;
				this.gameObject.collider.enabled = false;
			}
		}
		else if (transform.position.y > InitialLocation.y){
			transform.position = new Vector3(transform.position.x, InitialLocation.y, transform.position.z);
		}
	}
}
