using UnityEngine;
using System.Collections;

public class shoot : MonoBehaviour {
    public float speed = 0;
	public AudioClip hitSound;
    Vector3 lastPosition = Vector3.zero;
    Vector3 lastCueBallPosition;
    Vector3 lastPlayerPosition;
	float lastLocalCueStickPositionZ;
	private bool collide = false;
	public GameObject cueBall;
	public GameObject player;
	private Vector3 RotateDirection = Vector3.left;
	
	void Start(){
		transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
		lastLocalCueStickPositionZ = transform.localPosition.z;
	}
	/*
	void OnCollisionEnter(Collision collision){
	    if(collision.gameObject.name == "ball-cue"){
			Vector3 force = transform.forward;
			force.y = 0;
			collision.rigidbody.AddForceAtPosition(force * (speed * 50000), collision.contacts[0].point);
			audio.PlayOneShot(hitSound);
			this.gameObject.collider.enabled = false;
		}
		else{
			Physics.IgnoreCollision(rigidbody.collider, collision.collider, true);
		}
    }*/
	void Update()
	{	
		//if (cueBall.transform.position != lastCueBallPosition){
		//	this.gameObject.collider.enabled = false;
		//}
		if (cueBall.transform.position == lastCueBallPosition && player.transform.position == lastPlayerPosition && !SixenseInput.IsBaseConnected(0)){
			transform.LookAt(cueBall.transform);
			this.gameObject.transform.Translate(new Vector3(0, 0, (float)(Input.GetAxis("Mouse Y") * 0.03)));
		}
		else{
			transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
			transform.LookAt(new Vector3(cueBall.transform.position.x, ((float)(cueBall.transform.position.y + 0.25f)), cueBall.transform.position.z));
		}
		lastCueBallPosition = cueBall.transform.position;
		lastPlayerPosition = player.transform.position;
		lastLocalCueStickPositionZ = transform.localPosition.z;	
	} 

	
    void FixedUpdate()
    {
        speed = (transform.position - lastPosition).magnitude;
        lastPosition = transform.position;
    }	
}
