using UnityEngine;
using System.Collections;

public class ImageSharePlacement : MonoBehaviour {

	private Transform player;           // Reference to the player's transform.
	private Camera playerCamera;           // Reference to the player's transform.
	public Texture2D imageShare;		//Reference to the image share texture

	private Transform myTransform;		//reference to the object's transform	

	private int cursorHeight = 300;		//height of texture
	private int cursorWidth = 300;		//width of texture
	private bool showShare = false;
	//public float autoHideTime = 0.15f;        // The time when share disappears automatically
	//float timer;                              // A timer to determine when to hide share.
	//float effectsDisplayTime = 0.2f;         // The proportion of the time that the share will display for.

	/*void Awake()
	{
		// Setting up the reference.
		player = GameObject.FindGameObjectWithTag(Tags.player).transform;
		playerCamera = GameObject.FindGameObjectWithTag(Tags.mainCamera);
		//transform.position = player.forward;
	}*/

	void Update()
	{
		// Add the time since Update was last called to the timer.
		//timer += Time.deltaTime;

		//check if share should be displayed
		if (Input.GetButton ("Show") && showShare == false ){//&& timer <= autoHideTime) {
			showShare = true;
		} 

		if(Input.GetButton("Hide"))
		   showShare = false;

		// If the timer has exceeded the proportion of time that the share should be displayed for
		/*if(timer >= autoHideTime * effectsDisplayTime && showShare)
		{
			//hide the share
			showShare = false;
		}*/

	}

	void Start() {
		playerCamera =  GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		myTransform = transform;    //So you don't GetComponent your transform with every OnGUI call
		//Screen.showCursor = false;
		
	}
	

	void OnGUI() {
		
		//Vector3 screenPos = playerCamera.WorldToScreenPoint(myTransform.position);
		//screenPos.y = Screen.height - screenPos.y; //The y coordinate on screenPos is inverted so we need to set it straight
		//GUI.DrawTexture(new Rect(screenPos.x, screenPos.y, cursorWidth, cursorHeight), imageShare);

		float x = Input.mousePosition.x-(cursorWidth>>1);
		float y = Input.mousePosition.y+(cursorHeight>>1);
		if(showShare)
			GUI.DrawTexture(new Rect(x,Screen.height - y, cursorWidth, cursorHeight), imageShare);
	}

	/*void RenderGame()
	{
		float x = Input.mousePosition.x-(cursorWidth>>1);
		float y = Input.mousePosition.x+(cursorHeight>>1);

		GUI.DrawTexture(new Rect(x,y, cursorWidth, cursorHeight), imageShare);

	}*/
}




