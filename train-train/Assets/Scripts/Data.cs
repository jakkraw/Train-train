using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
    //Texture2D Nie może być serializowane
    [NonSerialized]
    public List<Texture2D> drivers;
    [NonSerialized]
    public Texture2D selectedDriver;
    [NonSerialized]
    public List<Texture2D> passengers;
    [NonSerialized]
    public List<Texture2D> selectedPassengers;
    [NonSerialized]
    public List<Symbol_> symbols;
    [NonSerialized]
    public List<Symbol_> selectedSymbols;

    public int symboltypeindex = 0;
    public string driver_string = "Images/man";
    public List<String> passengers_strings = new List<string>() { "Images/Bee", "Images/Monkey", "Images/Mouse" };
    public List<String> symbols_strings = new List<string>() { "Images/carrot", "Images/cherries", "Images/grapes", "Images/watermelon", "Images/raspberry" };
    public float trainSpeed = 25;
    public string points;
    public bool doesEnd = true;     public bool changedToLetters;
    public bool limitPassengers = true;
    public bool allowScore = true;

    //Needs to be run 
    public void ReconstructProfile()
    {
        this.passengers = new List<Texture2D>();
        foreach (var img in this.passengers_strings)
        {
            this.passengers.Add(Resources.Load<Texture2D>(img));
        }
        this.selectedPassengers = new List<Texture2D>();
        this.selectedPassengers.AddRange( this.passengers );

        this.drivers = new List<Texture2D>();
        this.drivers.Add( Resources.Load<Texture2D>(this.driver_string) );
        this.drivers.Add(Resources.Load<Texture2D>("Images/happy_face 1"));
        this.selectedDriver = drivers[0];

        this.symbols = new List<Symbol_>();
        for (int i = 1; i <= 7; i++)
        {
            this.symbols.Add(new Symbol_(i.ToString()));
        }

        this.symbols = new List<Symbol_>();
        foreach (var img in this.symbols_strings)
        {
            this.symbols.Add(new Symbol_(Resources.Load<Texture2D>(img)));
        }

        this.selectedSymbols = new List<Symbol_>();
        this.selectedSymbols.AddRange( symbols );
    }

    public static Profile testProfile()
    {
        var p = new Profile();
        p.passengers_strings = new List<string>() { "Images/Bee", "Images/Monkey", "Images/Mouse" };
        p.driver_string = "Images/man";
        p.symbols_strings = new List<string>() { "Images/carrot", "Images/cherries", "Images/grapes", "Images/watermelon", "Images/raspberry" };
        p.ReconstructProfile();
        return p;
    }
}

//[Serializable]
//public class Textures
//{
    

//    public Textures(Texture2D driver_toSave, List<Symbol> symbols_toSave, List<Texture2D> passengers_toSave)
//    {
//        //Tutaj wrzuce konwersje parametrow wejsciowych na zmienne zadeklarowane wczesniej zeby mozna bylo to serializowac
//    }
//}

//Dopisz opcje - zmiany - trainspeed - doesnend - symbols - w settings zrobic 


//Klasa zapisywalna do pliku - lista profili oraz informacja o obecnym profilu
[Serializable]
public class ProfileList
{
    public List<Profile> profiles = new List<Profile>();
    //public Textures textures; //Jakas inicjalizacja domyslna??
    //                          //currentProfile - funkcja, ktora sptawdza index z ProfileList i wtedy ona wykorzystuje informacje z opdowiedniego profilu

    //public void NewProfiles()
    //{
    //}

    //public ProfileList()
    //{
    //    profiles = new List<Profile>
    //    {
    //        Profile.testProfile(),
    //        null
    //    };
    //}
}


public class Data
{
    static bool didInit = false;

    public static void init()
    {
        if (didInit) return;
        didInit = true;

        //ścieżka pliku z profilami //tymczasowo tutaj, potem do zmiany

        if (File.Exists(destination))
        {
            FileStream file = File.OpenRead(destination);
            ProfileList dataFromFile = (ProfileList)bf.Deserialize(file);
            file.Close();
            All_Profiles = new ProfileList
            {
                profiles = dataFromFile.profiles
            };
            currentProfile = All_Profiles.profiles[0];
            currentProfile.ReconstructProfile();
            Debug.Log("Profile file was loaded.");
        }
        else
        {
            FileStream file = File.Create(destination);
            var p_list = new ProfileList();
            p_list.profiles.Insert(0, Profile.testProfile());
            bf.Serialize(file, p_list);
            file.Close();
            Debug.Log("Profile file was created.");
        }
    }

    public void save()
    {
        if (File.Exists(destination))
        {
            FileStream file = File.OpenWrite(destination);
            var p_list = new ProfileList();
            p_list.profiles.Insert(0, currentProfile);
            bf.Serialize(file, p_list);
            file.Close();
            Debug.Log("Profile file was saved.");
        }
        else
        {
            FileStream file = File.Create(destination);
            var p_list = new ProfileList();
            p_list.profiles.Insert(0, currentProfile);
            bf.Serialize(file, p_list);
            file.Close();
            Debug.Log("Profile file was created while exitting settings.");
        }
    }

    public static Profile _currentProfile;
    public static ProfileList _All_Profiles;


    public static BinaryFormatter bf = new BinaryFormatter();
    public static string destination = Application.dataPath + "/profiles.bin";
    public static Profile currentProfile { get { init(); return _currentProfile; } set { _currentProfile = value; } }
    public static ProfileList All_Profiles { get { init(); return _All_Profiles; } set { _All_Profiles = value; } }
    

}