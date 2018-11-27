using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Station : MonoBehaviour {
    public int number;
    public Text stationNumber;
    public List<Seat> seats;

    public Seat FreeSeat() {
        return seats.Find(s => s.isEmpty());
    }

    private void Update()
    {
       var passengersToLeave = seats.FindAll(s => s.passenger && s.passenger.stationNumber == number);
       foreach(var seat in passengersToLeave)
       {
            var passenger = seat.passenger;
            Destroy(passenger.gameObject);
            seat.passenger = null;
        }
        
    }
}
