using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class Settings : MonoBehaviour
{
    public Slider trainSpeedSlider;
    public Toggle doesGameEndToogle;
    public Toggle limitPassengers;
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
        PicturePicker.picturePickerTarget = PicturePickerTarget.TEXTURE_SYMBOl;
        onSymbolPicturePickExitClick();
        SceneManager.LoadScene( "PicturePicker" );
    }

    public void onResetProfileClick()
    {
        Data.reset();
        Start();
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

        switch( Data.Profile.symbolType )
        {
            case SymbolType.SimpleTextures:
                changeSelectionButton.SetActive( true );
                break;
            case SymbolType.NumberRange:{
                    TMP_InputField inputField;
                    inputField = rangeSelection.transform.Find( "InputFrom" ).gameObject.GetComponent<TMP_InputField>();
                    inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    inputField.characterLimit = 1;
                    inputField.text = Data.Profile.numberRange.begin.ToString();

                    inputField = rangeSelection.transform.Find( "InputTo" ).gameObject.GetComponent<TMP_InputField>();
                    inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    inputField.characterLimit = 1;
                    inputField.text = Data.Profile.numberRange.end.ToString();

                    rangeSelection.SetActive( true );
                }
                break;
            case SymbolType.Letters:{
                    TMP_InputField inputField;
                    inputField = rangeSelection.transform.Find( "InputFrom" ).gameObject.GetComponent<TMP_InputField>();
                    inputField.contentType = TMP_InputField.ContentType.Alphanumeric;
                    inputField.text = Data.Profile.letters.list[0].ToString();
                    inputField.characterLimit = 1;

                    inputField = rangeSelection.transform.Find( "InputTo" ).gameObject.GetComponent<TMP_InputField>();
                    inputField.contentType = TMP_InputField.ContentType.Alphanumeric;
                    inputField.text = Data.Profile.letters.list[Data.Profile.letters.list.Count - 1].ToString();
                    inputField.characterLimit = 1;

                    rangeSelection.SetActive( true );
                }
                break;
        }
    }

    public void Symbol_type_selected(System.Int32 newValue)
    {
        Data.Profile.symbolType = (SymbolType)newValue;
        EnableSymbolSelectionEditFields();
        onSymbolRangeDeselected();
    }

    public void onSymbolRangeDeselected()
    {
        GameObject popup = transform.Find("SelectStationSymbolPopUp").gameObject;
        GameObject rangeSelection = popup.transform.Find("SymbolRange").gameObject;
        TMP_InputField inputFieldFrom = rangeSelection.transform.Find("InputFrom").gameObject.GetComponent<TMP_InputField>();
        TMP_InputField inputFieldTo = rangeSelection.transform.Find("InputTo").gameObject.GetComponent<TMP_InputField>();

        var symbols = new List<Symbol>();
        switch (Data.Profile.symbolType) {
            case SymbolType.SimpleTextures:
                break;
            case SymbolType.NumberRange:
                int i, e;
                int.TryParse(inputFieldFrom.text, out i);
                int.TryParse(inputFieldTo.text, out e);
                Data.Profile.numberRange.begin = i;
                Data.Profile.numberRange.end = e;
                break;

            case SymbolType.Letters:
                var letters = new List<char>();
                for (char c = inputFieldFrom.text[0]; c <= inputFieldTo.text[0]; c++) {
                    letters.Add(c);
                }
                Data.Profile.letters.list = letters;
                break;
        }
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
        limitPassengers.isOn = Data.Profile.limitPassengers;
        symbolType.value = (int)Data.Profile.symbolType;
        trainSpeedSlider.value = Data.Profile.trainSpeed;
    }
}
