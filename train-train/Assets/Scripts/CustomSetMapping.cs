using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSetMapping : SymbolRepresentation {

    public Image maskSymbol, maskText;
    private SymbolMapping mapping = null;
    private bool drawedMask = false;

    public void setMapping(SymbolMapping mapping){
        this.mapping = mapping;
        setSymbol( mapping.stationSymbol() );
    }

    public void onClick() {
        GetComponentInParent<CustomSetPicker>().HandleSelectRequest(mapping, gameObject);
        Start();
    }

    public void onEditMappingClick() {
        CustomPicker.Modify(mapping);
    }

    public void Update()
    {
        if(!drawedMask)
            Start();
    }

    public void Start() {
        if( mapping != null )
            DrawSelected( GetComponentInParent<CustomSetPicker>().isSelected( mapping ) );
    }

    public void DrawSelected(bool selectedState) {
        Color color = maskSymbol.color;
        color.a = selectedState ? 0.4f : 0.0f;
        maskSymbol.color = color;

        color = maskText.color;
        color.a = selectedState ? 0.4f : 0.0f;
        maskText.color = color;

        drawedMask = true;
    }
}
