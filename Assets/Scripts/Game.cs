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

	public GameUI gameUI;
	public WinUI winUI;
	public LoseUI loseUI;

	public Color[] playerColors;

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

	void Awake() {
		Application.targetFrameRate = 60;
		StartGame();
	}

	private void ClearUI()
	{
		winUI.gameObject.SetActive(false);
		loseUI.gameObject.SetActive(false);
	}

	private void Clear()
	{
		foreach (var hamster in hamsters) {
			DestroyImmediate(hamster.gameObject);
		}
		hamsters.Clear();
	}

	public void StartGame()
	{
		Clear();

		if (playerCount < 1) {
			throw new System.Exception("playerCount must be higher than 0.");
		}

		Hamster hamsterPrefab = Resources.Load<Hamster>("Hamsters/Hamster");

		for (int i = 0; i < playerCount; i++) {
			Hamster hamster = (Hamster)Instantiate(hamsterPrefab);
			hamsters.Add(hamster);
			hamster.transform.position = Wheel.current.spinningTransform.position;
			hamster.transform.rotation = Wheel.current.spinningTransform.rotation;
			//float offset = 0.5f+(float)i*0.4f;
			//hamster.transform.Translate(offset, 0, 0);
			//hamster.desiredDistanceFromCenter = offset;
			//hamster.distanceFromCenter = 0.5f+(float)i*0.4f;
		}

		hamsters[0].target = hamsters[1];
		hamsters[1].target = hamsters[0];

		playerHamster.gameObject.AddComponent<PlayerController>();
		hamsters[1].gameObject.AddComponent<AIController>();

		ClearUI();
	}

	public void OnHamsterDie(Hamster hamster)
	{
		if (hamster != playerHamster)
		{
			Win();
		}
		else
		{
			Lose();
		}
	}

	private void Win()
	{
		ClearUI();

		winUI.gameObject.SetActive(true);
	}

	private void Lose()
	{
		ClearUI();

		loseUI.gameObject.SetActive(true);
	}

	public Color GetHamsterColor(Hamster hamster)
	{
		Color color = Color.white;
		int index = hamsters.IndexOf(hamster);
		if (index != -1)
		{
			color = playerColors[index];
		}
		return color;
	}
}
