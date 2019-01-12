using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Train : MonoBehaviour
{

    public List<Seat> seats;
    public Transform middle;
    public Image driver;

    public Seat FreeSeat()
    {
        var seat = seats.Find(s => s.isEmpty());
        if (seat && seat.blocked) {
            return null;
        }

        return seat;
    }

    public float AccelerationSpeed = 8;
    public float DecelerationSpeed = 6;
    public float BreakSpeed = 20;
    public float SpeedLimit = 500;

    private float _speed = 0;
    public float Speed {
        get { return _speed; }
        set {
            _speed = Mathf.Clamp(value, 0f, SpeedLimit);
        }
    }

    public void Accelerate()
    {
        Speed += Time.deltaTime * AccelerationSpeed;
    }

    public void Decelerate()
    {
        Speed -= Time.deltaTime * DecelerationSpeed;
    }

    public void Break()
    {
        Speed -= Time.deltaTime * BreakSpeed;
    }

    public void playLeave()
    {
        GetComponent<Animator>().Play("train_leave");
    }


}
