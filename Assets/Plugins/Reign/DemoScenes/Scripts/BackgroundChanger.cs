using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Reign;
using System.IO;
using System.Text.RegularExpressions;

/* Alternate method of changing the background using a button and streams to access the file picker.
*  The main canvas found in the scene is then found and the background image is changed
*/

public class BackgroundChanger : MonoBehaviour {
    public Button  ImagePickerButton;
    public Boolean backfolder = true;
    public Boolean uploadfolder = true;
    public bool uploaded = false;
    public bool background = false;

    public Canvas sceneCanvas;
    public Button menuBtn;
    public Button addBtn;
    public Button rmvBtn;
    public Button backBtn;

    public int counter = 0;
    
    // Use this for initialization
    void Start () {
        ImagePickerButton.onClick.AddListener(imagePickerClicked);
        
        //hide buttons until scene background has been selected
        menuBtn.gameObject.SetActive(false);
        rmvBtn.gameObject.SetActive(false);
        addBtn.gameObject.SetActive(false);
        //keep back button active 
        backBtn.gameObject.SetActive(true);
    }
	
    //Enable the button to select the image
    private void enableButtons()
    {
        ImagePickerButton.enabled = true;
    }

    //Open the streams and locate image types of png, jpg and jpeg, once found call the image call back method
    private void imagePickerClicked()
    {
        // NOTE: Unity only supports loading png and jpg data
        uploaded = true;
        StreamManager.LoadFileDialog(FolderLocations.Pictures, 128, 128, new string[] { ".png", ".jpg", ".jpeg" }, imageLoadedCallback);

    }


    private void imageLoadedCallback(Stream stream, bool succeeded)
    {

        enableButtons();
        MessageBoxManager.Show("Image Status", "Image Loaded: " + succeeded);
        if (!succeeded)
        {
            if (stream != null) stream.Dispose();
            return;
        }

        try
        {

            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            var newImage = new Texture2D(4, 4);
            newImage.LoadImage(data);
            newImage.Apply();
            Rect r = new Rect(0, 0, newImage.width, newImage.height);
            sceneCanvas.GetComponent<Image>().sprite = Sprite.Create(newImage, r, new Vector2());

            addBtn.gameObject.SetActive(true);
            menuBtn.gameObject.SetActive(true);
            rmvBtn.gameObject.SetActive(true);
            ImagePickerButton.gameObject.SetActive(false);
            
        }
        catch (Exception e)
        {
            MessageBoxManager.Show("Error", e.Message);
        }
        finally
        {
            // NOTE: Make sure you dispose of this stream !!!
            if (stream != null) stream.Dispose();
        }

    }
}
