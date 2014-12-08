using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
		//Indicates if the associated enemy is within the players collider range
		internal bool inFieldRange { set; get; }

		//Indicates if the associate enemy is still alive
		internal bool Alive { set; get; }

		void Awake ()
		{
				inFieldRange = false;
				Alive = true;
		}
		
		public void KilledInAction ()
		{
				Alive = false;
		}

		//Toggles the inFieldRange variable
		public void SetCollision ()
		{
				inFieldRange = !inFieldRange;
		}
}