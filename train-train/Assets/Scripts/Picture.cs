using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Picture : MonoBehaviour {

    public Image child;

    public void onClick()
    {
        bool isSelected = GetComponentInParent<PicturePicker>().HandleSelectRequest( GetComponent<Image>().sprite.texture );
        DrawSelected( isSelected );
    }

    public void DrawSelected(bool selectedState )
    {
        Color color = child.color;
        color.a = selectedState ? 0.4f : 0.0f;
        child.color = color;
    }
}
