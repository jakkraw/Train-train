using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class Symbol
{
    string _text = null;
    Texture2D _texture = null;
    public string text { get { return _text; } set { _text = value; _texture = null; } }
    public Texture2D texture { get { return _texture; } set { _texture = value; _text = null; } }

    public Symbol(string t)
    {
        text = t;
    }

    public Symbol(Texture2D t)
    {
        texture = t;
    }

    public bool IsEqual(Symbol symbol)
    {
        if(text == symbol.text && texture == symbol.texture) { return true; }
        return false;
    }

    public bool IsEqual(Texture2D texture)
    {
        if (this.texture == texture) { return true; }
        return false;
    }

    public bool IsEqual(string text)
    {
        if (this.text == text) { return true; }
        return false;
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
    public List<Symbol> symbols;
    [NonSerialized]
    public List<Symbol> selectedSymbols;

    // This is needed for persistance of selected symbols, first element is for digits, second for characters
    // StationSymbol is poorly designed and needs refactor, but there is no time for that now
    public string[] firstSymbolOfRange;
    public string[] lastSymbolOfRange;
    //temporary WA for persistance of selected symbols; same reason as above
    [NonSerialized]
    public List<Symbol> selectedSymbolsWA;

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
            symbols_info.Add(new TextureInfo(selectedSymbols.Exists(selected => selected.IsEqual(symbol)), symbol.texture));

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

        symbols = new List<Symbol>();
        selectedSymbols = new List<Symbol>();
        foreach (var info in symbols_info)
        {
            var symbol = new Symbol(info.construct());
            symbols.Add(symbol);
            if (info.isSelected) { selectedSymbols.Add(symbol); }
        }

        firstSymbolOfRange = new string[2];
        firstSymbolOfRange[0] = "0";
        firstSymbolOfRange[1] = "a";
        lastSymbolOfRange = new string[2];
        lastSymbolOfRange[0] = "9";
        lastSymbolOfRange[1] = "z";
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
    
    public static void reset()
    {
        All_Profiles = new ProfileList(Profile.testProfile());
        Profile.ReconstructProfile();
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
    public static string destination = Application.persistentDataPath + "/profiles2.bin";
    public static Profile Profile { get { return All_Profiles.currentProfile(); } }
    

}