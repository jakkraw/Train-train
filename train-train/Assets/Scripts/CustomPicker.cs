using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomPicker : MonoBehaviour {

    public static string PreviousScene = "";
    public static SymbolMapping pickable;
    public GameObject customTemplate;
    public TextMeshProUGUI selectedCounterTextBox;
    public TMP_InputField input;
    private bool isInDeleteMode = false;


    public static void Modify(SymbolMapping pickable ) {
        CustomPicker.pickable = pickable;
        CustomPicker.PreviousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("CustomPicker");
    }

    // Use this for initialization
    private void Start() {
        DrawCustoms(pickable.passengerSymbols());
    }

    public void DeletePictureOnClick(Image image) {
        isInDeleteMode = !isInDeleteMode;

        Color color = image.color;
        color.a = isInDeleteMode ? 0.4f : 0.0f;
        image.color = color;
    }

    public void BackOnClick() {
        if (!pickable.IsSelectedEnough()) { return; }
        Data.save();
        SceneManager.LoadScene(PreviousScene);
    }

    public bool isSelected(Symbol symbol ) {
        return pickable.isSelected(symbol);
    }

    public void HandleSelectRequest(Symbol symbol, GameObject gameObject) {
        if (isSelected(symbol)) 
            pickable.deselect(symbol);
        else 
            pickable.select(symbol);

        if (isInDeleteMode) {
            pickable.remove(symbol);
            Destroy(gameObject);
        }

        selectedCounterTextBox.text = pickable.NumberOfSelected().ToString();
    }

    public void TakePictureOnClick(int maxSize = -1) {
        NativeCamera.TakePicture((path) => {
            if (path == null)
                return;

            Texture2D texture = NativeCamera.LoadImageAtPath(path, 700, false);
            if(texture != null)
                HandlePictureAddition(new List<Texture2D> { texture });
        }, maxSize);
    }

    public void AddPicturesFromGalleryOnClick() {
        if( NativeGallery.IsMediaPickerBusy() )
            return;

        if (NativeGallery.CanSelectMultipleFilesFromGallery())
            NativeGallery.GetImagesFromGallery((paths) => HandlePictureAddition(paths), "Select pictures", "image/*");
        else
            NativeGallery.GetImageFromGallery((path) => HandlePictureAddition(new[] { path }), "Select picture", "image/*");
    }

    private void HandlePictureAddition(string[] paths) {
        List<Texture2D> textures = new List<Texture2D>();
        for (int i = 0; i < paths.Length; i++ ){
            Texture2D texture = NativeGallery.LoadImageAtPath(paths[i], 700, false);
            if( texture != null)
                textures.Add(texture);
        }
        HandlePictureAddition(textures);
    }

    private void HandlePictureAddition(List<Texture2D> textures) {
        List<Selectable<Symbol> > mapping = new List<Selectable<Symbol> >();
        textures.ForEach( t => mapping.Add( new Selectable<Symbol>( new Symbol(t), false ) ) );
        HandleSymbolAddition(mapping);
    }

    public void HandleTextSymbolAddition() {
        List<Selectable<Symbol> > mapping = new List<Selectable<Symbol> >();
        mapping.Add(new Selectable<Symbol>( new Symbol( input.text ), false ));
        HandleSymbolAddition(mapping);
    }

    private void HandleSymbolAddition( List<Selectable<Symbol>> mapping ) {
        mapping.ForEach( t => pickable.passengerSymbols().Add( t ) );
        DrawCustoms( mapping );
    }

    private void DrawCustoms(List<Selectable<Symbol> > symbols) {
        selectedCounterTextBox.text = pickable.NumberOfSelected().ToString();
        for (int i = 0; i < symbols.Count; i++) {
            Instantiate( customTemplate, transform ).GetComponent<CustomMapping>().setSymbol(symbols[i].value);
        }
    }
}
