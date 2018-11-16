using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour {

    public Seat BackSeat, FrontSeat, DriverSeat;

    public bool hasSeat() {
        return BackSeat.isEmpty();
    }

    public void Seat(Passenger passenger)
    {
        BackSeat.Place(passenger);
    }

    public Seat freeSeat()
    {
        if (BackSeat.isEmpty()) return BackSeat;
        if (FrontSeat.isEmpty()) return FrontSeat;
        if (DriverSeat.isEmpty()) return DriverSeat;
        return null;
    }

    public float AccelerationSpeed = 8;
    public float DecelerationSpeed = 6;
    public float BreakSpeed = 20;
    public float SpeedLimit = 500;

    private float _speed = 0;
    public float Speed {
        get { return _speed; }
        set {
            _speed = Mathf.Min(SpeedLimit, value);
            _speed = Mathf.Max(0, _speed);
        }
    }

    public void Accelerate() {
        Speed += Time.deltaTime * AccelerationSpeed;
    }

    public void Decelerate() {
        Speed -= Time.deltaTime * DecelerationSpeed;
    }

    public void Break() {
        Speed -= Time.deltaTime * BreakSpeed;
    }

  
}
