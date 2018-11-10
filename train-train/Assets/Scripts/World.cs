using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public Train train;
    public Environment environment;

    private bool InputAccelerateTrain() {
        return Input.touchCount > 0 || Input.GetKey(KeyCode.RightArrow);
    }

    private bool InputBreakTrain() {
        return Input.GetKey(KeyCode.LeftArrow);
    }

    // Update is called once per frame
    void Update () {
        if (InputAccelerateTrain()) {  train.Accelerate(); }
        if (InputBreakTrain()) { train.Break(); }
        environment.SetMoveSpeed(-train.Speed);
    }
}
