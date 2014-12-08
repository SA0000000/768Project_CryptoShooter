using UnityEngine;
using System.Collections;

public class LastPlayerSighting : MonoBehaviour
{
	public Vector3 position = new Vector3(1000f, 1000f, 1000f);			// The last global sighting of the player.
	public Vector3 resetPosition = new Vector3(1000f, 1000f, 1000f);	// The default position if the player is not in sight.
	public float fadeSpeed = 7f;										// How fast the light fades between low and high intensity.
	public float musicFadeSpeed = 1f;									// The speed at which the 
	
	private AudioSource panicAudio;										// Reference to the AudioSource of the panic msuic.

	void Awake ()
	{
		// Setup the reference to the additonal audio source.
		panicAudio = transform.FindChild("secondaryMusic").audio;

	}
	
	
	void Update ()
	{
		// Switch the alarms and fade the music.
		//SwitchAlarms();
		MusicFading();
	}
	

	
	void MusicFading ()
	{
		// If the alarm is not being triggered...
		if(position != resetPosition)
		{
			// ... fade out the normal music...
			audio.volume = Mathf.Lerp(audio.volume, 0f, musicFadeSpeed * Time.deltaTime);
			
			// ... and fade in the panic music.
			panicAudio.volume = Mathf.Lerp(panicAudio.volume, 0.8f, musicFadeSpeed * Time.deltaTime);
		}
		else
		{
			// Otherwise fade in the normal music and fade out the panic music.
			audio.volume = Mathf.Lerp(audio.volume, 0.8f, musicFadeSpeed * Time.deltaTime);
			panicAudio.volume = Mathf.Lerp(panicAudio.volume, 0f, musicFadeSpeed * Time.deltaTime);
		}
	}
}
