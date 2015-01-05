using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game:MonoBehaviour {
	static private Game _current;
	static public Game current {
		get {
			if (!_current) {
				_current = FindObjectOfType<Game>();
			}
			return _current;
		}
	}

	public int playerCount = 2;
	public List<Hamster> hamsters = new List<Hamster>();
	public Hamster playerHamster {
		get {
			if (hamsters.Count == 0) {
				return null;
			}
			return hamsters[0];
		}
	}

	void Start() {
		if (playerCount < 1) {
			throw new System.Exception("playerCount must be higher than 0.");
		}
		Hamster hamsterPrefab = Resources.Load<Hamster>("Hamsters/Hamster");

		for (int i = 0; i < playerCount; i++) {
			Hamster hamster = (Hamster)Instantiate(hamsterPrefab);
			hamsters.Add(hamster);
			hamster.distanceFromCenter = 0.6f+(float)i*0.3f;
		}

		playerHamster.gameObject.AddComponent<PlayerController>();
	}
}
