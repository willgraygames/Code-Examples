using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject enemy;						//Reference to the diamond enemy game object
	public GameObject spawnerHealthBar;				//Reference to the spawner's health bar
	public GameObject trigger;						//Reference to the spawner's trigger
	public GameObject gameManager;					//Reference to the game manager
	public ParticleSystem spawnParticle;			//Reference to the enemy spawn particle system
	public ParticleSystem preDeathParticle;			//Reference to the pre-death particle system
	public ParticleSystem deathParticle;			//Reference to the exploding death particle system

	public float currSpawnerHealth;					//The spawner's current health
	public float maxSpawnerHealth;					//The spawner max health
	public float delay;								//The delay between spawn rounds
	public int spawnRounds;							//The number of spawn rounds
	public int scoreValue;							//The score value of the spawner

	private bool triggerHasEntered = false;			//Has the player entered this spawner's trigger?

	Coroutine co;									//Reference to the spawn coroutine so it can be stopped upon death

	void Start () {
		//Gets the reference to the game manager object
		gameManager = GameObject.FindGameObjectWithTag ("GameController");
		//Sets the spawner's current health to its max health, ensuring that each iteration starts at max health
		currSpawnerHealth = maxSpawnerHealth;
		//Adds the spawner as an enemy in the game manager's enemy count
		gameManager.GetComponent<GameManager> ().enemyCount += 1;
		//Starts the Spawn coroutine with spawnRounds as a parameter
		co = StartCoroutine (Spawn (spawnRounds));
	}

	void Update () {
		//Calculate the health of the spawner
		CalculateHealth ();
		//Sets the triggerHasEntered variable to equal the spawner's trigger's hasEntered variable
		triggerHasEntered = trigger.GetComponent<TriggerScript> ().hasEntered;
		//Checks to see if the spawner has died. If it has, starts the death coroutine
		if (currSpawnerHealth <= 0) {
			StartCoroutine (SpawnerDeath ());
			currSpawnerHealth = 9999999f;
		}
	}

	//Spawn coroutine. Waits until the player has entered the spawner's trigger, then spawns five enemies with a delay between rounds until the number of rounds dictated by spawnRounds is reached
	IEnumerator Spawn (int spawnRounds) {
		yield return new WaitUntil (() => triggerHasEntered == true);
		for (int i = 0; i < spawnRounds; i++) {
			StartCoroutine (SpawnDiamond (5));
			yield return new WaitForSeconds (delay);
		}
	}

	//Spawns a basic diamond enemy above the spawner
	IEnumerator SpawnDiamond (int numberOfDiamonds) {
		spawnParticle.Emit (25);
		Instantiate (enemy, new Vector3 (transform.position.x, transform.position.y + 2, transform.position.z), transform.rotation);
		yield return new WaitForSeconds (.2f);
	}

	//Calculates the health of the spawner as a percentage
	void CalculateHealth () {
		float calcHealth = currSpawnerHealth / maxSpawnerHealth;
		if (calcHealth <= 0) {
			calcHealth = 0;
		}
		SetHealthBar (calcHealth);
	}

	//Takes the calculated health percentage of the spawner and sets the spawner's health bar's length to be equal to the percentage
	public void SetHealthBar (float myHealth) {
		if (spawnerHealthBar != null) {
			spawnerHealthBar.transform.localScale = new Vector3 (myHealth, spawnerHealthBar.transform.localScale.y, spawnerHealthBar.transform.localScale.z);
		}
	}

	//Spawner's death coroutine. Runs if the spawner's health reaches zero
	IEnumerator SpawnerDeath () {
		//Stops the spawn coroutine
		StopCoroutine (co);
		//Adds the score value of the spawner to the game manager's score count
		gameManager.GetComponent<GameManager> ().killCount += scoreValue;
		//Removes the spawner from the game manager's enemy count
		gameManager.GetComponent<GameManager> ().enemyCount -= 1;
		//Destroys the spawner's health bar 
		Destroy (spawnerHealthBar);
		//Begins playing the aura particle that indicates the spawner has died and waits for it to play out
		preDeathParticle.Play ();
		yield return new WaitForSeconds (3);
		//Stops the aura particle and pauses before the spawner explodes
		preDeathParticle.Stop ();
		yield return new WaitForSeconds (1);
		//Disables the spawner's mesh renderer to add to the illusion that it has exploded
		this.gameObject.GetComponent<MeshRenderer> ().enabled = false;
		//For good measure, the entire mesh renderer component is destroyed as it is no longer needed
		Destroy (this.gameObject.GetComponent<MeshCollider>());
		//Emits 400 particles in a burst to simulate the spawner exploding and waits 3 seconds before destroying the spawner
		deathParticle.Emit (400);
		yield return new WaitForSeconds (3);
		Destroy (this.gameObject);
	}
}
