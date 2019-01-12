using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Station : MonoBehaviour {
    public SymbolRepresentation symbol;
    public List<Seat> seats;
    public Transform middle;

    public static Station Spawn(Symbol s)
    {
        var station = Instantiate(Resources.Load<GameObject>("Station")).GetComponent<Station>();
        station.setSymbol(s);
        return station;
    }


    public Seat FreeSeat() {
        return seats.Find(s => s.isEmpty());
    }

    public void setSymbol(Symbol s)
    {
        symbol.setSymbol(s);
    }

}
