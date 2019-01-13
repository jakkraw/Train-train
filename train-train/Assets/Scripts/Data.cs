using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public enum SymbolType {
    SimpleTextures,
    NumberRange,
    Letters,
    MultiplyTest
};

[Serializable]
public class SelectableSTexture2D {
    public bool selected = false;
    public STexture2D texture;

    public SelectableSTexture2D(STexture2D t, bool s) {
        texture = t;
        selected = s;
    }
}

[Serializable]
public class Passengers {
    private List<SelectableSTexture2D> passengers = new List<SelectableSTexture2D>();

    public void add(Texture2D t) {
        var selectable = new SelectableSTexture2D(new STexture2D(t), false);
        passengers.Add(selectable);
    }

    public List<Texture2D> selected() {
        return passengers.FindAll(st => st.selected).Select(a => a.texture.Texture).ToList();
    }

    public List<Texture2D> all() {
        return passengers.Select(a => a.texture.Texture).ToList();
    }

    private SelectableSTexture2D find(Texture2D texture) {
        return passengers.Find(p => p.texture.Texture == texture);
    }

    public void select(Texture2D texture) {
        var found = find(texture);
        found.selected = true;
    }

    public void deselect(Texture2D texture) {
        var found = find(texture);
        found.selected = false;
    }

    public bool isSelected(Texture2D texture) {
        var found = find(texture);
        return found.selected;
    }

    public void remove(Texture2D texture) {
        var found = find(texture);
        found.texture.delete();
        passengers.Remove(found);
    }
}

[Serializable]
public class TextureSymbols {
    private List<SelectableSTexture2D> textureSymbols = new List<SelectableSTexture2D>();

    public void add(Texture2D t) {
        var selectable = new SelectableSTexture2D(new STexture2D(t), false);
        textureSymbols.Add(selectable);
    }

    public List<Texture2D> selected() {
        return textureSymbols.FindAll(st => st.selected).Select(a => a.texture.Texture).ToList();
    }

    public List<Texture2D> all() {
        return textureSymbols.Select(a => a.texture.Texture).ToList();
    }

    private SelectableSTexture2D find(Texture2D texture) {
        return textureSymbols.Find(p => p.texture.Texture == texture);
    }

    public void select(Texture2D texture) {
        var found = find(texture);
        found.selected = true;
    }

    public void deselect(Texture2D texture) {
        var found = find(texture);
        found.selected = false;
    }

    public bool isSelected(Texture2D texture) {
        var found = find(texture);
        return found.selected;
    }

    public void remove(Texture2D texture) {
        var found = find(texture);
        found.texture.delete();
        textureSymbols.Remove(found);
    }
}

[Serializable]
public class Drivers {
    private List<SelectableSTexture2D> drivers = new List<SelectableSTexture2D>();

    public void add(Texture2D t) {
        var selectable = new SelectableSTexture2D(new STexture2D(t), false);
        drivers.Add(selectable);
    }

    private SelectableSTexture2D find(Texture2D texture) {
        return drivers.Find(p => p.texture.Texture == texture);
    }

    public void select(Texture2D t) {
        foreach (var driver in drivers) {
            driver.selected = false;
        }

        var found = find(t);
        found.selected = true;
    }

    public bool isSelected(Texture2D texture) {
        var found = find(texture);
        return found.selected;
    }

    public void remove(Texture2D t) {
        var found = find(t);
        found.texture.delete();
        drivers.Remove(found);
    }

    public List<Texture2D> all() {
        return drivers.Select(a => a.texture.Texture).ToList();
    }

    public Texture2D selected() {
        var l = drivers.Find(st => st.selected);
        if (l == null) { return null; }
        return l.texture;
    }

    public void deselect(Texture2D texture) {
        var found = find(texture);
        found.selected = false;
    }
}

[Serializable]
public class STexture2D {
    [NonSerialized]
    private Texture2D _texture = null;

    private string path = null;

    public void delete() {
        File.Delete(path);
    }

    public static string generateID() {
        return Guid.NewGuid().ToString("N");
    }

    public Texture2D Texture {
        get {
            if (_texture == null) {
                var file = File.OpenRead(path);
                _texture = new Texture2D(0, 0);
                _texture.LoadImage((byte[])new BinaryFormatter().Deserialize(file));
                file.Close();
            }

            return _texture;
        }

        set {
            _texture = value;
            if(path == null) path = Application.persistentDataPath + "/" + generateID() + ".png";
            var file = File.Open(path, FileMode.Create);
            new BinaryFormatter().Serialize(file, _texture.EncodeToPNG());
            file.Close();
        }
    }

