using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PicturePicker : MonoBehaviour {

    public GameObject imageTemplate;
    public GameObject selectedCounterTextBox;
    public string[] addedImages;
    public string[] selectedImages;

    private float numOfColumns; // float just for less casting later

	// Use this for initialization
	void Start () {
        numOfColumns = 5.0f;
        float spacingX = GetComponent<GridLayoutGroup>().spacing.x;
        float cellWidth = (float)Screen.width - (numOfColumns+1) * spacingX / numOfColumns;
        float additionalPadding = (cellWidth - (int)cellWidth) * (numOfColumns+1);
        
        GetComponent<GridLayoutGroup>().cellSize = new Vector2((int)cellWidth, (int)cellWidth);
        GetComponent<GridLayoutGroup>().padding.left  = (int) (spacingX + additionalPadding / 2.0f);
        GetComponent<GridLayoutGroup>().padding.right = (int) (spacingX + additionalPadding / 2.0f);

        //HandlePictureSelect(Data.currentProfile.);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void TakePictureOnClick( int maxSize = -1 )
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture( ( path ) =>
        {
            if( path != null )
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath( path );
                if( texture == null )
                {
                    Debug.Log( "Couldn't load texture from " + path );
                    return;
                }

                NativeGallery.SaveImageToGallery(texture.EncodeToJPG(), "TrainTrain", "{0}.png", null /*fuck error handling, what can go wrong?*/);
                HandlePictureSelect(new[] { "" }, texture );
            }
        }, maxSize );
    }

    public void BackOnClick()
    {
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex - 1 );
    }

    public void AddPicturesFromGallery()
    {
        if( !NativeGallery.IsMediaPickerBusy() )
        {
            if( NativeGallery.CanSelectMultipleFilesFromGallery() )
                NativeGallery.GetImagesFromGallery( (paths) => HandlePictureSelect(paths),          "Select pictures", "image/*" );
            else
                NativeGallery.GetImageFromGallery(  (path)  => HandlePictureSelect(new[] { path }), "Select picture",  "image/*" );
        }
    }

    private void HandlePictureSelect(string[] paths, Texture2D texture = null)
    {
        for( int i = 0; i < paths.Length; i++ )
        {
            if( texture == null)
                texture = NativeGallery.LoadImageAtPath( paths[i], -1, false );

            Rect rect = new Rect(0, 0, texture.width, texture.height);
            Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            Image image = ((GameObject)Instantiate(imageTemplate, transform)).GetComponent<Image>();
            image.sprite = sprite;
        }

        TextMesh text = selectedCounterTextBox.GetComponent<TextMesh>();
        int result; int.TryParse( text.text, out result );
        result += paths.Length;
        text.text = result.ToString();
    }

    private void Load()
    {
       
    }
}
