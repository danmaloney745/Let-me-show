using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CreateImageDisplay : MonoBehaviour {

    public RectTransform sceneSelector;
    public Transform startPos;

    private Dictionary<string, Sprite> assetSprites = new Dictionary<string, Sprite>();

    // Use this for initialization
    void Start () {
        toggleScenePanel(true);
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void addListeners(GameObject section)
    {
        foreach (Transform child in section.transform)
        {
            // Save name of each child to memory
            string captured = child.name;
            string spriteName = child.gameObject.GetComponent<Image>().sprite.name;

            //Find the required panel
            switch (section.name)
            {
                case "AssetSelectionPanel":
                    // Add listeners to stickers
                    Debug.Log(captured);
                    child.GetComponent<Button>().onClick.AddListener(() => addSticker(captured));
                    break;
            }
        }
    }

    void addSticker(string name)
    {
        //Instantiate a sticker in the scene editor
        GameObject sticker = Instantiate(Resources.Load("Sticker")) as GameObject;
       
        Sprite temp = assetSprites[name];
        sticker.name = name;
        sticker.GetComponent<Image>().sprite = temp;
        sticker.transform.position = startPos.transform.position;
        Debug.Log("New Sticker " + name);

        sticker.transform.SetParent(startPos.transform.parent, true);
    }

    void toggleScenePanel(bool activate)
    {
        CanvasGroup cg = sceneSelector.GetComponent<CanvasGroup>();
          
        if (activate)
        {
            cg.alpha = 1;
            cg.blocksRaycasts = true;
            cg.interactable = true;
            Debug.Log("activate false");
        }
        else {
            cg.alpha = 0;
            cg.blocksRaycasts = false;
            cg.interactable = false;
            Debug.Log("activate true");
        }
    }

   
}
