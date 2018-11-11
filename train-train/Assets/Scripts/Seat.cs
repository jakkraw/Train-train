using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public GameObject passenger;

    public bool isEmpty() { return passenger == null; }
    public void Place(GameObject passenger) {
        passenger.transform.SetParent(transform);
        this.passenger = passenger;

        var p = passenger.GetComponent<RectTransform>();
        var my = GetComponent<RectTransform>();
        p.sizeDelta = my.sizeDelta;
        p.position = my.position;
    }
    public void Remove() {
        Destroy(passenger);
    }
}