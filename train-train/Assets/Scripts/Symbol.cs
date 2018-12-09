using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Symbol : MonoBehaviour {
    public Text text;
    public RawImage image;
    public Symbol_ symbol_;

    public void setSymbol(Symbol_ symbol_)
    {
        this.symbol_ = symbol_;
        if(symbol_.text.Length != 0)
        {
            text.text = symbol_.text;
            image.gameObject.SetActive(false);
            text.gameObject.SetActive(true);
        }
        else if(symbol_.texture != null)
        {
            image.texture = symbol_.texture;
            image.gameObject.SetActive(true);
            text.gameObject.SetActive(false);
        }
    }


}
