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


//Klasa zapisywalna do pliku - lista profili oraz informacja o obecnym profilu
[Serializable]
public class ProfileList
{
    public List<Profile> Profiles;
}

public class Data
{
    //static Data()
    //{
    //    currentProfile = Profile.testProfile();
    //}    

    static Data()
    {
        //ścieżka pliku z profilami
        currentProfile = Profile.testProfile(); //tymczasowo tutaj, potem do zmiany
        string destination = Application.dataPath + "/StreamingAssets/profiles.bin";
        //Debug.Log("Start!");
        BinaryFormatter bf = new BinaryFormatter();

        //if (File.Exists(destination))
        //{
        //    Debug.Log("Wczytano!");
        //    FileStream file = File.OpenRead(destination);
        //    ProfileList data = (ProfileList)bf.Deserialize(file);
        //    file.Close();
        //    currentProfile = data.Profiles[0];
        //}
        //else
        //{
        //    FileStream file = File.Create(destination);
        //    Debug.Log("Stworzony");
        //    All_Profiles.Profiles.Add(currentProfile);
        //    bf.Serialize(file, All_Profiles);
        //    file.Close();
        //}
    }

    public static Profile currentProfile;
    public static ProfileList All_Profiles;

}