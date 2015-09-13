using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameUI:MonoBehaviour {
	public CircularProgressBar hpBar;
	public CircularProgressBar otherHPBar;
	public CircularProgressBar mpBar;

	public CircularProgressBarText hpBarText;
	public CircularProgressBarText otherHPBarText;
	public CircularProgressBarText mpBarText;

	public RectTransform alignmentIndicatorPivot;

	public Button primaryAttackButton;
	public Image primaryAttackFillImage;
	public Image primaryAttackGlow;

	public Button secondaryAttackButton;
	public Image secondaryAttackFillImage;
	public Image secondaryAttackGlow;

	public Button superAttackButton;
	public Image superAttackFillImage;
	public Image superAttackGlow;

	private List<RectTransform> alignmentIndicators = new List<RectTransform>();
	void Start() {
		for (int i = 0; i < Game.current.hamsters.Count; i++)
		{
			Hamster hamster = Game.current.hamsters[i];
			RectTransform alignmentIndicator = Instantiate(hamster.positionIndicatorPrefab);
			Image image = alignmentIndicator.gameObject.GetComponent<Image>();
			if (image)
			{
				image.color = Game.current.GetHamsterColor(hamster);
			}
			alignmentIndicator.SetParent(alignmentIndicatorPivot, false);
			alignmentIndicators.Add(alignmentIndicator);
		}
		
		primaryAttackButton.onClick.AddListener(OnClickPrimaryAttackButton);
		secondaryAttackButton.onClick.AddListener(OnClickSecondaryAttackButton);
		superAttackButton.onClick.AddListener(OnClickSuperAttackButton);
	}
	
	void Update() {
		Hamster playerHamster = Game.current.playerHamster;
		Hamster otherHamster = Game.current.hamsters[1];
	
		hpBar.value = playerHamster.hp/playerHamster.maxHP;
		otherHPBar.value = otherHamster.hp/otherHamster.maxHP;
		mpBar.value = playerHamster.mp/playerHamster.maxMP;

		hpBarText.text.text = playerHamster.hp.ToString("0");
		otherHPBarText.text.text = otherHamster.hp.ToString("0");
		mpBarText.text.text = playerHamster.mp.ToString("0");

		for (int i = 0; i < Game.current.hamsters.Count; i++)
		{
			RectTransform alignmentIndicator = alignmentIndicators[i];
			Hamster hamster = Game.current.hamsters[i];
			alignmentIndicator.localEulerAngles = new Vector3(0, 0, -hamster.positionOnWheel);
		}

		primaryAttackFillImage.fillAmount = playerHamster.primaryAttack.coolDown;
		secondaryAttackFillImage.fillAmount = playerHamster.secondaryAttack.coolDown;
		superAttackFillImage.fillAmount = playerHamster.superCharge;

		primaryAttackGlow.gameObject.SetActive(playerHamster.primaryAttack.coolDown >= 1);
		secondaryAttackGlow.gameObject.SetActive(playerHamster.secondaryAttack.coolDown >= 1);
		superAttackGlow.gameObject.SetActive(playerHamster.superCharge >= 1);
	}
	
	void OnClickPrimaryAttackButton() {
		Hamster playerHamster = Game.current.playerHamster;
		playerHamster.DoPrimaryAttack(playerHamster.target);
	}
	
	void OnClickSecondaryAttackButton() {
		Hamster playerHamster = Game.current.playerHamster;
		playerHamster.DoSecondaryAttack(playerHamster.target);
	}

	void OnClickSuperAttackButton() {
		Hamster playerHamster = Game.current.playerHamster;
		playerHamster.DoSuperAttack(playerHamster.target);
	}
}
