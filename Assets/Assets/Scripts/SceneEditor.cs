
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.SceneManagement;

//http://www.thegamecontriver.com/2014/10/create-sliding-pause-menu-unity-46-gui.html
//http://docs.unity3d.com/Manual/LoadingResourcesatRuntime.html
//https://unity3d.com/learn/tutorials/topics/scripting/persistence-saving-and-loading-data

public class SceneEditor : MonoBehaviour
{

    public Camera mainCamera;
    public Canvas sceneCanvas;
    public RectTransform sceneSelector;
    public GameObject uiPanel;
    public GameObject scenes;

    public GameObject assetSelector;
    public Transform startPos;
    public Text placeHolder;
    public Text storyText;

    public GameObject nameStoryPanel;
    public GameObject confirmNamePanel;

    public GameObject gamePanel;
    public GameObject themeSidebar;
    public GameObject assetSidebar;

    public Text storyNameText;
    public InputField setNameInput;
    public InputField confirmNameInput;
    public int sceneCount = 6;

    public Button saveNameButton;
    public Button confirmNameButton;
    public Button menuBtn;
    public Button addBtn;
    public Button rmvBtn;
    //public Button backBtn;

    public GameObject sceneScroller;
    public GameObject themeScroller;
    public GameObject stickerPool;
    public GameObject peoplePool;
    public InputField storyTextInput;

    private Animator anim;
    private Animator animStickers;

    private Dictionary<string, Sprite> stickerSprites = new Dictionary<string, Sprite>();
    private HashSet<string> themeStrings = new HashSet<string>();
    private Dictionary<string, GameObject> themes = new Dictionary<string, GameObject>();
    private HashSet<string> newStickerThemeNames = new HashSet<string>();
    private const int MAX_SCENES = 5;

    private GameObject activeTheme;
    private GameObject stickerPanelTemplate;
    //private Texture2D[] savedScenes = new Texture2D[MAX_SCENES];
    private List<Texture2D> savedScenes = new List<Texture2D>();


    private float loadingTimer = 3f;
    private float loadingTimeout = 20f;
    private bool storyFinished = false;
    private bool initialized = false;
    private bool populatedLists = false;
    private bool sideBarActive = false;
    private bool stickerBarActive = false;
    private string storyName = "";
    private int counter;

    void Start()
    {
        /*if (!PlayerPrefs.HasKey("firstTime"))
        {
            PlayerPrefs.SetInt("firstTime", 1);
            togglePanels(instructionsPanel, true);
        }
        else if (PlayerPrefs.GetInt("firstTime") == 0)
        {
            togglePanels(instructionsPanel, false);
        }*/

        // Initialize variables
        stickerPanelTemplate = Instantiate(assetSelector);
        togglePanels(sceneSelector.gameObject, true);
        populateMenus("BackgroundThumbnails", "Scene", sceneScroller);
        addListeners(sceneScroller);
        populateThemeSidebar();
        addListeners(themeScroller);

        anim = themeSidebar.GetComponent<Animator>();
        anim.enabled = false;

        animStickers = assetSidebar.GetComponent<Animator>();
        animStickers.enabled = false;

        //hide buttons until scene background has been selected
        menuBtn.gameObject.SetActive(false);
        rmvBtn.gameObject.SetActive(false);

        //keep back button active 
        //backBtn.gameObject.SetActive(true);
    }


    void addListeners(GameObject section)
    {
        foreach (Transform child in section.transform)
        {
            // Save name of each child to a string
            string captured = child.name;
            switch (section.name)
            {
                case "SceneSelectionPanel":
                    // Add listener to Scene selections
                    child.GetComponent<Button>().onClick.AddListener(() => changeBackgroundImg(captured));
                    break;
                case "ThemePopPanel":
                    // Add listeners to Theme import scroll panel
                    child.GetComponent<Button>().onClick.AddListener(() => selectAssetTheme(captured));
                    break;
                case "AssetPopPanel":
                    // Add listeners to Assets Scroll panel
                    child.GetComponent<Button>().onClick.AddListener(() => addAsset(captured));
                    break;
            }
        }
    }

