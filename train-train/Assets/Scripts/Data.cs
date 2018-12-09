using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;                                                   
using System;
using System.Collections.Generic;

public class Symbol_
{
    public string text = "";
    public Texture2D texture = null;

    public Symbol_(string t)
    {
        text = t;
    }

    public Symbol_(Texture2D t)
    {
        texture = t;
    }
}


//Klasa, która zawiera informacje zapisywane - tutaj jest miejsce na wszelkie ustawienia
[Serializable]
public class Profile
{
    public Texture2D driver;
    public List<Symbol_> symbols;
    public List<Texture2D> passengers;
    public float trainSpeed;
    public string points;
    public bool doesEnd;


    public static Profile testProfile()
    {
        var p = new Profile();
        p.passengers = new List<Texture2D>();
        var strings = new List<string>() { "Images/Bee", "Images/Monkey", "Images/Mouse" };
        foreach (var img in strings)
        {
            p.passengers.Add(Resources.Load<Texture2D>(img));
        }

        p.trainSpeed = 20;
        p.doesEnd = true;

        p.symbols = new List<Symbol_>();
        for (int i = 1; i <= 7; i++)
        {
            p.symbols.Add(new Symbol_(i.ToString()));
        }

        p.driver = Resources.Load<Texture2D>("Images/man");

        return p;
    }
}


public class Data
{
    static Data()
    {
        currentProfile = Profile.testProfile();
    }    


    public static string choosenProfile;
    public static Profile currentProfile;
    private readonly string gameDataProjectFilePath = "/StreamingAssets/";



    //Ładowanie danych z zapisanego pliku - do reimplementacji zgodnie z rozmową z 7.12.2018
    private Profile LoadProfileData()
    {
        if (!choosenProfile.Contains("profile")) choosenProfile = "profileA.json"; //W wypadku, gdy nie ma wybranego profilu, domyslny to profileA
        string filePath = Application.dataPath + gameDataProjectFilePath + choosenProfile;
        Profile profile;

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            currentProfile = JsonUtility.FromJson<Profile>(dataAsJson);
        }
        else
        {
            profile = new Profile
            {
                trainSpeed = 0,
                points = "Disable",
                doesEnd = false
            };
        }
        return currentProfile;
    }


    //Zapisywanie do pliku - do reimplementacji zgodnie z rozmową z 7.12.2018
    private void SaveProfileData()
    {

        string dataAsJson = JsonUtility.ToJson(currentProfile);

        string filePath = Application.dataPath + gameDataProjectFilePath + choosenProfile;
        File.WriteAllText(filePath, dataAsJson);

    }
}