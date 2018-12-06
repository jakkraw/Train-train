using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;                                                        // The System.IO namespace contains functions related to loading and saving files
using System;

public class Data : MonoBehaviour
{
    private string gameDataProjectFilePath = "/StreamingAssets/profileA.json";
    private enum Points { Disable, Plus, PlusMinus };

    [Serializable]
    private class Profile
    {
        public float trainSpeed;
        public Points points;
        public bool doesEnd;
    }

    public float Get_trainSpeed()
    {
        Profile profile = LoadProfileData();
        return profile.trainSpeed;
    }

    private Profile LoadProfileData()
    {
        string filePath = Application.dataPath + gameDataProjectFilePath;
        Profile profile;

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            profile = JsonUtility.FromJson<Profile>(dataAsJson);
        }
        else
        {
            profile = new Profile
            {
                trainSpeed = 0,
                points = Points.Disable,
                doesEnd = false
            };
        }
        return profile;
    }

}  