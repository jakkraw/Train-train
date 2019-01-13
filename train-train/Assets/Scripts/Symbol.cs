using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Symbol
{
    string _text = null;
    STexture2D _texture = null;
    public string text { get { return _text; } set { _text = value; _texture = null; } }
    public STexture2D texture { get { return _texture; } set { _texture = value; _text = null; } }

    public Symbol(string t)
    {
        text = t;
    }

    public Symbol(Texture2D t)
    {
        texture = new STexture2D(t);
    }

    public bool IsEqual(Symbol symbol)
    {
        if (text == symbol.text && texture == symbol.texture) { return true; }
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


public class SymbolMapping
{
    Symbol matcher;
    List<Symbol> matches = new List<Symbol>();

    public SymbolMapping(Texture2D t) {
        var symbol = new Symbol(t);
        this.matcher = symbol;
        matches.Add(symbol);
    }

    public SymbolMapping(int i) {
        var symbol = new Symbol(i.ToString());
        this.matcher = symbol;
        matches.Add(symbol);
    }

    public SymbolMapping(string s) {
        var symbol = new Symbol(s);
        this.matcher = symbol;
        matches.Add(symbol);
    }

    public Symbol stationSymbol() {
        return matcher;
    }

    public List<Symbol> passengerSymbols() {
        return matches;
    }

    public bool doesMatch(Symbol symbol) {
        return matches.Any(s => symbol.IsEqual(s));
    }

}
