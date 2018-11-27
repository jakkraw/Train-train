using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationSpawner : MonoBehaviour {

    private static int number = 2;

    public static Station Spawn()
    {
        
        var instance = Instantiate(Resources.Load<GameObject>("Station"));
        var station = instance.GetComponent<Station>();
        station.number = number;
        station.stationNumber.text = number++.ToString();

        foreach (var seat in station.seats)
        {
            if (Random.value < .7) { seat.Place(Passenger.SpawnRandom()); }
        }

        return instance.GetComponent<Station>();
    }


}
