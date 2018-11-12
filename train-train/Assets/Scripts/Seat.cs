using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    private Passenger passenger;

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
}