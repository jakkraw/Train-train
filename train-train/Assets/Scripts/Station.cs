using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    private SymbolMapping mapping;
    public SymbolRepresentation symbolRepresentation;
    public List<Seat> seats;
    public Transform middle;

    public static Station Spawn(SymbolMapping s)
    {
        var station = Instantiate(Resources.Load<GameObject>("Station")).GetComponent<Station>();
        station.mapping = s;
        station.symbolRepresentation.setSymbol(station.mapping.stationSymbol());
        return station;
    }

    public Seat FreeSeat()
    {
        return seats.Find(s => s.isEmpty());
    }

    public bool doesMatch(Passenger passenger) {
        if(mapping == null) { return false; }
        return mapping.doesMatch(passenger.symbol);
    }
}
