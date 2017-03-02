using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMaster : MonoBehaviour {

    //Player resources variables
	public float playerHealth;                  //Player's current health value
	public float maxPlayerHealth;               //Player's maximum health value
	public float playerStamina;                 //Player's current stamina value
	public float maxPlayerStamina;              //Player's maximum stamina value
	public float playerMana;                    //Player's current mana value
	public float maxPlayerMana;                 //Player's maximum mana value
	public GameObject healthBar;                //Reference to Player's health bar
	public GameObject staminaBar;               //Reference to Player's stamina bar
	public GameObject manaBar;                  //Reference to Player's mana bar

    //Current target variables
	public Image targetHealth;                  //Reference to current target's heatlh bar
	public Image cursor;                        //Reference to player's cursor
	public Sprite[] cursorImages;               //Array of different cursor icons for different target types
	public Text interactText;                   //Reference to the text naming the player's current target
	public float interactRange;                 //The maximum range from the player to a target to be able to interact with it
	RaycastHit hit;                             //RaycastHit for when the player hits something while attacking

    //Weapon and attack variables
	public GameObject equippedWeapon;           //Reference to the player's currently equipped weapon
	public GameObject[] hitMarks;               //Array of hit markers for different materials
	float weaponDamage;                         //The amount of damage the current weapon does to a target
	float weaponSpeed = 1;                      //The speed at which the current weapon can attack
	float weaponRange = 5;                      //The range of the current weapon
	string weaponGenus = "Melee";               //The Genus of the weapon (this classifies weapons as Melee, Bows, or Magic to determine weapon behavior)
	string weaponSpecies;                       //The Species of the weapon (this classifies weapons as Greatsword, Longsword, GreatAxe, Axe, LongBow, Staff, etc to determine specific speed and range attributes)

    //Movement variables
	public float speed;                         //Speed at which the Player moves
	public float jumpForce;                     //Force with which the Player jumps - determines jump height
	public LayerMask heightMask;                //LayerMask to tell the player's capsule collider what is ground and what is not for the purposes of being grounded
	Vector3 movement;                           //Player's movement value stored in a Vector3
	CapsuleCollider myCollider;                 //Reference to the Player's capsule collider
	Rigidbody rb3d;                             //Reference to the Player's rigidbody

	void Start () {
        //Sets the player's resources to full at start
		playerHealth = maxPlayerHealth;
		playerMana = maxPlayerMana;
		playerStamina = maxPlayerStamina;
        //Sets the interact text to blank
		interactText.text = "";
        //Initializes player's collider and rigidbody references
		myCollider = GetComponent<CapsuleCollider> ();
		rb3d = GetComponent<Rigidbody>();
        //Locks cursor the center of the screen
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update () {
        //If ESC is pressed, unlock the cursor
		if (Input.GetButtonDown("Cancel"))
		{
			Cursor.lockState = CursorLockMode.None;
		}

        //Sends out a raycast forward from the player to check if they are looking at something within range that is interactable. If they are, display the target's name and change the cursor 
        //image to the appropriate sprite for the target object. Currently only doors have a dedicated sprite.
		if (Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), out hit, interactRange)) {
			if (hit.collider.gameObject.tag == "Interactable" || hit.collider.gameObject.tag == "Door") {
				interactText.text = hit.collider.gameObject.name;
				cursor.sprite = cursorImages [1];
			} else {
				interactText.text = "";
				cursor.sprite = cursorImages [0];
			}
		} else {
			interactText.text = "";
			cursor.sprite = cursorImages [0];
		}

        //If the left mouse button is clicked, attack. Currently only melee weapons are supported.
		if (Input.GetMouseButtonDown (0) && weaponGenus == "Melee") {
			StartCoroutine (MeleeAttack ());
		}

        //If the enter key is pressed, run the EnterDoor function
		if (Input.GetButtonDown ("Submit")) {
			EnterDoor ();
		}

        //Calls of the player's resource calculation functions
		CalculateHealth ();
		CalculateStamina ();
		CalculateMana ();
	}

    //Player movement is handled inside of FixedUpdate due to the use of Unity's rigidbody physics system
	void FixedUpdate () {
        //Variables to hold the player's movement inputs
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");

        //Calls the MovePlayer function with horizontal and vertical as parameters
		MovePlayer (horizontal, vertical);

        //If the player is grounded and presses the spacebar, call the Jump function
		if (Input.GetButtonDown ("Jump") && isGrounded()) {
			Jump ();
		}
	}

    //Function to calculate the player's health for purposes of setting the player's health bar
	void CalculateHealth () {
        //Divides the player's current health by their maximum possible health, effectively making it a percentage and storing it as calcHealth
		float calcHealth = playerHealth / maxPlayerHealth;

        //calcHealth should not be below 0, so if it is set it back to 0
		if (calcHealth <= 0) {
			calcHealth = 0;
		}

        //Calls SetHealthBar with calcHealth as a parameter
		SetHealthBar (calcHealth);
	}

    //Function to set the player's health bar to the appropriate length. Takes a float value as a parameter
	void SetHealthBar (float myHealth) {
        //Sets the player's health bar to a certain scale based on the player's calculated health percentage
		healthBar.transform.localScale = new Vector3 (myHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
	}

    //Function to calculate the player's health for purposes of setting the player's stamina bar
    void CalculateStamina () {
        //Divides the player's current stamina by their maximum possible stamina, effectively making it a percentage and storing it as calcStam
        float calcStam = playerStamina / maxPlayerStamina;

        //calcStam should not be below 0, so if it is set it back to 0
        if (calcStam <= 0) {
			calcStam = 0;
		}

        //Calls SetStaminaBar with calcStam as a parameter
        SetStaminaBar(calcStam);
	}

    //Function to set the player's stamina bar to the appropriate length. Takes a float value as a parameter
    void SetStaminaBar (float myStamina) {
        //Sets the player's stamina bar to a certain scale based on the player's calculated stamina percentage
        staminaBar.transform.localScale = new Vector3 (myStamina, staminaBar.transform.localScale.y, staminaBar.transform.localScale.z);
	}

    //Function to calculate the player's mana for purposes of setting the player's mana bar
	void CalculateMana () {
        //Divides the player's current mana by their maximum possible stamina, effectively making it a percentage and storing as calcMana
		float calcMana = playerMana / maxPlayerMana;

        //calcMana should not be below 0, so if it is set it back to 0
		if (calcMana <= 0) {
			calcMana = 0;
		}

        //Calls SetManaBar with calcMana as a parameter
		SetManaBar (calcMana);
	}

    //Function to set the player's mana bar to the appropriate length. Takes a float value as a parameter
	void SetManaBar (float myMana) {
        //Sets the player's mana bar to a certain scale based on the player's calculated mana percentage
		manaBar.transform.localScale = new Vector3 (myMana, manaBar.transform.localScale.y, manaBar.transform.localScale.z);
	}

    //Coroutine to handle the player attacking with a melee weapon
	IEnumerator MeleeAttack() {
        //Waits for an amount of time dictated by weaponSpeed before registering the hit on the target
		yield return new WaitForSeconds (weaponSpeed);
        //Casts a ray forward from the player that has a length of weapon speed and returns a hit if there is an object within that range. Currently does nothing except log a message in console
		if (Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), weaponRange)) {
			Debug.Log ("You hit something");
		}
	}

    //Function to handle the player entering a door and changing to another scene
	void EnterDoor () {
        //Sets a RaycastHit and casts a ray that checks if there is an object within interactRange of the player, if that object is interactable, and if it is a door. 
        //If it is, change the scene to the scene dictated by that object
		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), out hit, interactRange) && hit.collider.gameObject.tag == "Door") {
			Debug.Log ("LoadingScene");
			SceneManager.LoadScene (hit.collider.gameObject.GetComponent<SceneTransition> ().mySceneName);
		}
	}

    //Function to handle moving the player. Takes two float values for the horizontal and vertical inputs as parameters
	void MovePlayer (float h, float v) {
        //Sets the movement Vector3 with the horizonal and vertical inputs
		movement.Set (h, 0f, v);
        //Calculates movement in relation to Time.deltaTime and the speed value
		movement = movement.normalized * speed * Time.deltaTime;
        //Sets the movement to be relative to the player's rotation
		movement = transform.TransformDirection (movement);
        //Moves the player's rigidbody in the direction of the movement Vector3
		rb3d.MovePosition (transform.position + movement);
	}
    
    //Function to handle the player jumping
	void Jump () {
        //Adds a force from the player in the up direction multiplied by the jumpForce variable
		rb3d.AddForce(transform.up * jumpForce);
        //Subtracts 10 from the player's stamina
		playerStamina -= 10;
	}

    //Boolean function to determine if the player is grounded. 
	bool isGrounded () {
        //Checks a capsule around the player equal to the player's capsule collider offset down to check if the ground is beneath the player. If it is, isGrounded returns true.
		return Physics.CheckCapsule (myCollider.bounds.center, new Vector3 (myCollider.bounds.center.x, myCollider.bounds.center.y - 1f, myCollider.bounds.center.z), 0.18f, heightMask.value);
	}
}
