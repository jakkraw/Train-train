using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Station : MonoBehaviour {
    bool _pickingPassengers = false;
    public bool PickingPassengers
    {
        get { return _pickingPassengers; } 
        set { _pickingPassengers = value; }
    }

    public Seat freeSeat()
    {
        if (s1.isEmpty()) return s1;
        if (s2.isEmpty()) return s2;
        if (s3.isEmpty()) return s3;
        return null;
    }

    public Seat s1, s2, s3;
}
