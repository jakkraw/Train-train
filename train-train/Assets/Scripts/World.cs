using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public Train train;
    public Environment environment;

    private void Start()
    {
        train.BackSeat.Place(Passenger.Spawn());
    }

    // Update is called once per frame
    void Update () {
        HandleInput();
        MoveWorld();
        train.Decelerate();
    }

    void MoveWorld()
    {
        environment.SetMoveSpeed(-train.Speed);
        foreach (GameObject station in GameObject.FindGameObjectsWithTag("Station"))
        {
            station.transform.Translate(Time.deltaTime * -train.Speed, 0.0f, 0.0f);
        }
    }

    void SwapSeat(Seat a, Seat b)
    {
        var p1 = a.Remove();
        var p2 = b.Remove();
        if(p2) a.Place(p2);
        if(p1) b.Place(p1);
    }

    Station ClosestStation()
    {
        var a = GameObject.FindGameObjectsWithTag("Station");
        return a[0].GetComponent<Station>();
    }

    void HandleInput()
    {
        if (Input.touchCount < 1) return;

        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.touches[0].position), out hit, 500)) {
            train.Break();
            return;
        }

        var gameObject = hit.collider.gameObject;
        
        if(gameObject.name == "Train") {
            gameObject.GetComponent<Train>().Accelerate();
        }

        if (gameObject.CompareTag("Train Seat"))
        {
            var seat = gameObject.GetComponent<Seat>();
            if(!seat.isEmpty()) SwapSeat(ClosestStation().s1, seat);
        }

        if (gameObject.CompareTag("Station Seat"))
        {
            var seat = gameObject.GetComponent<Seat>();
            if (!seat.isEmpty()) SwapSeat(train.BackSeat, seat);
        }

    }

}
