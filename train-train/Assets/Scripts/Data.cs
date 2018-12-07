using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;                                                   
using System;

public class Data : MonoBehaviour
{

    //Klasa, która zawiera informacje zapisywane - tutaj jest miejsce na wszelkie ustawienia
    [Serializable]
    public class Profile
    {
        public float trainSpeed;
        public string points;
        public bool doesEnd;
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