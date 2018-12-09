using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Passenger : MonoBehaviour {

    public RawImage image;
    public Symbol symbol;
    public static Passenger GetPassenger(Symbol_ s, Texture2D t)
    {
        var p = Instantiate(Resources.Load<GameObject>("Passenger")).GetComponent<Passenger>();
        p.setTexture(t);
        p.setDestination(s);
        return p;
    }

    public void setDestination(Symbol_ s)
    {
        symbol.setSymbol(s);
    }

    public void setTexture(Texture2D t)
    {
        image.texture = t;
    }

    public void setActive(bool active)
    {
        var color = image.color;
        color.a = active ? 1f : 0.5f;
        image.color = color;
    }
}
