using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public Train train;
    public Environment environment;

    private bool InputAccelerateTrain()
    {
        return Input.touchCount == 1;
    }

    private bool InputBreakTrain()
    {
        return Input.touchCount == 2;
    }

    // Update is called once per frame
    void Update () {
        train.Decelerate();
        if (InputAccelerateTrain()) {  train.Accelerate(); }
        if (InputBreakTrain()) { train.Break(); }
        environment.SetMoveSpeed(-train.Speed);
    }
}
