using UnityEngine;
using System.Collections;

public class Emotions : MonoBehaviour{

    Vector2 direction;
    Collider2D current;
    bool escape;
    static GameObject menu;
    int counter= 0;
    int counter1 = 2;
    

    public GUIpopUP pop;

 
    void OnMouseDown()
    {
       
        counter = counter + 1;
        if (counter == counter1)
        {
            pop.enabled = true;
            Debug.Log("counter: " + counter);
            Debug.Log("counter1:  " + counter1);
            

        }
        else if (counter > counter1)
        {
            pop.enabled = false;
            Debug.Log("Counter:  " + counter);
            counter = 0;
            counter1 = 2;     
        }
        
    }

   



    void activate()
    {
        //Cast a ray in the direction specified in the inspector.
        RaycastHit2D hit = Physics2D.Raycast(this.gameObject.transform.position, direction);
        
        
            //Display the point in world space where the ray hit the collider's surface.
            pop.enabled = false;
          
   
        current = hit.collider;
     }
}