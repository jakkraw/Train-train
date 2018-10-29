using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrain : MonoBehaviour {

    private float speed = 0;
    public float acceleration_speed = 8;
    public float decelleration_speed = 7;
    public FreeParallax parallax;
    public Transform train;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.touchCount > 0)
        {
            speed += acceleration_speed * Time.deltaTime;
        }
        else
        {
            speed = Mathf.Max(0, speed - decelleration_speed * Time.deltaTime);
        }
      

        if (parallax != null)
        {
            parallax.Speed = -speed;
        }

    }
}
