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
                train.Seat(Instantiate(Resources.Load<GameObject>("Passenger")));
            }
        }
        if (Input.GetKey(KeyCode.DownArrow)){
            train.BackSeat.Remove();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Pressed left click, casting ray.");
            CastRay();
        }

    }

    void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            Debug.DrawLine(ray.origin, hit.point);
            Debug.Log("Hit object: " + hit.collider.gameObject.name);

            var el = hit.collider.gameObject.GetComponent<Seat>();
            if (el)
            {
                if (el.isEmpty())
                {
                    el.Place(Instantiate(Resources.Load<GameObject>("Passenger")));
                }
                else
                {
                    el.Remove();
                }
            }

        }
    }

}
