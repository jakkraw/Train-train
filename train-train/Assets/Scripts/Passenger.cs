using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Passenger : MonoBehaviour {

    public int stationNumber = 1;

    public static Passenger SpawnRandom()
    {
        var strings = new List<string>() { "Images/Bee", "Images/Monkey", "Images/Mouse" };
      
        var instance = Instantiate(Resources.Load<GameObject>("Passenger"));
        instance.GetComponent<Image>().sprite = Resources.Load<Sprite>(strings[Random.Range(0, strings.Count)]);
        var passenger = instance.GetComponent<Passenger>();
        passenger.stationNumber = Random.Range(1, 10);
        return passenger;
    }

    

}
