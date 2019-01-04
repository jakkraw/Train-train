using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class Settings : MonoBehaviour
{
    public Slider trainSpeedSlider;
    public Toggle doesGameEndToogle;     public Toggle limitPassengers;
    public Toggle allowScore;
    public TMP_Dropdown symbolType;

    public void onBackClick()
    {
        Data.save();
        SceneManager.LoadScene("Menu");
    }

    public void onPassengerPicturePickClick()
    {
        PicturePicker.picturePickerTarget = PicturePickerTarget.PASSENGER;
        SceneManager.LoadScene( "PicturePicker" );
    }

    public void onDriverPicturePickClick()
    {
        PicturePicker.picturePickerTarget = PicturePickerTarget.DRIVER;
        SceneManager.LoadScene( "PicturePicker" );
    }

    public void onSymbolPicturePickClick()
    {
        GameObject popup = transform.Find( "SelectStationSymbolPopUp" ).gameObject;
        EnableSymbolSelectionEditFields();
        popup.SetActive( true );
    }

    public void onSymbolPicturePickExitClick()
    {
        onSymbolRangeDeselected();
        GameObject popup = transform.Find( "SelectStationSymbolPopUp" ).gameObject;
        popup.SetActive( false );
    }

    public void onChangeSymbolPictureClick()
    {
        PicturePicker.picturePickerTarget = PicturePickerTarget.STATION_SYMBOL;
        onSymbolPicturePickExitClick();
        SceneManager.LoadScene( "PicturePicker" );
    }

    public void Slider_Changed(float newValue)
    {
        Data.Profile.trainSpeed = newValue;
    }

    private void EnableSymbolSelectionEditFields()
    {
        GameObject popup = transform.Find( "SelectStationSymbolPopUp" ).gameObject;
        GameObject changeSelectionButton = popup.transform.Find( "ChangeSelectedPicturesButton" ).gameObject;
        GameObject rangeSelection = popup.transform.Find( "SymbolRange" ).gameObject;
        changeSelectionButton.SetActive( false );
        rangeSelection.SetActive( false );

        switch( Data.Profile.symboltypeindex )
        {
            case 0:
                changeSelectionButton.SetActive( true );
                break;
            case 1:{
                    TMP_InputField inputField;
                    inputField = rangeSelection.transform.Find( "InputFrom" ).gameObject.GetComponent<TMP_InputField>();
                    inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    inputField.characterLimit = 1;
                    inputField.text = Data.Profile.firstSymbolOfRange[0];

                    inputField = rangeSelection.transform.Find( "InputTo" ).gameObject.GetComponent<TMP_InputField>();
                    inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    inputField.characterLimit = 1;
                    inputField.text = Data.Profile.lastSymbolOfRange[0];

                    rangeSelection.SetActive( true );
                }
                break;
            case 2:{
                    TMP_InputField inputField;
                    inputField = rangeSelection.transform.Find( "InputFrom" ).gameObject.GetComponent<TMP_InputField>();
                    inputField.contentType = TMP_InputField.ContentType.Alphanumeric;
                    inputField.text = Data.Profile.firstSymbolOfRange[1];
                    inputField.characterLimit = 1;

                    inputField = rangeSelection.transform.Find( "InputTo" ).gameObject.GetComponent<TMP_InputField>();
                    inputField.contentType = TMP_InputField.ContentType.Alphanumeric;
                    inputField.text = Data.Profile.lastSymbolOfRange[1];
                    inputField.characterLimit = 1;

                    rangeSelection.SetActive( true );
                }
                break;
        }
    }

    public void Symbol_type_selected(System.Int32 newValue)
    {
        if( Data.Profile.symboltypeindex == 0 )
            Data.Profile.selectedSymbolsWA = Data.Profile.selectedSymbols;
        Data.Profile.symboltypeindex = newValue;
        EnableSymbolSelectionEditFields();
        onSymbolRangeDeselected();
        if( Data.Profile.symboltypeindex == 0 )
            Data.Profile.selectedSymbols = Data.Profile.selectedSymbolsWA;
    }

    public void onSymbolRangeDeselected()
    {
        GameObject popup = transform.Find( "SelectStationSymbolPopUp" ).gameObject;
        GameObject rangeSelection = popup.transform.Find( "SymbolRange" ).gameObject;
        TMP_InputField inputFieldFrom = rangeSelection.transform.Find( "InputFrom" ).gameObject.GetComponent<TMP_InputField>();
        TMP_InputField inputFieldTo = rangeSelection.transform.Find( "InputTo" ).gameObject.GetComponent<TMP_InputField>();

        var symbols = new List<StationSymbol>();
        switch( Data.Profile.symboltypeindex )
        {
            case 0:
                symbols.AddRange( Data.Profile.selectedSymbols );
                break;
            case 1:
                int i, e;
                int.TryParse(inputFieldFrom.text, out i);
                int.TryParse(inputFieldTo.text, out e);
                for(; i <= e; i++ )
                {
                    symbols.Add( new StationSymbol( i.ToString() ) );
                }
                Data.Profile.firstSymbolOfRange[0] = inputFieldFrom.text;
                Data.Profile.lastSymbolOfRange[0] = inputFieldTo.text;
                break;

            case 2:
                for( char c = inputFieldFrom.text[0]; c <= inputFieldTo.text[0]; c++ )
                {
                    symbols.Add( new StationSymbol( c.ToString() ) );
                }
                Data.Profile.firstSymbolOfRange[1] = inputFieldFrom.text;
                Data.Profile.lastSymbolOfRange[1] = inputFieldTo.text;
                break;
        }
        Data.Profile.selectedSymbols = symbols;
    }

    public void OnToggleDoesGameEndClick(bool newValue)
    {
        Data.Profile.doesEnd = newValue;
    }

    public void OnToggleAllowScoreClick(bool newValue)
    {
        Data.Profile.allowScore = newValue;
    }

    public void OnToggleLimitPassengersClick(bool newValue)
    {
        Data.Profile.limitPassengers = newValue;
    }

    void Start()
    {
        allowScore.isOn = Data.Profile.allowScore;
        doesGameEndToogle.isOn = Data.Profile.doesEnd;
        limitPassengers.isOn = Data.Profile.limitPassengers;         symbolType.value = Data.Profile.symboltypeindex;
        trainSpeedSlider.value = Data.Profile.trainSpeed;     }
}
