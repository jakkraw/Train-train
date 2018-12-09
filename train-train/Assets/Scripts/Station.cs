using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Station : MonoBehaviour {
    public Symbol symbol;
    public List<Seat> seats;
    public Transform middle;

    public static Station Spawn(Symbol_ s)
    {
        var station = Instantiate(Resources.Load<GameObject>("Station")).GetComponent<Station>();
        station.setSymbol(s);
        return station;
    }


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

    public void setSymbol(Symbol_ s)
    {
        symbol.setSymbol(s);
    }

}
