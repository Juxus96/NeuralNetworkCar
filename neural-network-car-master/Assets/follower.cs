using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject follow;
    public Camera cam;
	public float bias;

	// Use this for initialization
	void Start () {
			
	}
	
	// Update is called once per frame
	void Update () {
		
		Vector3 moveCamTo = follow.transform.position - transform.forward * 10f ;

		cam.transform.position = cam.transform.position * bias + moveCamTo * (1f - bias);
		
		cam.transform.LookAt (follow.transform.position);

	}
}
