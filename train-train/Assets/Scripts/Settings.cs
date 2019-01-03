using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Settings : MonoBehaviour
{
    public Slider trainSpeedSlider;
    public Toggle doesGameEndToogle;     public Toggle limitPassengers;
    public Toggle allowScore;
    public TMPro.TMP_Dropdown symbolType;

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
        PicturePicker.picturePickerTarget = PicturePickerTarget.STATION_SYMBOL;
        SceneManager.LoadScene( "PicturePicker" );
    }

    public void Slider_Changed(float newValue)
    {
        Data.Profile.trainSpeed = newValue;
    }

    public void Symbol_type_selected(System.Int32 newValue)
    {
        Data.Profile.symboltypeindex = newValue;
        var symbols = new List<StationSymbol>();
        Data.Profile.selectedSymbols = symbols;
      
        switch (newValue)
        {
            case 0:
                symbols.AddRange(Data.Profile.selectedSymbols);
                break;
            case 1:
                for (int i = 1; i <= 7; i++) {
                   symbols.Add(new StationSymbol(i.ToString()));
                }
                break;

            case 2:
                symbols.Add(new StationSymbol("a"));
                symbols.Add(new StationSymbol("b"));
                symbols.Add(new StationSymbol("c"));
                symbols.Add(new StationSymbol("d"));
                symbols.Add(new StationSymbol("e"));
                symbols.Add(new StationSymbol("f"));
                symbols.Add(new StationSymbol("g"));
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
        limitPassengers.isOn = Data.Profile.limitPassengers;         symbolType.value = Data.Profile.symboltypeindex;
        trainSpeedSlider.value = Data.Profile.trainSpeed;     }
}
