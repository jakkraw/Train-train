using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class World : MonoBehaviour {

    public Train train;
    public Environment environment;
    float _newStationDistance = 100;
    bool enablePassengerMove = false;

    void Update () {
        SpawnStations(); 
        HandleInput();
        MoveWorld();
        train.Decelerate();

        var rect = ClosestStation().GetComponent<BoxCollider2D>().bounds;
        enablePassengerMove = train.seats.All(seat => rect.Contains(seat.transform.position));

        foreach (Seat seat in FindObjectsOfType<Seat>()) {
            seat.setActive(train.Speed == 0 && enablePassengerMove);
        }
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
        var trainPos = train.transform.position;
        var station = ClosestStation();
        var stationPos = station.transform.position;
        if (Vector3.Distance(trainPos, stationPos) > _newStationDistance && trainPos.x > stationPos.x)
        {
            Destroy(station.gameObject);
            StationSpawner.Spawn();
            _newStationDistance = Random.Range(30, 40);
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
        if(a.Length == 0) {
            _newStationDistance = Random.Range(30, 40);
            return StationSpawner.Spawn();
        }
        return a[0].GetComponent<Station>();
    }

    void HandleInput()
    {


        if (Input.touchCount < 1)
        {
            if(train.Speed != 0)
            {
                var trainPos = train.middle.position;
                var station = ClosestStation();
                var stationPos = station.middle.position;
                if (trainPos.x < stationPos.x)
                {
                    var k = Vector3.Distance(train.middle.position, station.middle.position) / 10.0f;
                    if (k < 2.5)
                    {
                        train.Break();
                        if (train.Speed < 4) train.Speed = 4;
                        if (k < 0.1)
                        {
                            if (enablePassengerMove)
                            {
                                train.Speed = 0;
                            }
                        }
                    }
                }
            }   
            return;
        }


        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.touches[0].position), out hit, 500)) {
            train.Break();
            return;
        }

        var gameObject = hit.collider.gameObject;


        if (gameObject.CompareTag("Accelerate")) {
            train.Accelerate();
        }

        if (gameObject.CompareTag("Train Seat"))
        {
            var seat = gameObject.GetComponent<Seat>();
            if(!seat.isEmpty()) SwapSeat(ClosestStation().FreeSeat(), seat);
        }

        if (gameObject.CompareTag("Station Seat"))
        {
            var seat = gameObject.GetComponent<Seat>();
            var train_seat = train.FreeSeat();
            if(train_seat) { SwapSeat(train_seat, seat); }
        }

    }

}
