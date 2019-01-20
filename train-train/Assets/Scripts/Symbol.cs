using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
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

[Serializable]
public class SymbolMapping
{
    Symbol matcher;
    List<Selectable<Symbol> > matches = new List<Selectable<Symbol> >();

    public SymbolMapping(Texture2D t) {
        var symbol = new Selectable<Symbol>( new Symbol(t), false );
        this.matcher = symbol.value;
        matches.Add(symbol);
    }

    public SymbolMapping(Symbol symbol, List<Selectable<Symbol> > symbols) {
        this.matcher = symbol;
        matches = symbols;
    }

    public SymbolMapping( Symbol symbol, List<Symbol> symbols )
    {
        List<Selectable<Symbol> > tmp = new List<Selectable<Symbol> >();
        symbols.ForEach( i => tmp.Add( new Selectable<Symbol>( i, false ) ) );
        this.matcher = symbol;
        matches = tmp;
    }

    public SymbolMapping(int i) {
        var symbol = new Selectable<Symbol>( new Symbol(i.ToString()), false);
        this.matcher = symbol.value;
        matches.Add(symbol);
    }

    public SymbolMapping(string s) {
        var symbol = new Selectable<Symbol>( new Symbol(s), false );
        this.matcher = symbol.value;
        matches.Add(symbol);
    }

    public SymbolMapping(string s, bool b) {
        var symbol = new Selectable<Symbol>( new Symbol(s), b );
        this.matcher = symbol.value;
        matches.Add(symbol);
    }

    public Symbol stationSymbol() {
        return matcher;
    }
    
    public Symbol randomMatching() {
        return matches[UnityEngine.Random.Range(0, matches.Count)].value;
    }

    public List<Selectable<Symbol> > passengerSymbols() {
        return matches;
    }

    public bool doesMatch(Symbol symbol) {
        return matches.Any(s => symbol.IsEqual(s.value));
    }

    private List<Symbol> selected() {
        return matches.FindAll( st => st.selected ).Select( a => a.value ).ToList();
    }

    public bool IsSelectedEnough()
    {
        return selected().Count > 0;
    }

    public int NumberOfSelected()
    {
        return selected().Count;
    }

    private Selectable<Symbol> find(Symbol symbol ) {
        return matches.Find(p => p.value == symbol );
    }

    public void select(Symbol symbol) {
        var found = find(symbol);
        found.selected = true;
    }

    public void deselect(Symbol symbol) {
        var found = find(symbol);
        found.selected = false;
    }

    public bool isSelected(Symbol symbol) {
        var found = find(symbol);
        return found.selected;
    }

    public void remove(Symbol symbol) {
        var found = find(symbol);
        matches.Remove(found);
    }
}
