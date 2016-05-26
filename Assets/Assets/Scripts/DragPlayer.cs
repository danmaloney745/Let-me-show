using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]

public class DragPlayer : MonoBehaviour {
    public Transform gameCanvas;
    public Transform gamePanel;
    public float scaleFactor = 0.5f;        // The rate of change of the scale
    public float speed = 0.1F;

    private Vector3 screenPoint;
    private Vector3 offset;
    private Touch finger1, finger2;
 
    private Vector3 minScale;
    private Vector3 maxScale;
    private Vector3 baseScale;
    private float maxSize;
    private float minSize;
    private bool activeAsset;


    void Start()
    {
        gameCanvas = GameObject.Find("Canvas").transform;
        gamePanel = GameObject.Find("People").transform;

        //get original size of object
        baseScale = transform.localScale;
        maxScale = baseScale * 3;
        maxSize = maxScale.magnitude;

        minScale = baseScale;
        minSize = minScale.magnitude;
    }

    void Update()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
        {
            RaycastHit rayHit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); ;

            if (Physics.Raycast(ray, out rayHit))
            {
                if (rayHit.collider.gameObject == gameObject)
                {
                    Debug.Log(rayHit.collider.name);
                    activeAsset = true;
                }
            }
            else {
                activeAsset = false;
            }
        }

        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            //Scale an asset
            if (transform.localScale.magnitude > minScale.magnitude
                    && transform.localScale.magnitude < maxScale.magnitude)
            {
                transform.localScale += new Vector3(deltaMagnitudeDiff, deltaMagnitudeDiff, scaleFactor);
            }
            else if (transform.localScale.magnitude < minScale.magnitude)
            {
                // If asset is less than min size, return to base szie
                transform.localScale = baseScale;

            }
            else if (transform.localScale.magnitude > (baseScale.magnitude * 2))
            {
                // If sticker is more than max size, return to base size
                transform.localScale = baseScale;
            }

        }

    }
    void OnMouseDown()
    {
    #if !UNITY_EDITOR
		    if(Input.touchCount == 1) {
			    // Center object on camera
			    screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

			    offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		    } 
    #endif
    #if UNITY_EDITOR

            // Center object on camera
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

		    // Set the active sticker as the last one dragged
		    offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    #endif
	}

    void OnMouseDrag()
    {
    #if !UNITY_EDITOR
		    if (Input.touchCount == 1) {
			    transform.SetParent(gameCanvas);
			    Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
			    Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
			    transform.position = curPosition;
		    }
    #endif
    #if UNITY_EDITOR
        transform.SetParent(gameCanvas);
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    #endif
    }

    void OnMouseUp()
    {
        transform.SetParent(gamePanel);
        activeAsset = false;
    }
}

