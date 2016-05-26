using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Instantiate : MonoBehaviour {

    public Button addbtn;
    public Button rmvBtn;
    public GameObject personPrefab;
    public GameObject peopleTracker;
    public GameObject assetTracker;
    public Transform assetPos;
    public Transform personPos;

    private string placeholder;
    private int maxPeople = 4;
    private List<GameObject> peopleList = new List<GameObject>();

	// Use this for initialization and adding a person to the scene
	/*void Start () {
        if(peopleList != null)
        {
            foreach (GameObject person in peopleList)
            {
                Destroy(person);
            }
            peopleList.Clear();
        }
	}*/

    /*GameObject lastClicked;
    Ray ray;
    RaycastHit rayHit;

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out rayHit))
            {
                lastClicked = rayHit.collider.gameObject;
                placeholder = lastClicked.transform.parent.name;
                if (lastClicked != null)
                    print(lastClicked.name);
            }
        }
    }*/

    public void addPerson()
    {
        if(peopleTracker.transform.childCount < maxPeople)
        {
            GameObject person = (GameObject)Instantiate(personPrefab);
            //peopleList.Add(person);
            person.transform.position = personPos.transform.position;
            person.transform.SetParent(peopleTracker.transform, true);
        }

        else
        {
            Debug.Log("Max People Reached");
        }

        //Instantiate(Resources.Load("Prefabs/Person (2)", typeof(GameObject)), Vector3.zero, Quaternion.identity);
    }

    public void removeObject()
    {
        if (assetTracker.transform.childCount > 0)
        {
            Destroy(assetTracker.transform.GetChild(assetTracker.transform.childCount - 1).gameObject);
        }
    }
}
