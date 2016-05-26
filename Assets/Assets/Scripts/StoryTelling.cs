using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StoryTelling : MonoBehaviour {

    private bool showText = true;
   
    private string story;
    public Rect Rect = new Rect(420, 540, 100, 50);
    public Rect textArea = new Rect(0, 0, Screen.width, Screen.height);
    public string stringToEdit = "Hello World";
    // Use this for initialization
    void Start () {
        var input = gameObject.GetComponent<InputField>();
        var se = new InputField.SubmitEvent();
        se.AddListener(SubmitName);
        input.onEndEdit = se;

        //or simply use the line below, 
        //input.onEndEdit.AddListener(SubmitName);  // This also works
    }

    private void SubmitName(string arg0)
    {
        Debug.Log(arg0);
        story = arg0;
    }

    void OnGUI()
    {
        GUI.backgroundColor = Color.yellow;
        //GUI.Window(0, Rect, TellStory, "Story");
        //GUI.Label(textArea, "Here is a block of text\nlalalala\nanother line\nI could do this all day!");
      
    }

    public void TellStory(int windowID)
    {
      
    }


}
