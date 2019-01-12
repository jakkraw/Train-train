using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolRepresentation : MonoBehaviour {
    public Text text;
    public RawImage image;
    public Symbol symbol;

    public void setSymbol(Symbol symbol)
    {
        this.symbol = symbol;
        if(symbol.text.Length != 0)
        {
            text.text = symbol.text;
            image.gameObject.SetActive(false);
            text.gameObject.SetActive(true);
        }
        else if(symbol.texture != null)
        {
            image.texture = symbol.texture;
            image.gameObject.SetActive(true);
            text.gameObject.SetActive(false);
        }
    }


}
