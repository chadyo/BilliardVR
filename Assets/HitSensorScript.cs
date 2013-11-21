using UnityEngine;
using System.Collections;

public class HitSensorScript : MonoBehaviour {
    public float speed = 0;	
	public AudioClip hitSound;
    Vector3 lastPosition = Vector3.zero;

	void Update()
	{	
        speed = (transform.position - lastPosition).magnitude;
        lastPosition = transform.position;
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit) && hit.collider.gameObject.name == "ball-cue" && hit.distance < 0.030f){
			Vector3 force = transform.forward;
			force.y = 0;
			//hit.collider.rigidbody.AddForceAtPosition(force * (speed * 80000), hit.point);
			hit.collider.rigidbody.AddForce(force * (speed * 50000));
			audio.PlayOneShot(hitSound);
		}
	}	
}
