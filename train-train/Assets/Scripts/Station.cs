using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Station : MonoBehaviour {
    public List<Seat> seats;

    public Seat FreeSeat() {
        return seats.Find(s => s.isEmpty());
    }
}
