using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Picture : MonoBehaviour {

    public Image child;

    void Start()
    {
        DrawSelected( GetComponentInParent<PicturePicker>().isSelected( GetComponent<Image>().sprite.texture ) );
    }

    public void onClick()
    {
        bool isSelected = GetComponentInParent<PicturePicker>().HandleSelectRequest( GetComponent<Image>().sprite.texture );
        bool isDeleting = GetComponentInParent<PicturePicker>().isDeleteModeActive();
        if(!isDeleting)
            DrawSelected( isSelected );
        else
        {
            GetComponentInParent<PicturePicker>().HandleDeleteRequest( GetComponent<Image>().sprite.texture, isSelected );
            Destroy( gameObject );
        }
    }

    public void DrawSelected(bool selectedState )
    {
        Color color = child.color;
        color.a = selectedState ? 0.4f : 0.0f;
        child.color = color;
    }
}
