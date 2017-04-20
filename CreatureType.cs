using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureType : MonoBehaviour {

	public Transform activeMate;

	public List<Transform> potentialMatesInRange = new List<Transform>();
	[Header("Creature Properties")]
	public string myCreatureName;
	public CreatureSpecies myCreatureSpecies;
	public CreatureFamilies myCreatureFamily;
	public CreatureElement myCreatureElement;
	public CreatureGenus myCreatureGenus;
	public CreatureFood myCreatureFood;
	public CreatureSpecies[] possibleMates;
	public CreatureGender myCreatureGender;
	public GameObject[] possibleEggs;

	[Header("Creature Stats")]
	public float creatureHealth;
	public float maxCreatureHealth;
	public float creatureHappiness;
	public float maxCreatureHappiness;
	public float requiredBreedPercentage = .75f;
	public float rechargeTime;
	public float timeToBreed;

	float breedTimer;
	bool readyToBreed;
	bool itsTimeToBreed;

	NavMeshAgent myNav;

	void Start () {
		gameObject.name = myCreatureSpecies.ToString();
		creatureHealth = maxCreatureHealth;
		//creatureHappiness = maxCreatureHappiness;
		breedTimer = rechargeTime;
		myNav = GetComponent<NavMeshAgent> ();
		StartCoroutine (Breed ());
	}

	void Update () {
		breedTimer += Time.deltaTime;
		if (creatureHappiness >= maxCreatureHappiness * requiredBreedPercentage && breedTimer >= rechargeTime) {
			readyToBreed = true;
			activeMate = FindAMate (potentialMatesInRange);
		} else {
			readyToBreed = false;
			activeMate = null;
		}
		if (readyToBreed == true && activeMate != null) {
			itsTimeToBreed = true;
			myNav.SetDestination (activeMate.transform.position);
		} else {
			itsTimeToBreed = false;
		}
	}

	IEnumerator Breed () {
		yield return new WaitUntil (() => itsTimeToBreed == true);
		GetComponent<CreatureWander> ().enabled = false;
		myNav.SetDestination (activeMate.transform.position);
		yield return new WaitForSeconds (timeToBreed);
		if (myCreatureGender == CreatureGender.Female) {
			Vector3 eggPosition = Vector3.Lerp (transform.position, activeMate.transform.position, .5f);
			//GameObject.Instantiate (testEgg, eggPosition, Quaternion.identity);
			Debug.Log ("BIRTH");
		}
		readyToBreed = false;
		itsTimeToBreed = false;
		breedTimer = 0;
		GetComponent<CreatureWander> ().enabled = true;
	}

	Transform FindAMate (List<Transform> possibleMates) {
		Transform bestMate = null;
		float closestDistanceSqr = Mathf.Infinity;
		Vector3 currentPosition = transform.position;
		foreach (Transform potentialMate in potentialMatesInRange) {
			Vector3 directionToTarget = potentialMate.position - currentPosition;
			float distanceSqrToTarget = directionToTarget.sqrMagnitude;
			if (distanceSqrToTarget < closestDistanceSqr && potentialMate.gameObject.GetComponent<CreatureType>().readyToBreed == true && potentialMate.gameObject.GetComponent<CreatureType>().myCreatureGender != myCreatureGender) {
				closestDistanceSqr = distanceSqrToTarget;
				bestMate = potentialMate;
			}
		}

		return bestMate;
	}

	void OnTriggerEnter (Collider collider) {
		if (collider.gameObject.tag == "Creature") {
			if (System.Array.IndexOf (possibleMates, collider.gameObject.GetComponent<CreatureType> ().myCreatureSpecies) != -1 && collider.gameObject.GetComponent<CreatureType>().readyToBreed == true) {
				potentialMatesInRange.Add (collider.transform);
			}
		}
	}

	void OnTriggerExit (Collider collider) {
		if (collider.gameObject.tag == "Creature") {
			if (potentialMatesInRange.Contains(collider.transform)) {
				potentialMatesInRange.Remove (collider.transform);
			}
		}
	}
}

//Enumerations for different creature properties
public enum CreatureElement {
	Fire,
	Earth,
	Water,
	Air,
	Death,
	Life,
	Meat,
	Ice,
	Ghost,
	Space,
	Electric,
	NonElemental
}

public enum CreatureFood {
	Any,
	Meat,
	Vegetables,
	Fruit
}

public enum CreatureGenus {
	Normal,
	Flying,
	Nocturnal
}

public enum CreatureSpecies {
	BooperDoop,
	SneedleDoop,
	FleepSnop
}

public enum CreatureFamilies {
	TheFleepSnops
}

public enum CreatureGender {
	Male,
	Female
}
