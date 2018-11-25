using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationSpawner : MonoBehaviour {

    public static Station Spawn()
    {
        var instance = Instantiate(Resources.Load<GameObject>("Station"));
        var station = instance.GetComponent<Station>();

        foreach(var seat in station.seats)
        {
            if (Random.value < .7) { seat.Place(Passenger.SpawnRandom()); }
        }

        return instance.GetComponent<Station>();
    }


}
