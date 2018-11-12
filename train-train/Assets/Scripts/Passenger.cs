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

}
