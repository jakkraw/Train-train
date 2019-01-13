using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    public SymbolRepresentation symbolRepresentation;
    public List<Seat> seats;
    public Transform middle;

    public Symbol symbol { get { return symbolRepresentation.symbol; } set { symbolRepresentation.setSymbol(value); } }

    public static Station Spawn(Symbol s)
    {
        var station = Instantiate(Resources.Load<GameObject>("Station")).GetComponent<Station>();
        station.symbol = s;
        return station;
    }

    public Seat FreeSeat()
    {
        return seats.Find(s => s.isEmpty());
    }
}