    //This method calls in all assets from the LayoutImages folder in the persistantPath
    void addNewAssets(string prefab, GameObject parent)
    {
        foreach (string file in System.IO.Directory.GetFiles(Application.persistentDataPath + "/Uploads/"))
        {
            counter = counter + 1;
            Texture2D tex = null;
            byte[] fileData;
            var fileName = file;
            fileData = File.ReadAllBytes(fileName);
            Debug.Log("File is around:" + file);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.

            var fileIndex = file.LastIndexOf("/");
            var named = file.Substring(fileIndex + 1, file.Length - fileIndex - 1);
            Rect rec = new Rect(0, 0, tex.width, tex.height);
            GameObject j = Instantiate(Resources.Load(prefab)) as GameObject;
            Sprite temp = Sprite.Create(tex, rec, new Vector2());
            j.GetComponent<Image>().sprite = temp;
            stickerSprites.Add(named + counter, temp);
            Debug.Log("tex is: " + named);
            stickerSprites[named] = temp;
            Debug.Log("temp is: " + temp);
            j.transform.SetParent(parent.transform);
            j.transform.position = j.transform.parent.position;
            j.name = named;
            Debug.Log(" is: " + tex);
        }

    }

    void changeBackgroundImg(string name)
    {
        //Remove naming convention from resource folder
        name = Regex.Replace(name, "[0-9]", "");
        
        //find resouce folder backgrounds
        string backgroundName = "Backgrounds/" + name;
        
        //Create a new Texture2D object and assign the image from the resource folder to this gameobject
        Texture2D background = Instantiate(Resources.Load(backgroundName)) as Texture2D;
        
        //Create a new rectangle from the width and height of the image.
        Rect r = new Rect(0, 0, background.width, background.height);
        
        //Find the scene canvas, locate the image property and replace it with the new background iamge
        //sceneCanvas.GetComponent<Image>().sprite = Sprite.Create(background, 0, 0, background.width, background.height, new Vector2());
        sceneCanvas.GetComponent<Image>().sprite = Sprite.Create(background, r, new Vector2());

        //Toggle additional gui objects off during the choosing of the background
        togglePanels(sceneSelector.gameObject, false);
        menuBtn.gameObject.SetActive(true);
        rmvBtn.gameObject.SetActive(true);
        addBtn.gameObject.SetActive(true);
    }

    void selectAssetTheme(string theme)
    {
        //Check if the theme to be imported is not the "Imports" folder
        if (theme != "Imports") {

            //Find any child elements that may exist already and detatch them from the gameobject
            if (assetSelector.transform.childCount > 0)
                removeChildren(assetSelector.transform.GetChild(0).gameObject);

            //Check if the theme panel already contains elements
            if (themeStrings.Contains(theme) || newStickerThemeNames.Contains(theme))
            {
                // If another theme is active, make it invisible
                if (activeTheme != null && activeTheme != themes[theme])
                {
                    togglePanels(activeTheme, false);
                }

                //Make the slected them panel active
                togglePanels(themes[theme], true);
                activeTheme = themes[theme];
                activeTheme.transform.SetParent(assetSelector.transform);

                //Use the slide in animation
                animStickers.enabled = true;
                slideIn(animStickers);
                stickerBarActive = true;
                return;
            }

        }

        //If the theme to be imported is contained within the "Imports" folder, populate with the new data path
        if (theme == "Imports")
        {
           
            themeStrings.Add(theme);
            string themeName = "Stickers/" + theme;
            stickerBarActive = true;
           // deleteChildren(assetSelector);
            addNewAssets("AssetSelector", assetSelector);
            addListeners(assetSelector);
            animStickers.enabled = true;
            slideIn(animStickers);

            // Instantiate the stickers from the theme and add them to a list
            GameObject go = Instantiate(assetSelector);
            go.name = theme + "Panel";
            Debug.Log("Theme Add: " + go);
            themes.Add(theme, go);
            activeTheme = themes[theme];

        }
    }

