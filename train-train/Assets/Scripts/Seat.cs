using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Seat : MonoBehaviour
{
    public Passenger passenger;
    public Text text;

    public bool isEmpty() { return passenger == null; }
    public void Place(Passenger passenger) {
 
        passenger.transform.SetParent(transform);
        this.passenger = passenger;

        var p = passenger.GetComponent<RectTransform>();
        var my = GetComponent<RectTransform>();
        p.sizeDelta = my.sizeDelta;
        p.position = my.position;
        p.localScale = my.localScale;
    }
    public Passenger Remove() {
        var tmp = passenger;
        passenger = null;
        return tmp;
    }

    private void Update()
    {
        if (text)
        {
            if (passenger) { text.text = passenger.stationNumber.ToString(); }
            else { text.text = ""; }
        }
    }

}