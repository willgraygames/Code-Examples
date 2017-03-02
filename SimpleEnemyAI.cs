using UnityEngine;
using System.Collections;

public class SimpleEnemyAI : MonoBehaviour {

	public Transform target;					//The target the enemy is set to follow
	public int moveSpeed;						//The speed at which the enemy moves
	public int rotationSpeed;					//The speed at which the enemy rotates
	public int minDistance;						//The minimum distance to keep from the player

	void Start () {
		//Sets the enemy's target to the player GameObject
		GameObject go = GameObject.FindGameObjectWithTag ("Player");

		target = go.transform;
	}

	void Update () {
		//Rotates the enemy to face the target according to rotationSpeed
		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (target.position - transform.position), rotationSpeed * Time.deltaTime);

		//So long as the enemy is not at the minDistance, move towards the player according to moveSpeed
		if (Vector3.Distance (target.position, transform.position) > minDistance) {
			transform.position += transform.forward * moveSpeed * Time.deltaTime;
		}
	}
}
