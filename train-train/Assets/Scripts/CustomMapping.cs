using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomMapping : SymbolRepresentation {

    public Image textMask, imageMask;
    private bool drawedMask = false;

    public void onClick() {
        GetComponentInParent<CustomPicker>().HandleSelectRequest(symbol, gameObject);
        Start();
    }

    public void Update(){
        if(!drawedMask)
            Start();
    }

    public void Start() {
        if( symbol != null ) 
            DrawSelected(GetComponentInParent<CustomPicker>().isSelected(symbol));
    }

    public void DrawSelected(bool selectedState) {
        Color color = textMask.color;
        color.a = selectedState ? 0.4f : 0.0f;
        textMask.color = color;

        color = imageMask.color;
        color.a = selectedState ? 0.4f : 0.0f;
        imageMask.color = color;

        drawedMask = true;
    }
}
