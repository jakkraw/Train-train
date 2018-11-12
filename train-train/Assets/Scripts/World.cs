using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public Train train;
    public Environment environment;

    private bool InputAccelerateTrain()
    {
        return Input.touchCount == 1;
    }

    private bool InputBreakTrain()
    {
        return Input.touchCount == 2;
    }

    // Update is called once per frame
    void Update () {
        HandleInput();

        train.Decelerate();
        if (InputAccelerateTrain()) {  train.Accelerate(); }
        if (InputBreakTrain()) { train.Break(); }
        environment.SetMoveSpeed(-train.Speed);

        foreach(GameObject station in GameObject.FindGameObjectsWithTag("Station"))
        {
            station.transform.Translate(Time.deltaTime * - train.Speed, 0.0f, 0.0f);
        }

    }


    void HandleInput()
    {
        if (Input.touchCount < 1) return;

        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.touches[0].position), out hit, 500)) { return; }

        var gameObject = hit.collider.gameObject;

        if(gameObject.name == "Train") {
            gameObject.GetComponent<Train>().Accelerate();
        }

        if (gameObject.name == "Seat")
        {
            var seat = gameObject.GetComponent<Seat>();
            if (seat.isEmpty())
            {
                seat.Place(Passenger.Spawn());
            }
            else
            {
                var passenger = seat.Remove();
                Destroy(passenger.gameObject);
            }
        }
   
    }

}
