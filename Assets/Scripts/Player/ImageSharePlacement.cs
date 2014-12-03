using UnityEngine;
using System.Collections;

public class ImageSharePlacement : MonoBehaviour {

	private Transform player;           // Reference to the player's transform.
	private Camera playerCamera;        // Reference to the player's transform.
	public Texture2D imageShare;		//Reference to the image share texture

	private Transform myTransform;		//reference to the object's transform	

	private int cursorHeight = 300;		//height of texture
	private int cursorWidth = 300;		//width of texture
	private bool showShare = false;

	void Update()
	{
		//check if share should be displayed
		if (Input.GetButton ("Show") && showShare == false ){//&& timer <= autoHideTime) {
			showShare = true;
		} 

		if(Input.GetButton("Hide"))
		   showShare = false;

	}

	void Start() {
		playerCamera =  GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		myTransform = transform;    //So you don't GetComponent your transform with every OnGUI call	
	}

	void OnGUI() {

		float x = Input.mousePosition.x-(cursorWidth>>1);
		float y = Input.mousePosition.y+(cursorHeight>>1);
		if(showShare)
			GUI.DrawTexture(new Rect(x,Screen.height - y, cursorWidth, cursorHeight), imageShare);
	}
}




