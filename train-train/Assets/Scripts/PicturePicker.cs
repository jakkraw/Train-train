using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PicturePicker : MonoBehaviour {

    public GameObject imageTemplate;
    public GameObject selectedCounterTextBox;

	// Use this for initialization
	void Start () {

        switch( Settings.picturePickerTarget )
        {
            case PicturePickerTarget.DRIVER:
                ModifySelectedCounter( 1 );
                break;

            case PicturePickerTarget.PASSENGER:
                ModifySelectedCounter( Data.currentProfile.selectedPassengers.Count );
                break;

            case PicturePickerTarget.STATION_SYMBOL:
                {
                    int counter = 0;
                    for( int i = 0; i < Data.currentProfile.selectedSymbols.Count; i++ )
                        if( Data.currentProfile.selectedSymbols[i].texture )
                            counter++;
                    ModifySelectedCounter( counter );
                    break;
                }
            
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
        switch( Settings.picturePickerTarget )
        {
            case PicturePickerTarget.DRIVER:
                if( Data.currentProfile.selectedDriver == null )
                    return;
                break;

            case PicturePickerTarget.PASSENGER:
                if( Data.currentProfile.selectedPassengers.Count == 0 )
                    return;
                break;

            case PicturePickerTarget.STATION_SYMBOL:
                break;

            case PicturePickerTarget.NOT_SELECTED:
            default:
                Debug.Log( "PicturePickerTarget.NOT_SELECTED" );
                Debug.Assert( false );
                SceneManager.LoadScene( "Settings" );
                break;
        }
        SceneManager.LoadScene( "Settings" );
    }

    private void ModifySelectedCounter( int delta )
    {
        TextMeshProUGUI text = selectedCounterTextBox.GetComponent<TextMeshProUGUI>();
        int result; int.TryParse( text.text, out result );
        result += delta;
        text.text = result.ToString();
    }

    public bool isSelected( Texture2D texture )
    {
        switch( Settings.picturePickerTarget )
        {
            case PicturePickerTarget.DRIVER:
                if( Data.currentProfile.selectedDriver == null )
                    return false;
                return Data.currentProfile.selectedDriver.Equals( texture );

            case PicturePickerTarget.PASSENGER:
                return Data.currentProfile.selectedPassengers.Contains( texture );

            case PicturePickerTarget.STATION_SYMBOL:
                Symbol_ symbol = new Symbol_( texture );
                return Data.currentProfile.selectedSymbols.Contains(symbol);

            case PicturePickerTarget.NOT_SELECTED:
            default:
                Debug.Log( "PicturePickerTarget.NOT_SELECTED" );
                Debug.Assert( false );
                SceneManager.LoadScene( "Settings" );
                return false;
        }
    }

    public bool HandleSelectRequest( Texture2D texture )
    {
        if( isSelected( texture ) )
        {
            RemoveSelectedFromProfile( texture );
            ModifySelectedCounter( -1 );
            return false;
        }
        else if( Settings.picturePickerTarget == PicturePickerTarget.DRIVER &&
                 Data.currentProfile.selectedDriver != null )
        {
            //driver can only have on picture selected
            //and this request is not unselect becouse of failure of previous check
            //then it is another select, which is not allowed
            return false;
        }
        else
        {
            AddToProfile( new[] { texture }, true );
            ModifySelectedCounter( 1 );
            return true;
        }
    }

    private void RemoveSelectedFromProfile( Texture2D texture )
    {
        switch( Settings.picturePickerTarget )
        {
            case PicturePickerTarget.DRIVER:
                Data.currentProfile.selectedDriver = null;
                break;

            case PicturePickerTarget.PASSENGER:
                Data.currentProfile.selectedPassengers.Remove( texture );
                break;

            case PicturePickerTarget.STATION_SYMBOL:
                Symbol_ symbol = new Symbol_( texture );
                Data.currentProfile.selectedSymbols.Remove( symbol );
                break;

            case PicturePickerTarget.NOT_SELECTED:
            default:
                Debug.Log( "PicturePickerTarget.NOT_SELECTED" );
                Debug.Assert( false );
                SceneManager.LoadScene( "Settings" );
                break;
        }
    }

    private void AddToProfile( Texture2D[] textures, bool addToSelected = false )
    {
        switch( Settings.picturePickerTarget )
        {
            case PicturePickerTarget.DRIVER:
                if( addToSelected )
                {
                    Debug.Assert( textures.Length == 1 );
                    Data.currentProfile.selectedDriver = textures[0];
                }
                else
                    Data.currentProfile.drivers.AddRange( textures );
                break;

            case PicturePickerTarget.PASSENGER:
                if( addToSelected )
                    Data.currentProfile.selectedPassengers.AddRange( textures );
                else
                    Data.currentProfile.passengers.AddRange( textures );
                break;

            case PicturePickerTarget.STATION_SYMBOL:
                {
                    List<Symbol_> symbols = new List<Symbol_>();
                    for( int i = 0; i < textures.Length; i++ )
                        symbols.Add( new Symbol_( textures[i] ) );

                    if( addToSelected )
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
                DrawPictures( Data.currentProfile.passengers.ToArray() );
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

                //NativeGallery.SaveImageToGallery(texture.EncodeToJPG(), "TrainTrain", "{0}.png", null /*fuck error handling, what can go wrong?*//*);
                HandlePictureAddition(new[] { texture } );
            }
        }, maxSize );
        //((GameObject) Instantiate( imageTemplate, transform )).GetComponent<Image>().color = Random.ColorHSV();
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

    private void HandlePictureAddition(string[] paths)
    {
        List<Texture2D> textures = new List<Texture2D>();
        for( int i = 0; i < paths.Length; i++ )
            textures.Add( NativeGallery.LoadImageAtPath( paths[i], -1/* set resize(?) size here*/ ) );
        HandlePictureAddition( textures.ToArray() );
    }

    private void HandlePictureAddition(Texture2D[] textures)
    {
        DrawPictures(textures);
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
        }
    }
}
