using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;


public class Passenger : MonoBehaviour {

   

    public static Passenger Spawn()
    {
        var instance = Instantiate(Resources.Load<GameObject>("Passenger"));
        instance.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Bee");
        return instance.GetComponent<Passenger>();
    }

    public static Passenger SpawnRandom()
    {
        var strings = new List<string>() { "Images/Bee", "Images/Monkey", "Images/Mouse" };
      
        var instance = Instantiate(Resources.Load<GameObject>("Passenger"));
        instance.GetComponent<Image>().sprite = Resources.Load<Sprite>(strings[Random.Range(0, strings.Count)]);
        return instance.GetComponent<Passenger>();
    }

}
