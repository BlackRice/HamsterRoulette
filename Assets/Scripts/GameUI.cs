using UnityEngine;
using System.Collections;

public class GameUI:MonoBehaviour {
	public CircularProgressBar hpBar;
	public CircularProgressBar otherHPBar;
	public CircularProgressBar mpBar;
	void Update() {
		Hamster hamster = Game.current.playerHamster;
		Hamster otherHamster = Game.current.hamsters[1];
		
		hpBar.value = hamster.hp/hamster.maxHP;
		otherHPBar.value = otherHamster.hp/otherHamster.maxHP;
		mpBar.value = hamster.mp/hamster.maxMP;
	}
}
