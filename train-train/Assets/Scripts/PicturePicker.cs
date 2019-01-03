using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public enum PicturePickerTarget { 
    DRIVER,
    PASSENGER,
    STATION_SYMBOL
}

public class PicturePicker : MonoBehaviour {

    public static PicturePickerTarget picturePickerTarget = PicturePickerTarget.PASSENGER;
    public GameObject imageTemplate;
    public GameObject selectedCounterTextBox;

	// Use this for initialization
	void Start () {

        switch(picturePickerTarget)
        {
            case PicturePickerTarget.DRIVER:
                ModifySelectedCounter( 1 );
                break;

            case PicturePickerTarget.PASSENGER:
                ModifySelectedCounter( Data.Profile.selectedPassengers.Count );
                break;

            case PicturePickerTarget.STATION_SYMBOL:
                {
                    int counter = 0;
                    for( int i = 0; i < Data.Profile.selectedSymbols.Count; i++ )
                        if( Data.Profile.selectedSymbols[i].texture )
                            counter++;
                    ModifySelectedCounter( counter );
                    break;
                }
        }
        
        LoadFromProfile();
    }

    public void BackOnClick()
    {
        switch( picturePickerTarget )
        {
            case PicturePickerTarget.DRIVER:
                if( Data.Profile.selectedDriver == null )
                    return;
                break;

            case PicturePickerTarget.PASSENGER:
                if( Data.Profile.selectedPassengers.Count == 0 )
                    return;
                break;

            case PicturePickerTarget.STATION_SYMBOL:
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
        switch( picturePickerTarget )
        {
            default:
            case PicturePickerTarget.DRIVER:
                if( Data.Profile.selectedDriver == null )
                    return false;
                return Data.Profile.selectedDriver.Equals( texture );

            case PicturePickerTarget.PASSENGER:
                return Data.Profile.selectedPassengers.Contains( texture );

            case PicturePickerTarget.STATION_SYMBOL:
                StationSymbol symbol = new StationSymbol( texture );
                return Data.Profile.selectedSymbols.Exists( s => s.texture == texture);
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
        else if( picturePickerTarget == PicturePickerTarget.DRIVER &&
                 Data.Profile.selectedDriver != null )
        {
            //driver can only have on picture selected
            //and this request is not unselect becouse of failure of previous check
            //then it is another select, which is not allowed
            return false;
        }
        else
        {
            AddToProfile(new List<Texture2D> { texture }, true );
            ModifySelectedCounter( 1 );
            return true;
        }
    }

    private void RemoveSelectedFromProfile( Texture2D texture )
    {
        switch( picturePickerTarget )
        {
            case PicturePickerTarget.DRIVER:
                Data.Profile.selectedDriver = null;
                break;

            case PicturePickerTarget.PASSENGER:
                Data.Profile.selectedPassengers.RemoveAll(t => t == texture);
                break;
            
            case PicturePickerTarget.STATION_SYMBOL:
                Data.Profile.selectedSymbols.RemoveAll( s => s.texture == texture );
                break;
            default: break;
        }
    }

    private void AddToProfile( List<Texture2D> textures, bool addToSelected = false )
    {
       
        switch( picturePickerTarget )
        {
            case PicturePickerTarget.DRIVER:
                if( addToSelected )
                {
                    Debug.Assert( textures.Count == 1 );
                    Data.Profile.selectedDriver = textures[0];
                }
                else
                    Data.Profile.drivers.AddRange( textures );
                break;

            case PicturePickerTarget.PASSENGER:
                if( addToSelected )
                    Data.Profile.selectedPassengers.AddRange( textures );
                else
                    Data.Profile.passengers.AddRange( textures );
                break;

            case PicturePickerTarget.STATION_SYMBOL:
                {
                    List<StationSymbol> symbols = new List<StationSymbol>();
                    for( int i = 0; i < textures.Count; i++ )
                        symbols.Add( new StationSymbol( textures[i] ) );

                    if( addToSelected )
                        Data.Profile.selectedSymbols.AddRange( symbols );
                    else
                        Data.Profile.symbols.AddRange( symbols );
                    break;
                }
            default: break;
        }
    }

    private void LoadFromProfile()
    {
        switch( picturePickerTarget )
        {
            case PicturePickerTarget.DRIVER:
                DrawPictures( Data.Profile.drivers);
                break;

            case PicturePickerTarget.PASSENGER:
                DrawPictures( Data.Profile.passengers );
                break;

            case PicturePickerTarget.STATION_SYMBOL:
                {
                    List<Texture2D> textures = new List<Texture2D>();
                    List<StationSymbol> symbols = Data.Profile.symbols;
                    for( int i = 0; i < symbols.Count; i++ )
                        if( symbols[i].texture )
                            textures.Add( symbols[i].texture );
                    DrawPictures( textures );
                    break;
                }
            default: break;
        }
    }

    public void TakePictureOnClick( int maxSize = -1 )
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture( ( path ) =>
        {
            if( path != null )
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath( path, 200, false );
                if( texture == null )
                {
                    Debug.Log( "Couldn't load texture from " + path );
                    return;
                }

                //NativeGallery.SaveImageToGallery(texture.EncodeToJPG(), "TrainTrain", "{0}.png", null /*fuck error handling, what can go wrong?*//*);
                HandlePictureAddition(new List<Texture2D> { texture } );
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
            textures.Add( NativeGallery.LoadImageAtPath( paths[i], 200, false) );
        HandlePictureAddition( textures );
    }

    private void HandlePictureAddition(List<Texture2D> textures)
    {

        DrawPictures(textures);
        AddToProfile(textures);
    }

    private void DrawPictures(List<Texture2D> textures)
    {
        for( int i = 0; i < textures.Count; i++ )
        {
            Rect rect = new Rect(0, 0, textures[i].width, textures[i].height);
            Sprite sprite = Sprite.Create(textures[i], rect, new Vector2(0.5f, 0.5f));
            Image image = ((GameObject)Instantiate(imageTemplate, transform)).GetComponent<Image>();
            image.preserveAspect = true;
            image.sprite = sprite;
        }
    }
}
