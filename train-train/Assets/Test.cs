using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public Train train;
    public Environment environment;

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow)) { train.Accelerate(); }
        if (Input.GetKey(KeyCode.LeftArrow)) { train.Break(); }
        if (Input.GetKey(KeyCode.UpArrow)) {
            if (train.hasSeat()) {
                train.Seat(Instantiate(Resources.Load<Passenger>("Passenger")));
            }
        }
        if (Input.GetKey(KeyCode.DownArrow)){
            var l = train.BackSeat.Remove();
            if(l != null)
            {
                Destroy(l.gameObject);
            }
            
        }
    }

}
