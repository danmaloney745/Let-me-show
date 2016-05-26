using UnityEngine;
using System.Collections;

public class FollowUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    //drag the player's transform here in the inspector
    public Transform player;
    //drag the guiText here in the inspector
    public GUIText debugGUIText;

    void Update()
    {
        debugGUIText.transform.position = Camera.main.WorldToViewportPoint(player.position);
    }
}
