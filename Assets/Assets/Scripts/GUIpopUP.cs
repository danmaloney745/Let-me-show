using UnityEngine;
using System.Collections;

public class GUIpopUP : MonoBehaviour
{
    int window;
    public object personEmo;
    private Animator animator;

    public Rect windowRect0 = new Rect(0, 0, Screen.width, Screen.height);
    public Rect windowRect1 = new Rect(420, 540, 100, 50);
    public Rect windowRect2 = new Rect(291, 540, 100, 50);
    public Rect windowRect3 = new Rect(168, 540, 100, 50);
    public Rect windowRect4 = new Rect(39, 540, 100, 50);
    
    bool display = true;
    private Texture wheatButton;
    public GameObject gob;
    float hungry;
    Vector3 screenPosition;

    AudioSource happyAudio;
    AudioSource sadAudio;
    AudioSource hungryAudio;
    AudioSource tiredAudio;

    void Start()
    {
        //screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        //screenPosition.y = Screen.height - screenPosition.y;

        //windowRect0 = new Rect(screenPosition.x+200, screenPosition.y-40, 100, 50);

        AudioSource[] allMyAudioSources = GetComponents<AudioSource>();
        happyAudio = allMyAudioSources[0];
        hungryAudio = allMyAudioSources[1];
        sadAudio = allMyAudioSources[2];
        tiredAudio = allMyAudioSources[3];
    }


    public void OnGUI()
    {
        windowRect0 = GUI.Window(0, windowRect0, DoMyWindowHappy, "Happy");
        windowRect1 = GUI.Window(1, windowRect1, DoMyWindowSad, "Sad");
        windowRect2 = GUI.Window(2, windowRect2, DoMyWindowHungry, "Hungry");
        windowRect3 = GUI.Window(3, windowRect3, DoMyWindowTired, "Tired");
        windowRect4 = GUI.Window(4, windowRect4, DoMyWindowDelete, "Delete");
    }

    public void DoMyWindowDelete(int windowID)
    {
        wheatButton = (Texture)Resources.Load("Trash_Can");

        if (GUI.Button(new Rect(10, 20, 100, 100), wheatButton))
        {
            Destroy(this.gob);
        }

        //GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    public void DoMyWindowSad(int windowID)
    {
        wheatButton = (Texture)Resources.Load("sad");
       
        if (GUI.Button(new Rect(10, 20, 100, 100), wheatButton))
        {
            gob.transform.GetComponent<Animator>().Play("sad");
            sadAudio.Play();
        }
        // GUI.Button(new Rect(10, 20, 100, 20), "Hello World");

       
        // print("Animation Test Button........." + windowID);

       //GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    public void DoMyWindowTired(int windowID)
    {
        wheatButton = (Texture)Resources.Load("hungry");
        
        // Debug.Log("OnGUI");
        if (GUI.Button(new Rect(10, 20, 100, 100), wheatButton)) {
            tiredAudio.Play();
        }
        // GUI.Button(new Rect(10, 20, 100, 20), "Hello World");


        //GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }



    public void DoMyWindowHungry(int windowID)
    {
        wheatButton = (Texture)Resources.Load("hungry");
       
        // Debug.Log("OnGUI");
        if (GUI.Button(new Rect(10, 20, 100, 100), wheatButton)) {

            gob.transform.GetComponent<Animator>().Play("hungry");
            hungryAudio.Play();
        }
        // GUI.Button(new Rect(10, 20, 100, 20), "Hello World");

     
        // print("Animation Test Button........." + windowID);

        //GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    public void DoMyWindowHappy(int windowID)
    {
        wheatButton = (Texture)Resources.Load("happy");

        // Debug.Log("OnGUI");
        if (GUI.Button(new Rect(10, 20, 100, 100), wheatButton)) {
            gob.transform.GetComponent<Animator>().Play("happy");
            happyAudio.Play();
        }
        // GUI.Button(new Rect(10, 20, 100, 20), "Hello World");

      
        // print("Animation Test Button........." + windowID);

        //GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }
}
