using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour {

    public Train train;
    public Environment environment;
    float _newStationDistance = 100;

    private bool _picksPassengers;
    private bool PicksPassengers {
        get
        {
            return _picksPassengers;
        }
        set
        {
            if(value != _picksPassengers)
            {
                foreach (Seat seat in FindObjectsOfType<Seat>()) {
                    var collider = seat.GetComponent<BoxCollider>();
                    collider.enabled = value;
                }

                foreach (Passenger passenger in FindObjectsOfType<Passenger>())
                {
                    var image = passenger.GetComponent<Image>();
                    var color = image.color;
                    color.a = value ? 1f : 0.5f;
                    image.color = color; 
                }

                var timage = train.GetComponent<SpriteRenderer>();
                var tcolor = timage.color;
                tcolor.a = value ? 0.5f : 1f;
                timage.color = tcolor;

            }
            _picksPassengers = value;
        }
    }

    private void Start()
    {
        PicksPassengers = true;
        PicksPassengers = false;
        if(Random.value < .4) train.BackSeat.Place(Passenger.SpawnRandom());
        if (Random.value < .4) train.FrontSeat.Place(Passenger.SpawnRandom());
        if (Random.value < .4) train.DriverSeat.Place(Passenger.SpawnRandom());
    }

    void Update () {
        SpawnStations(); 
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

    void SpawnStations()
    {
        if(Vector3.Distance(train.transform.position, ClosestStation().transform.position) > _newStationDistance)
        {
            Destroy(ClosestStation().gameObject);
            StationSpawner.Spawn();
            _newStationDistance = Random.Range(30, 130);
        }


    }

    void SwapSeat(Seat a, Seat b)
    {
        if (!a || !b) return;
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
        PicksPassengers = ClosestStation().PickingPassengers;

        if (Input.touchCount < 1) return;

        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.touches[0].position), out hit, 500)) {
            train.Break();
            return;
        }

        var gameObject = hit.collider.gameObject;
       
        if (PicksPassengers) { train.Speed = 0; }


        if (gameObject.CompareTag("Train") && !PicksPassengers) {
            train.Accelerate();
        }

        if (gameObject.CompareTag("Train Seat"))
        {
            var seat = gameObject.GetComponent<Seat>();
            if(!seat.isEmpty()) SwapSeat(ClosestStation().freeSeat(), seat);
        }

        if (gameObject.CompareTag("Station Seat"))
        {
            var seat = gameObject.GetComponent<Seat>();
            if (!seat.isEmpty()) SwapSeat(train.freeSeat(), seat);
        }

    }

}