    public STexture2D(Texture2D texture) {
        Texture = texture;
    }

    public static implicit operator Texture2D(STexture2D st) {
        return st.Texture;
    }

}

[Serializable]
public class NumberRange {
    public int begin;
    public int end;
}

[Serializable]
public class Letters {
    public List<char> list = new List<char>();
}

[Serializable]
public class Profile {
    public float trainSpeed = 25;
    public bool doesEnd = true;
    public bool limitPassengers = true;
    public bool allowScore = true;

    public Drivers drivers = new Drivers();
    public Passengers passengers = new Passengers();

    public SymbolType symbolType = SymbolType.SimpleTextures;
    public TextureSymbols textureSymbols;
    public NumberRange numberRange;
    public Letters letters;

    public List<SymbolMapping> Symbols {
        get {
            switch (symbolType) {
                case SymbolType.SimpleTextures:
                    return textureSymbols.selected().Select(t => new SymbolMapping(t)).ToList();
                case SymbolType.NumberRange:
                    var numberMappings = new List<SymbolMapping>();
                    for(var i = numberRange.begin; i <= numberRange.end; i++) {
                        numberMappings.Add(new SymbolMapping(i));
                    }
                    return numberMappings;
                case SymbolType.Letters:
                    var letterMappings = new List<SymbolMapping>();
                    foreach(var letter in letters.list) {
                        letterMappings.Add(new SymbolMapping(letter.ToString()));
                    }
                    return letterMappings;
                case SymbolType.MultiplyTest:
                    return null;
            }
            return null;
        } 
    }

    public static Profile testProfile() {

        var passengers = new Passengers();
        foreach (var path in new List<string>() { "Images/Bee", "Images/Monkey", "Images/Mouse" }) {
            var texture = Resources.Load<Texture2D>(path);
            passengers.add(texture);
            passengers.select(texture);
        }

        foreach (var path in new List<string>() { "Images/businessman", "Images/doctor", "Images/girl", "Images/girl2", "Images/girl3", "Images/man2", "Images/student", "Images/woman", }) {
            var texture = Resources.Load<Texture2D>(path);
            passengers.add(texture);
        }

        var drivers = new Drivers();
        var driver1 = Resources.Load<Texture2D>("Images/girl3");
        var driver2 = Resources.Load<Texture2D>("Images/driver");
        drivers.add(driver1);
        drivers.add(driver2);
        drivers.select(driver2);

        var textureSymbols = new TextureSymbols();
        foreach (var path in new List<string>() { "Images/carrot", "Images/cherries", "Images/grapes", "Images/watermelon", "Images/raspberry" }) {
            textureSymbols.add(Resources.Load<Texture2D>(path));
        }

        foreach (var path in new List<string>() { "Images/gamepad", "Images/pyramid", "Images/rocket", "Images/skateboard", "Images/spinner", "Images/gift", }) {
            textureSymbols.add(Resources.Load<Texture2D>(path));
        }

        foreach (var texture in textureSymbols.all()) {
            textureSymbols.select(texture);
        }

        var numberRange = new NumberRange {
            begin = 1,
            end = 10
        };

        var letters = new Letters {
            list = new List<char>() { 'a', 'b', 'c' }
        };

        return new Profile {
            drivers = drivers,
            passengers = passengers,
            symbolType = SymbolType.Letters,
            textureSymbols = textureSymbols,
            numberRange = numberRange,
            letters = letters
        };
    }
}


public static class Data {
    static Data() {
        load();
    }

    public static void load() {

        if (File.Exists(destination)) {
            var file = File.OpenRead(destination);
            Profile = (Profile)new BinaryFormatter().Deserialize(file);
            file.Close();
        } else {
            reset();
        }

        Profile.drivers.all();
        Profile.passengers.all();
        Profile.textureSymbols.all();

        Debug.Log("Profile file was loaded.");
    }

    public static void reset() {
        Profile = Profile.testProfile();
    }

    public static void save() {
        var file = File.Open(destination, FileMode.Create);
        new BinaryFormatter().Serialize(file, Profile);
        file.Close();
        Debug.Log("Profile file was saved.");
    }

    public static Profile Profile;
    public static string destination = Application.persistentDataPath + "/profiles12.bin";


}