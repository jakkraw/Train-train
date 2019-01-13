using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public enum PicturePickerTarget { 
    DRIVER,
    PASSENGER,
    TEXTURE_SYMBOl
}

public class PicturePicker : MonoBehaviour {

    public static PicturePickerTarget picturePickerTarget = PicturePickerTarget.PASSENGER;
    public GameObject imageTemplate;
    public GameObject selectedCounterTextBox;
    private bool isInDeleteMode = false;

    // Use this for initialization
    void Start() {
        switch (picturePickerTarget) {
            case PicturePickerTarget.DRIVER:
                DrawPictures(Data.Profile.drivers.all());
                break;

            case PicturePickerTarget.PASSENGER:
                DrawPictures(Data.Profile.passengers.all());
                break;

            case PicturePickerTarget.TEXTURE_SYMBOl:
                    DrawPictures(Data.Profile.textureSymbols.all());
                    break;
            default: break;
        }
    }

    public void DeletePictureOnClick(Image image) {
        isInDeleteMode = !isInDeleteMode;

        Color color = image.color;
        color.a = isInDeleteMode ? 0.4f : 0.0f;
        image.color = color;
    }

    public bool isDeleteModeActive() {
        return isInDeleteMode;
    }

    public void HandleDeleteRequest(Texture2D texture2D, bool isSelected) {
        switch (picturePickerTarget) {
            case PicturePickerTarget.DRIVER:
                Data.Profile.drivers.remove(texture2D);
                break;
            case PicturePickerTarget.PASSENGER:
                Data.Profile.passengers.remove(texture2D);
                break;
            case PicturePickerTarget.TEXTURE_SYMBOl:
                Data.Profile.textureSymbols.remove(texture2D);
                break;
        }
    }

    public void BackOnClick() {
        switch (picturePickerTarget) {
            case PicturePickerTarget.DRIVER:
                if (Data.Profile.drivers.selected() == null)
                    return;
                break;

            case PicturePickerTarget.PASSENGER:
                if (Data.Profile.passengers.selected().Count == 0)
                    return;
                break;

            case PicturePickerTarget.TEXTURE_SYMBOl:
                if (Data.Profile.textureSymbols.selected().Count == 0)
                    return;
                break;
        }
        SceneManager.LoadScene("Settings");
    }

    public void Update() {
        TextMeshProUGUI text = selectedCounterTextBox.GetComponent<TextMeshProUGUI>();
        var count = 0;
        switch (picturePickerTarget) {
            case PicturePickerTarget.DRIVER:
                count = Data.Profile.drivers.selected() == null ? 0 : 1;
                break;

            case PicturePickerTarget.PASSENGER:
                count = Data.Profile.passengers.selected().Count;
                break;

            case PicturePickerTarget.TEXTURE_SYMBOl:
                count = Data.Profile.textureSymbols.selected().Count;
                break;
        }
        text.text = count.ToString();
    }

    public bool isSelected(Texture2D texture) {
        switch (picturePickerTarget) {
            default:
            case PicturePickerTarget.DRIVER:
                return Data.Profile.drivers.isSelected(texture);

            case PicturePickerTarget.PASSENGER:
                return Data.Profile.passengers.isSelected(texture);

            case PicturePickerTarget.TEXTURE_SYMBOl:
                return Data.Profile.textureSymbols.isSelected(texture);
        }
    }

    public bool HandleSelectRequest(Texture2D texture) {
        if (isSelected(texture)) {
            unselect(texture);
            return false;
        } else {
            select(texture);
            return true;
        }
    }

    private void unselect(Texture2D texture) {
        switch (picturePickerTarget) {
            case PicturePickerTarget.DRIVER:
                //Data.Profile.drivers.deselect(texture);
                break;
            case PicturePickerTarget.PASSENGER:
                Data.Profile.passengers.deselect(texture);
                break;
            case PicturePickerTarget.TEXTURE_SYMBOl:
                Data.Profile.textureSymbols.deselect(texture);
                break;
            default: break;
        }
    }

    private void select(Texture2D texture) {
        switch (picturePickerTarget) {
            case PicturePickerTarget.DRIVER:
                Data.Profile.drivers.select(texture);
                break;
            case PicturePickerTarget.PASSENGER:
                Data.Profile.passengers.select(texture);
                break;
            case PicturePickerTarget.TEXTURE_SYMBOl:
                Data.Profile.textureSymbols.select(texture);
                break;
            default: break;
        }
    }

    private void AddToProfile(List<Texture2D> textures) {
        foreach(var texture in textures) {
            switch (picturePickerTarget) {
                case PicturePickerTarget.DRIVER:
                    Data.Profile.drivers.add(texture);
                    break;
                case PicturePickerTarget.PASSENGER:
                    Data.Profile.passengers.add(texture);
                    break;

                case PicturePickerTarget.TEXTURE_SYMBOl:
                    Data.Profile.textureSymbols.add(texture);
                    break;
                default: break;
            }
        }
    }

    public void TakePictureOnClick(int maxSize = -1) {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) => {
            if (path != null) {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, 128 * 128, false);
                if (texture == null) {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                //NativeGallery.SaveImageToGallery(texture.EncodeToJPG(), "TrainTrain", "{0}.png", null /*fuck error handling, what can go wrong?*//*);
                HandlePictureAddition(new List<Texture2D> { texture });
            }
        }, maxSize);
        //((GameObject) Instantiate( imageTemplate, transform )).GetComponent<Image>().color = Random.ColorHSV();
    }

    public void AddPicturesFromGalleryOnClick() {
        if (!NativeGallery.IsMediaPickerBusy()) {
            if (NativeGallery.CanSelectMultipleFilesFromGallery())
                NativeGallery.GetImagesFromGallery((paths) => HandlePictureAddition(paths), "Select pictures", "image/*");
            else
                NativeGallery.GetImageFromGallery((path) => HandlePictureAddition(new[] { path }), "Select picture", "image/*");
        }
    }

    private void HandlePictureAddition(string[] paths) {
        List<Texture2D> textures = new List<Texture2D>();
        for (int i = 0; i < paths.Length; i++)
            textures.Add(NativeGallery.LoadImageAtPath(paths[i], 128 * 128, false));
        HandlePictureAddition(textures);
    }

    private void HandlePictureAddition(List<Texture2D> textures) {
        DrawPictures(textures);
        AddToProfile(textures);
    }

    private void DrawPictures(List<Texture2D> textures) {
        for (int i = 0; i < textures.Count; i++) {
            Rect rect = new Rect(0, 0, textures[i].width, textures[i].height);
            Sprite sprite = Sprite.Create(textures[i], rect, new Vector2(0.5f, 0.5f));
            Image image = ((GameObject)Instantiate(imageTemplate, transform)).GetComponent<Image>();
            image.preserveAspect = true;
            image.sprite = sprite;
        }
    }
}
