
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System.IO;

public class DisplayScreenShots : MonoBehaviour
{
    public Canvas sceneCanvas;
    public RectTransform sceneSelector;
    public GameObject itemSelector;
    public GameObject stickerSelector;
    public Transform startPos;
    public Text placeHolder;

    public GameObject sideBar;
    public GameObject stickerSidebar;

    public GameObject sceneScroller;
    public GameObject themeScroller;

    public GameObject assets;

    public Button menuBtn;
    public Button bckBtn;
    public Button rmvBtn;
    public Button addPersonBtn;

    private Dictionary<string, int> stickerNames = new Dictionary<string, int>();


    private Animator anim;
    private Animator animStickers;
    private Dictionary<string, Sprite> stickerSprites = new Dictionary<string, Sprite>();
    private HashSet<string> themeStrings = new HashSet<string>();
    private Dictionary<string, GameObject> themes = new Dictionary<string, GameObject>();
    private GameObject activeTheme;
    private string sceneToLoad;

    private bool sideBarActive = false;
    private bool stickerBarActive = false;


    bool import = false;
    int counter;

    void Start()
    {
        togglePanels(sceneSelector.gameObject, true);

        populateMenus("screenshot", "Scene", sceneScroller);
        addListeners(sceneScroller);
 
        addListeners(themeScroller);

        anim = sideBar.GetComponent<Animator>();
        anim.enabled = false;

        animStickers = stickerSidebar.GetComponent<Animator>();
        animStickers.enabled = false;

        //hide buttons until scene background has been selected
        menuBtn.gameObject.SetActive(false);
        rmvBtn.gameObject.SetActive(false);
        addPersonBtn.gameObject.SetActive(false);

        //keep back button active 
        bckBtn.gameObject.SetActive(true);

    }

    void addListeners(GameObject section)
    {
        foreach (Transform child in section.transform)
        {
            // Save name of each child to memory
            string captured = child.name;
            string spriteName = child.gameObject.GetComponent<Image>().sprite.name;
            switch (section.name)
            {
                case "SceneSelectionPanel":
                    // Add listener to Scene selections
                    child.GetComponent<Button>().onClick.AddListener(() => changeImage(captured));
                    break;
            }
        }
    }

    //This method calls in all assets from the LayoutImages folder in the persistantPath
   

    void changeImage(string name)
    {
        name = Regex.Replace(name, "[0-9]", "");
        string sceneName = "Backgrounds/" + name + "Scene";
        Texture2D background = Instantiate(Resources.Load(sceneName)) as Texture2D;
        Rect r = new Rect(0, 0, background.width, background.height);

        sceneCanvas.GetComponent<Image>().sprite = Sprite.Create(background, r, new Vector2());
        togglePanels(sceneSelector.gameObject, false);

        menuBtn.gameObject.SetActive(true);
        rmvBtn.gameObject.SetActive(true);
        addPersonBtn.gameObject.SetActive(true);
    }

    public void deleteAsset()
    {
        if (assets.transform.childCount > 0)
        {
            Destroy(assets.transform.GetChild(assets.transform.childCount - 1).gameObject);
        }
    }

    private void slideIn(Animator temp)
    {
        temp.Play("SlideIn");
    }

    private void slideOut(Animator temp)
    {
        temp.Play("SideBarSlideOut");
    }

    public void sideBarButton()
    {
        anim.enabled = true;

        if (!stickerBarActive)
        {
            if (sideBarActive)
            {
                slideOut(anim);
                sideBarActive = false;
                menuBtn.transform.GetChild(0).GetComponent<Text>().text = "Menu";
            }
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

    private void populateMenus(string resource, string prefab, GameObject parent)
    {
        Texture2D t = null;
        byte[] fileData;

        //foreach (Texture2D t in Resources.LoadAll(resource, typeof(Texture2D)))
        //{
        foreach (string file in System.IO.Directory.GetFiles(Application.persistentDataPath))
        {

            if (file.Contains("screenshot"))
            {
                fileData = File.ReadAllBytes(file);
                // File.WriteAllBytes(Application.persistentDataPath + "/LayoutImages/"+ fileData + "1", fileData);
                t = new Texture2D(2, 2);
                t.LoadImage(fileData); //..this will auto-resize the texture dimensions.

                Rect r = new Rect(0, 0, t.width, t.height);
                GameObject i = Instantiate(Resources.Load(prefab)) as GameObject;
                Sprite temp = Sprite.Create(t, r, new Vector2());
                i.GetComponent<Image>().sprite = temp;

                if (prefab.Equals("StickerSelector"))
                {
                    stickerSprites.Add(t.name, temp);
                }

                i.transform.SetParent(parent.transform);
                i.transform.position = i.transform.parent.position;
                i.name = t.name;
            }
        }
    }

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
}
