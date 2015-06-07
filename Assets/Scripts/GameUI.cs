using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameUI:MonoBehaviour {
	public CircularProgressBar hpBar;
	public CircularProgressBar otherHPBar;
	public CircularProgressBar mpBar;
	public RectTransform alignmentIndicatorPivot;
	public Button primaryAttackButton;
	public Button secondaryAttackButton;
	
	private List<RectTransform> alignmentIndicators = new List<RectTransform>();
	void Start() {
		for (int i = 0; i < Game.current.hamsters.Count; i++)
		{
			RectTransform alignmentIndicator = Instantiate(Resources.Load<RectTransform>("UI/Alignment/Indicator"));
			alignmentIndicator.SetParent(alignmentIndicatorPivot, false);
			alignmentIndicators.Add(alignmentIndicator);
		}
		
		primaryAttackButton.onClick.AddListener(OnClickPrimaryAttackButton);
		secondaryAttackButton.onClick.AddListener(OnClickSecondaryAttackButton);
	}
	
	void Update() {
		Hamster playerHamster = Game.current.playerHamster;
		Hamster otherHamster = Game.current.hamsters[1];
		
		hpBar.value = playerHamster.hp/playerHamster.maxHP;
		otherHPBar.value = otherHamster.hp/otherHamster.maxHP;
		mpBar.value = playerHamster.mp/playerHamster.maxMP;
		
		for (int i = 0; i < Game.current.hamsters.Count; i++)
		{
			RectTransform alignmentIndicator = alignmentIndicators[i];
			Hamster hamster = Game.current.hamsters[i];
			alignmentIndicator.localEulerAngles = new Vector3(0, 0, -hamster.positionOnWheel);
		}
	}
	
	void OnClickPrimaryAttackButton() {
		Hamster playerHamster = Game.current.playerHamster;
		playerHamster.DoPrimaryAttack(playerHamster.target);
	}
	
	void OnClickSecondaryAttackButton() {
		Hamster playerHamster = Game.current.playerHamster;
		playerHamster.DoSecondaryAttack(playerHamster.target);
	}
}
