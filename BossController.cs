using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour {

	public Material[] myMaterials;								//Reference to an array of materials

	public float maxBossHealth = 1000f;							//Sets the boss's maximum health, defaults to 1000
	public float currBossHealth;								//Boss's current health

	public GameObject beamRotation;								//GameObject reference to the pivot of the beam attack's rotation
	public GameObject waveRotation;								//GameObject reference to the pivot of the wave attack's rotation
	public GameObject aoeStartUpParticleGameObject;				//GameObject reference to the particle effect indicating the beginning of the AOE attack
	public GameObject aoeParticleGameObject;					//GameObject reference to the particle effect of the AOE attack
	public GameObject beamParticleGameObject;					//GameObject reference to the particle effect of the beam attack
	public GameObject waveParticleGameObject;					//GameObject reference to the particle effect of the wave attack
	public GameObject bossHealthBar;							//GameObject reference to the boss's health bar

	public ParticleSystem aoeStartUpParticle;					//Reference to the particle system inside the aoeStartUpParticleGameObject
	public ParticleSystem aoeParticle;							//Reference to the particle system inside the aoeParticleGameObject
	public ParticleSystem beamParticle;							//Reference to the particle system inside the beamParticleGameObject
	public ParticleSystem waveParticle;							//Reference to the particle system inside the waveParticleGameObject;
	public ParticleSystem laserParticle;						//Reference to the particle system used to indicate the beam attack's targeting

	public AudioSource beamAudio;								//Reference to the audio for the beam attack
		
	private int arrayPos;										//Used to indicate the current position inside the myMaterials array

	void Start () {
		//Sets all necessary GameObject references to the appropriate GameObjects
		beamRotation = GameObject.FindGameObjectWithTag ("BeamRotation");
		waveRotation = GameObject.FindGameObjectWithTag ("WaveRotation");
		aoeStartUpParticleGameObject = GameObject.FindGameObjectWithTag ("AOEStart");
		aoeParticleGameObject = GameObject.FindGameObjectWithTag ("AOEParticle");
		beamParticleGameObject = GameObject.FindGameObjectWithTag ("BeamParticle");
		waveParticleGameObject = GameObject.FindGameObjectWithTag ("WaveParticle");
		bossHealthBar = GameObject.FindGameObjectWithTag ("BossHealth");

		//Sets all necessary ParticleSystem references to the appropriate ParticleSystems
		aoeStartUpParticle = aoeStartUpParticleGameObject.GetComponent<ParticleSystem> ();
		aoeParticle = aoeParticleGameObject.GetComponent<ParticleSystem>();
		beamParticle = beamParticleGameObject.GetComponent<ParticleSystem>();
		waveParticle = waveParticleGameObject.GetComponent<ParticleSystem>();
		laserParticle = beamRotation.GetComponent<ParticleSystem> ();

		//Sets all necessary AudioSource references to the appropriate AudioSources
		beamAudio = beamParticleGameObject.GetComponent<AudioSource> ();

		//Sets the boss's health at full
		currBossHealth = maxBossHealth;
		//Sets the boss's material to the default material
		arrayPos = 0;
	}

	void Update () {
		//USED FOR TESTING, BOSS AI HAS NOT BEEN IMPLEMENTED
		if (Input.GetKeyDown(KeyCode.F)) {
			StartCoroutine (BeamAttack ());
		}
		//Calls the function to calculate the boss's health
		CalculateBossHealth ();
	}

	//Boss's beam attack coroutine
	IEnumerator BeamAttack () {
		//Plays the tagerting laser's particle system
		laserParticle.Play ();
		//Enables the script allowing for the targeting laser to rotate towards the player and waits while it reaches the player's position
		beamRotation.GetComponent<RotateToFacePlayer> ().enabled = true;
		yield return new WaitForSeconds (3);
		//Freezes the laser in place and pauses to give the player a chance to get out of the way
		beamRotation.GetComponent<RotateToFacePlayer> ().enabled = false;
		yield return new WaitForSeconds (1);
		//Disables the laser targeting particle system and activates the actual attack, playing both the beam particle system and its audio effect and allowing them to play for a few seconds
		laserParticle.Stop ();
		beamParticle.Play ();
		beamAudio.Play ();
		yield return new WaitForSeconds (5);
		//Disables the beam particle system and its audio and ends the coroutine
		beamAudio.Stop ();
		beamParticle.Stop ();

	}

	//Boss's AOE attack coroutine
	IEnumerator AOEAttack () {
		//Plays the pre-AOE particle system and pauses to telegraph the attack to the player, then stops the pre-AOE particle system and activates the damaging attack particle system
		aoeStartUpParticle.Play ();
		yield return new WaitForSeconds (1);
		aoeStartUpParticle.Stop ();
		aoeParticle.Play ();
	}

	//Yet to be implemented
	IEnumerator WaveAttack () {
		yield return new WaitForSeconds (1);
	}

    //Navigates the myMaterials array to change the material of the boss if it is hit, then turn it back again
	public IEnumerator StartHitTexture () {
		arrayPos++;
		arrayPos %= myMaterials.Length;
		if (arrayPos > myMaterials.Length) {
			arrayPos = 0;
		}
		GetComponent<MeshRenderer> ().sharedMaterial = myMaterials [arrayPos];
		yield return new WaitForSeconds (.1f);
		arrayPos++;
		arrayPos %= myMaterials.Length;
		if (arrayPos > myMaterials.Length) {
			arrayPos = 0;
		}
		GetComponent<MeshRenderer> ().sharedMaterial = myMaterials [arrayPos];
	}

    //Calcultes the boss's health by dividing it by its maximum health, effectively making it a percentage for the purposes of setting the boss's health bar
	void CalculateBossHealth () {
		float calcHealth = currBossHealth / maxBossHealth;
		if (calcHealth <= 0) {
			calcHealth = 0;
		}
		SetHealthBar (calcHealth);
	}

    //Sets the boss's health bar to the proper length relative to its health. Takes a float value as a parameter
	public void SetHealthBar (float myHealth) {
		bossHealthBar.transform.localScale = new Vector3 (myHealth, bossHealthBar.transform.localScale.y, bossHealthBar.transform.localScale.z);
	}
}
