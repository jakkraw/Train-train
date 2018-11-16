using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationSpawner : MonoBehaviour {

    public static Station Spawn()
    {
        var instance = Instantiate(Resources.Load<GameObject>("Station"));
        var station = instance.GetComponent<Station>();

        if(Random.value < .7) { station.s1.Place(Passenger.SpawnRandom()); }
        if (Random.value < .7) { station.s2.Place(Passenger.SpawnRandom()); }
        if (Random.value < .2) { station.s3.Place(Passenger.SpawnRandom()); }
        return instance.GetComponent<Station>();
    }


}
