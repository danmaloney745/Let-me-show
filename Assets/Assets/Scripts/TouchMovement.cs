using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.

namespace Completed
{
    //Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
    public class TouchMovement : MoveObject
    {

        private Animator animator;                  //Used to store a reference to the Player's animator component.

        private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.

        private bool facingRight = true;

        public float speed = 1.5f;
        private Vector3 target;

        //Start overrides the Start function of MovingObject
        protected override void Start()
        {
            //Get a component reference to the Player's animator component
            animator = GetComponent<Animator>();

            //Call the Start function of the MovingObject base class.
            base.Start();

            target = transform.position;
        }

        private void Update()
        {

            int horizontal = 0;     //Used to store the horizontal move direction.
            int vertical = 0;       //Used to store the vertical move direction.

            //Check if we are running either in the Unity editor or in a standalone build.
            #if UNITY_STANDALONE || UNITY_WEBPLAYER
            if(Input.GetMouseButtonDown(0))
            {
                target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                target.z = transform.position.z;
            }

            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
            horizontal = (int)(Input.GetAxisRaw("Horizontal"));

            if(horizontal > 0 && !facingRight)
            {
                Flip();
            }

            else if(horizontal < 0 && facingRight)
            {
                Flip();
            }

            //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
            vertical = (int)(Input.GetAxisRaw("Vertical"));

            //Check if moving horizontally, if so set vertical to zero.
            if (horizontal != 0)
            {
                vertical = 0;
            }
            
            //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
            #elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
				//Store the first touch detected.
				Touch myTouch = Input.touches[0];
				
				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set touchOrigin to the position of that touch
					touchOrigin = myTouch.position;
				}
				
				//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;
					
					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - touchOrigin.x;
					
					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - touchOrigin.y;
					
					//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					touchOrigin.x = -1;
					
					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//If x is greater than zero, set horizontal to 1, otherwise set it to -1
						horizontal = x > 0 ? 1 : -1;
					else
						//If y is greater than zero, set horizontal to 1, otherwise set it to -1
						vertical = y > 0 ? 1 : -1;
				}
			}
			
            #endif //End of mobile platform dependendent compilation section started above with #elif
            //Check if we have a non-zero value for horizontal or vertical
            if (horizontal != 0 || vertical != 0)
            {
                //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
                //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
                AttemptMove<Object>(horizontal, vertical);
            }
        }

        //AttemptMove overrides the AttemptMove function in the base class MovingObject
        //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
        protected override void AttemptMove<T>(int xDir, int yDir)
        {

            //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
            base.AttemptMove<T>(xDir, yDir);

        }


        //OnCantMove overrides the abstract function OnCantMove in MovingObject.
        //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
        protected override void OnCantMove<T>(T component)
        {

        }


        //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
        private void OnTriggerEnter2D(Collider2D other)
        {
            //Check if the tag of the trigger collided with is Exit.
            if (other.tag == "Exit")
            {
                
                enabled = false;
            }

        }

        private void Flip()
        {
            facingRight = !facingRight;

            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

    }
}

