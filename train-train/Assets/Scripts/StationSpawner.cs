using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Symbol_
{
    public string text = "";
    public Texture2D texture = null;

    public Symbol_(string t)
    {
        text = t;
    }

    public Symbol_(Texture2D t)
    {
        texture = t;
    }
}


public static class Symbols
{
    static int currentIndex;
    static List<Symbol_> symbols;

    public static void load()
    {
        currentIndex = 0;
        symbols = new List<Symbol_>();
        var strings = new List<string>() { "Images/Bee", "Images/Monkey", "Images/Mouse" };

        //foreach (var img in strings)
        //{
        //    symbols.Add(new Symbol_(Resources.Load<Texture2D>(img)));
        //}
        //symbols.Add(new Symbol_("1"));
        //symbols.Add(new Symbol_("hp"));
        symbols.Add(new Symbol_("lama"));
    }

    public static Symbol_ getNextStationSymbol()
    {
        if (currentIndex >= symbols.Count) { return null; }

        var symbol = symbols[currentIndex++];

        return symbol;
    }

    public static Symbol_ randomPossibleDestination()
    {
        if(currentIndex >= symbols.Count) { return null; }
        var destinations = symbols.GetRange(currentIndex, symbols.Count-currentIndex);
        return destinations[Random.Range(0, destinations.Count)];
    }

}

public class StationSpawner : MonoBehaviour {

    public static Station Spawn()
    {
        var instance = Instantiate(Resources.Load<GameObject>("Station"));
        var station = instance.GetComponent<Station>();
        var nextStationSymbol = Symbols.getNextStationSymbol();
        if(nextStationSymbol == null)
        {
            SceneManager.LoadScene("Menu");
            return null;
        }
        station.symbol.setSymbol(nextStationSymbol);
    
        foreach (var seat in station.seats)
        {
            var passenger = Passenger.SpawnRandom();
            if(passenger) seat.Place(passenger);
        }

        return instance.GetComponent<Station>();
    }


}
