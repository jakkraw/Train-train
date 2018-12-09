using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour {

    public Train train;
    public Station firstStation;

	// Use this for initialization
	void Start () {
        PassegersTextures.load();
        Symbols.load();

        foreach (var seat in firstStation.seats)
            seat.Place(Passenger.SpawnRandom());
    }
	
}
