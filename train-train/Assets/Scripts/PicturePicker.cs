using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PicturePicker : MonoBehaviour {

    public GameObject imageTemplate;
    public GameObject selectedCounterTextBox;

    private List<Texture2D> selectedTextures;
    private int maxSelected;

	// Use this for initialization
	void Start () {
        float numOfColumns = 5.0f; // float just for less casting later
        float spacingX = GetComponent<GridLayoutGroup>().spacing.x;
        float cellWidth = ((float)Screen.width - (numOfColumns+1) * spacingX) / numOfColumns;
        float additionalPadding = (cellWidth - (int)cellWidth) * (numOfColumns+1);
        
        GetComponent<GridLayoutGroup>().cellSize = new Vector2((int)cellWidth, (int)cellWidth);
        GetComponent<GridLayoutGroup>().padding.left  = (int) (spacingX + additionalPadding / 2.0f);
        GetComponent<GridLayoutGroup>().padding.right = (int) (spacingX + additionalPadding / 2.0f);

        switch( Settings.picturePickerTarget )
        {
            case PicturePickerTarget.DRIVER:
                selectedTextures = new List<Texture2D>();
                selectedTextures.Add( Data.currentProfile.selectedDriver );
                maxSelected = 1;
                break;

            case PicturePickerTarget.PASSENGER:
                selectedTextures = Data.currentProfile.selectedPassengers;
                maxSelected = 4;
                break;

            case PicturePickerTarget.STATION_SYMBOL:
                for( int i = 0; i < Data.currentProfile.selectedSymbols.Count; i++ )
                    if( Data.currentProfile.selectedSymbols[i].texture )
                        selectedTextures.Add( Data.currentProfile.selectedSymbols[i].texture );
                maxSelected = 4;
                break;

            case PicturePickerTarget.NOT_SELECTED:
            default:
                Debug.Log( "PicturePickerTarget.NOT_SELECTED" );
                Debug.Assert( false );
                SceneManager.LoadScene( "Settings" );
                break;
        }
        
        LoadFromProfile();
    }

    public void BackOnClick()
    {
        SceneManager.LoadScene( "Settings" );
    }

    public void TakePictureOnClick( int maxSize = -1 )
    {
        /*NativeCamera.Permission permission = NativeCamera.TakePicture( ( path ) =>
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

                //NativeGallery.SaveImageToGallery(texture.EncodeToJPG(), "TrainTrain", "{0}.png", null /*fuck error handling, what can go wrong?*//*);
                HandlePictureAddition(new[] { texture } );
            }
        }, maxSize );*/
        ((GameObject) Instantiate( imageTemplate, transform )).GetComponent<Image>().color = Random.ColorHSV();
    }


    public void AddPicturesFromGalleryOnClick()
    {
        if( !NativeGallery.IsMediaPickerBusy() )
        {
            if( NativeGallery.CanSelectMultipleFilesFromGallery() )
                NativeGallery.GetImagesFromGallery( (paths) => HandlePictureAddition(paths),          "Select pictures", "image/*" );
            else
                NativeGallery.GetImageFromGallery(  (path)  => HandlePictureAddition(new[] { path }), "Select picture",  "image/*" );
        }
    }

    public bool HandleSelectRequest(Texture2D texture)
    {
        if(selectedTextures.Contains(texture))
        {
            selectedTextures.Remove( texture );
            ModifySelectedCounter( -1 );
            return false;
        }
        else if( selectedTextures.Count >= maxSelected )
        {
            return false;
        }
        else
        {
            selectedTextures.Add( texture );
            //TODO
            ModifySelectedCounter( 1 );
            return true;
        }
    }

    private void HandlePictureAddition(string[] paths)
    {
        List<Texture2D> textures = new List<Texture2D>();
        for( int i = 0; i < paths.Length; i++ )
            textures.Add( NativeGallery.LoadImageAtPath( paths[i], -1, false ) );
        HandlePictureAddition( textures.ToArray() );
    }

    private void HandlePictureAddition(Texture2D[] textures)
    {
        DrawPictures(textures);
        ModifySelectedCounter(textures.Length);
        AddToProfile(textures);
    }

    private void DrawPictures(Texture2D[] textures)
    {
        for( int i = 0; i < textures.Length; i++ )
        {
            Rect rect = new Rect(0, 0, textures[i].width, textures[i].height);
            Sprite sprite = Sprite.Create(textures[i], rect, new Vector2(0.5f, 0.5f));
            Image image = ((GameObject)Instantiate(imageTemplate, transform)).GetComponent<Image>();
            image.sprite = sprite;
            
            if( selectedTextures.Contains( textures[i] ) )
            {
                image.GetComponent<Picture>().DrawSelected( true );
            }
        }
    }

    private void AddToProfile( Texture2D[] textures, bool addToSelected = false )
    {
        switch( Settings.picturePickerTarget )
        {
            case PicturePickerTarget.DRIVER:
                if(addToSelected)
                {
                    Debug.Assert( textures.Length == 1 );
                    Data.currentProfile.selectedDriver = textures[0];
                }
                else
                    Data.currentProfile.drivers.AddRange( textures );
                break;

            case PicturePickerTarget.PASSENGER:
                if(addToSelected)
                    Data.currentProfile.selectedPassengers.AddRange( textures );
                else
                    Data.currentProfile.passengers.AddRange( textures );
                break;

            case PicturePickerTarget.STATION_SYMBOL:
                {
                    List<Symbol_> symbols = new List<Symbol_>();
                    for( int i = 0; i < textures.Length; i++ )
                        symbols.Add( new Symbol_( textures[i] ) );

                    if(addToSelected)
                        Data.currentProfile.selectedSymbols.AddRange( symbols );
                    else
                        Data.currentProfile.symbols.AddRange( symbols );
                    break;
                }

            case PicturePickerTarget.NOT_SELECTED:
            default:
                Debug.Log( "PicturePickerTarget.NOT_SELECTED" );
                Debug.Assert( false );
                SceneManager.LoadScene( "Settings" );
                break;
        }
    }

    private void LoadFromProfile()
    {
        switch( Settings.picturePickerTarget )
        {
            case PicturePickerTarget.DRIVER:
                DrawPictures( Data.currentProfile.drivers.ToArray() );
                break;

            case PicturePickerTarget.PASSENGER:
                DrawPictures( Data.currentProfile.passengers.ToArray());
                break;

            case PicturePickerTarget.STATION_SYMBOL:
                {
                    List<Texture2D> textures = new List<Texture2D>();
                    List<Symbol_> symbols = Data.currentProfile.symbols;
                    for( int i = 0; i < symbols.Count; i++ )
                        if( symbols[i].texture )
                            textures.Add( symbols[i].texture );
                    DrawPictures( textures.ToArray() );    
                    break;
                }

            case PicturePickerTarget.NOT_SELECTED:
            default:
                Debug.Log( "PicturePickerTarget.NOT_SELECTED" );
                Debug.Assert( false );
                SceneManager.LoadScene( "Settings" );
                break;
        }
    }

    private void ModifySelectedCounter(int delta)
    {
        TextMesh text = selectedCounterTextBox.GetComponent<TextMesh>();
        int result; int.TryParse( text.text, out result );
        result += delta;
        text.text = result.ToString();
    }
}
