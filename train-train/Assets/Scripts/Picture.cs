using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Picture : MonoBehaviour {

    public void onClick()
    {
        bool drawSelected = GetComponentInParent<PicturePicker>().HandleSelectRequest( GetComponent<Image>().sprite.texture );
        DrawSelected( drawSelected );
    }

    public void DrawSelected(bool draw)
    {
        if( draw )
            GetComponent<Image>().color = new Color( 255, 255, 0, 255 );
        else
            GetComponent<Image>().color = new Color( 255, 255, 255, 255 );
    }
}
