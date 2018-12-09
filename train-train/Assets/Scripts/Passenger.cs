using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;


public class PassegersTextures
{
    static List<Texture2D> passengers;

    public static void load()
    {
        passengers = new List<Texture2D>();

        var strings = new List<string>() { "Images/Bee", "Images/Monkey", "Images/Mouse" };
        foreach (var img in strings)
        {
            passengers.Add(Resources.Load<Texture2D>(img));
        }
    }

    public static Texture2D getRandomPassengerTexture()
    {
        return passengers[Random.Range(0, passengers.Count)];
    }

}


public class Passenger : MonoBehaviour {

    public RawImage image;
    public Symbol symbol;
    public static Passenger SpawnRandom()
    {
        var destination = Symbols.randomPossibleDestination();
        if (destination == null) { return null; }

        var instance = Instantiate(Resources.Load<GameObject>("Passenger"));
        var passenger = instance.GetComponent<Passenger>();
        passenger.image.texture = PassegersTextures.getRandomPassengerTexture();

        passenger.symbol.setSymbol(destination);
        return passenger;
    }

    public void setActive(bool active)
    {
        var color = image.color;
        color.a = active ? 1f : 0.5f;
        image.color = color;
    }
}
