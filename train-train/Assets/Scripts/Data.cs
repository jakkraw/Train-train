using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class StationSymbol
{
    public string text = "";
    public Texture2D texture = null;

    public StationSymbol(string t)
    {
        text = t;
    }

    public StationSymbol(Texture2D t)
    {
        texture = t;
    }
}

[Serializable]
public class Profile
{
    [Serializable]
    public class TextureInfo
    {

        public TextureInfo(bool isSelected, Texture2D tex)
        {
            this.isSelected = isSelected;
            this.bytes = tex.EncodeToPNG();
        }

        public Texture2D construct()
        {
            var tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            return tex;
        }

        public bool isSelected;
        public byte[] bytes;

    }

    [NonSerialized]
    public List<Texture2D> drivers;
    [NonSerialized]
    public Texture2D selectedDriver;
    [NonSerialized]
    public List<Texture2D> passengers;
    [NonSerialized]
    public List<Texture2D> selectedPassengers;
    [NonSerialized]
    public List<StationSymbol> symbols;
    [NonSerialized]
    public List<StationSymbol> selectedSymbols;

    [NonSerialized]
    public bool reconstructed = false;

    public int symboltypeindex = 0;
    public float trainSpeed = 25;
    public bool doesEnd = true;
    public bool limitPassengers = true;
    public bool allowScore = true;

    public List<TextureInfo> passengers_info = new List<TextureInfo>();
    public List<TextureInfo> drivers_info = new List<TextureInfo>();
    public List<TextureInfo> symbols_info = new List<TextureInfo>();


    public void PackProfile()
    {
        passengers_info.Clear();
        foreach (var tex in passengers)
            passengers_info.Add(new TextureInfo(selectedPassengers.Contains(tex), tex));

        symbols_info.Clear();
        foreach (var symbol in symbols)
            symbols_info.Add(new TextureInfo(selectedSymbols.Exists(s => s.texture == symbol.texture), symbol.texture));

        drivers_info.Clear();
        foreach (var tex in drivers)
            drivers_info.Add(new TextureInfo(selectedDriver == tex, tex));
    }

    public void ReconstructProfile()
    {
        if(reconstructed) { return; }
        reconstructed = true;

        this.passengers = new List<Texture2D>();
        this.selectedPassengers = new List<Texture2D>();
        foreach (var info in passengers_info)
        {
            var img = info.construct();
            passengers.Add(img);
            if (info.isSelected) { selectedPassengers.Add(img); }
        }

        drivers = new List<Texture2D>();
        selectedDriver = null;
        foreach (var info in drivers_info)
        {
            var img = info.construct();
            drivers.Add(img);
            if (info.isSelected) { selectedDriver = img; }
        }

        symbols = new List<StationSymbol>();
        selectedSymbols = new List<StationSymbol>();
        foreach (var info in symbols_info)
        {
            var symbol = new StationSymbol(info.construct());
            symbols.Add(symbol);
            if (info.isSelected) { selectedSymbols.Add(symbol); }
        }

    }

    public static Profile testProfile()
    {
        var p = new Profile();

        foreach(var path in new List<string>() { "Images/Bee", "Images/Monkey", "Images/Mouse" }) {
           p.passengers_info.Add(new TextureInfo(true, Resources.Load<Texture2D>(path))); 
        }

        foreach (var path in new List<string>() { "Images/businessman", "Images/doctor", "Images/girl", "Images/girl2", "Images/girl3", "Images/man2", "Images/student", "Images/woman", })
        {
            p.passengers_info.Add(new TextureInfo(false, Resources.Load<Texture2D>(path)));
        }

        foreach (var path in new List<string>() { "Images/girl3", "Images/driver" })
        {
            p.drivers_info.Add(new TextureInfo(false, Resources.Load<Texture2D>(path)));
        }

        p.drivers_info[0].isSelected = true;

        foreach (var path in new List<string>() { "Images/carrot", "Images/cherries", "Images/grapes", "Images/watermelon", "Images/raspberry" })
        {
            p.symbols_info.Add(new TextureInfo(true, Resources.Load<Texture2D>(path)));
        }

        foreach (var path in new List<string>() { "Images/gamepad", "Images/pyramid", "Images/rocket", "Images/skateboard", "Images/spinner", "Images/gift", })
        {
            p.symbols_info.Add(new TextureInfo(false, Resources.Load<Texture2D>(path)));
        }

        return p;
    }
}

[Serializable]
public class ProfileList
{
    public List<Profile> profiles = new List<Profile>();
    public int index = -1;

    public ProfileList(Profile profile)
    {
        index = 0;
        profiles.Add(profile);
        
    }

    public void addProfile(Profile profile) {
        profiles.Add(profile);
    }

    public Profile currentProfile() {
        var profile = profiles[index];
        profile.ReconstructProfile();
        return profile;
    }
}


public static class Data
{
    static Data()
    {
        load();
    }

    public static void load()
    {
        if (File.Exists(destination))
        {
           var file = File.OpenRead(destination);
           All_Profiles = (ProfileList)new BinaryFormatter().Deserialize(file);
            file.Close();
        }
        else
        {
            All_Profiles =  new ProfileList(Profile.testProfile());
        }
       
        Profile.ReconstructProfile();
        Debug.Log("Profile file was loaded.");
    }

    public static void save()
    {
        foreach (var profile in All_Profiles.profiles) profile.PackProfile();
        var file = File.Open(destination, FileMode.Create);
        new BinaryFormatter().Serialize(file, All_Profiles);
        file.Close();
        Debug.Log("Profile file was saved.");
    }

    public static ProfileList All_Profiles;
    public static string destination = Application.persistentDataPath + "/profiles.bin";
    public static Profile Profile { get { return All_Profiles.currentProfile(); } }
    

}