    //Method that removes all children from a game object
    void removeChildren(GameObject go)
    {
        go.transform.SetParent(null);
    }

    // Adding each theme type to the theme sidebar
    void populateThemeSidebar()
    {
        populateMenus("Themes", "Theme", themeScroller);

        foreach (Transform theme in themeScroller.transform)
        {
            //Remove any number naming conventions applied to each asset
            theme.name = Regex.Replace(theme.name, "[0-9]", "");
            //Call the populate asset panel
            populateAssetList(theme.name);
            theme.GetChild(0).GetComponent<Text>().text = theme.name;
        }
        sideBarActive = true;
    }

    //Take a screenshot of the current elements of the screen and their relative positions
    public void takeScreenshot()
    {
        //If the number of scenes added to a story is less than this set amount take a screenshot
        if (scenes.transform.childCount < sceneCount)
        {
            screenshot();
            // remove all assets, gameobjects and text
            removeAllAssets();
            storyTextInput.text = "";
        }
    }

    //Method to remove all game assets from the scene
    private void removeAllAssets()
    {
        //Firstly remove all of the assets found on the screen
        foreach (Transform child in stickerPool.transform)
        {
            Destroy(child.gameObject);
        }

        //Then remove all of the people game objects on the screen
        foreach(Transform child in peoplePool.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Save scene as snapshot to save the scene for viewing later
    private void screenshot()
    {
        if (storyText.text.Length == 0)
        {
            storyText.transform.parent.GetComponent<CanvasGroup>().alpha = 0;
        }

        uiPanel.GetComponent<CanvasGroup>().alpha = 0;
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        mainCamera.targetTexture = rt;

        GameObject nextScene = Instantiate(Resources.Load("Scene")) as GameObject;
        nextScene.name = "Scene" + (scenes.transform.childCount + 1);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = mainCamera.targetTexture;
        mainCamera.Render();
        Rect r = new Rect(0, 0, mainCamera.targetTexture.width, mainCamera.targetTexture.height);
        Texture2D imageOverview = new Texture2D(mainCamera.targetTexture.width, mainCamera.targetTexture.height, TextureFormat.RGB24, false);
        imageOverview.ReadPixels(r, 0, 0);
        imageOverview.Apply();
        imageOverview.name = nextScene.name;
        savedScenes.Add(imageOverview);
        RenderTexture.active = currentRT;

        Sprite sp = Sprite.Create(imageOverview, r, new Vector2());

        nextScene.GetComponent<Image>().sprite = sp;
        nextScene.transform.SetParent(scenes.transform);
        nextScene.transform.position = nextScene.transform.parent.position;
        mainCamera.targetTexture = null;

        togglePanels(sceneSelector.gameObject, true);

        // Show instructions panel to prompt user
        if (scenes.transform.childCount == 1) showHideReminder(true);
        storyText.text = null;
        storyText.transform.parent.GetComponent<CanvasGroup>().alpha = 1;
        uiPanel.GetComponent<CanvasGroup>().alpha = 1;
    }


    void populateAssetList(string theme)
    {
        GameObject themePanel = Instantiate(assetSelector);
        themeStrings.Add(theme);
        string themeName = "Stickers/" + theme;
        stickerBarActive = true;
        populateMenus(themeName, "AssetSelector", themePanel);

        populatedLists = true;

        // Instantiate the stickers from the theme and add them to a list
        themePanel.name = theme + "Panel";
        themes.Add(theme, themePanel);

        foreach (Transform child in themePanel.transform)
        {
            string captured = child.name;
            child.GetComponent<Button>().onClick.AddListener(() => addAsset(captured));
        }
    }

    //The following methods are variations found from the follwing source
    //http://www.thegamecontriver.com/2014/10/create-sliding-pause-menu-unity-46-gui.html
    //Methods that handles the sliding in and out of the side panels
    private void slideIn(Animator anim)
    {
        anim.Play("SlideIn");
    }

    //Method that handles the sliding in and out of the secondary side panel
    private void slideOut(Animator anim)
    {
        anim.Play("SideBarSlideOut");
    }

    //When the menu button is pressed "animate" the side panels
    public void sideBarButton()
    {
        //play the animation
        anim.enabled = true;

        //check if the menu bars are active
        //if not slide out the menu
        if (!stickerBarActive)
        {
            //if the sidepanel is not active, slide the panel out
            if (sideBarActive)
            {
                slideOut(anim);
                sideBarActive = false;
                menuBtn.transform.GetChild(0).GetComponent<Text>().text = "Menu";
            }
            //else if the menu is already out change the text of the side bar button
            else {
                menuBtn.transform.GetChild(0).GetComponent<Text>().text = "Cancel";
                slideIn(anim);
                sideBarActive = true;
            }
        }
        else {
            slideOut(animStickers);
            stickerBarActive = false;
        }
    }

    void addAsset(string name)
    {
        // Spawn a new sticker in the middle of the game area
        Debug.Log("Asset " + name);
        GameObject sticker = Instantiate(Resources.Load("Asset")) as GameObject;

        Sprite temp = stickerSprites[name];
        sticker.name = name;
        sticker.GetComponent<Image>().sprite = temp;
        sticker.transform.position = startPos.transform.position;
        sticker.transform.SetParent(stickerPool.transform, true);
    }

    public void removeAsset()
    {
        //Find the assetpool gameobject and corresponding child elements
        //if child objects exist, find the last clikced asset and remove it from the scene
        if (stickerPool.transform.childCount > 0)
        {
            Destroy(stickerPool.transform.GetChild(stickerPool.transform.childCount - 1).gameObject);
        }
    }

    private void populateMenus(string resource, string prefab, GameObject parent)
    {
        foreach (Texture2D t in Resources.LoadAll(resource, typeof(Texture2D)))
        {
            Rect r = new Rect(0, 0, t.width, t.height);
            GameObject i = Instantiate(Resources.Load(prefab)) as GameObject;
            Sprite temp = Sprite.Create(t, r, new Vector2());
            i.GetComponent<Image>().sprite = temp;

            if (prefab.Equals("AssetSelector"))
            {
                stickerSprites.Add(t.name, temp);
            }

            i.transform.SetParent(parent.transform);
            i.transform.position = i.transform.parent.position;
            i.name = t.name;
        }
    }

    public void showHideReminder(bool show)
    {
        CanvasGroup cg = GameObject.Find("ReminderPanel").GetComponent<CanvasGroup>();

        if (!show)
        {
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
        else {
            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    public void saveStory()
    {
        storyFinished = true;
        confirmNameInput.text = storyName;

        if (savedScenes.Count != 0)
        {
            CanvasGroup cg = confirmNamePanel.GetComponent<CanvasGroup>();
            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    public void setName()
    {
        CanvasGroup cg = nameStoryPanel.GetComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        storyName = storyNameText.text;

        storyNameText.transform.parent.GetComponent<InputField>().text = "";
    }

    public void saveName()
    {
        string fileName = confirmNameInput.text.Trim();
        // For each child of scenes, save as name to filename/sceneName
        foreach (Texture2D scene in savedScenes)
        {
            if (scene != null)
            {
                byte[] bytes = scene.EncodeToPNG();

                string path = Application.persistentDataPath + "/" + fileName + "/";
                System.IO.Directory.CreateDirectory(path);
                File.WriteAllBytes(path + scene.name + ".png", bytes);
            }
        }
        Destroy(mainCamera);
        //SceneManager.LoadScene("StoryViewer");
    }

    //Method to remove text in an input text field
    public void removePlaceHolder()
    {
        placeHolder.text = "";
    }

    // Deactivate/Reactivate panels
    void togglePanels(GameObject go, bool activate)
    {
        CanvasGroup cg = go.GetComponent<CanvasGroup>();

        if (activate)
        {
            cg.alpha = 1;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }
        else {
            cg.alpha = 0;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }
    }

    public bool isListPopulated()
    {
        return populatedLists;
    }
}

