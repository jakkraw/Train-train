using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Station : MonoBehaviour {
    public int number;
    public Symbol symbol;
    public List<Seat> seats;
    public Transform middle;

    public Seat FreeSeat() {
        return seats.Find(s => s.isEmpty());
    }

    private void Update()
    {
       var passengersToLeave = seats.FindAll(s => s.passenger && s.passenger.symbol.symbol_ == symbol.symbol_);
       foreach(var seat in passengersToLeave)
       {
            seat.leaveSeat();
        }
        
    }
}
