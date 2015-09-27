using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WinUI:MonoBehaviour {
	private float startTime;

	void Awake()
	{
		startTime = Time.timeSinceLevelLoad;
	}

	void Update()
	{
		if (Time.timeSinceLevelLoad-startTime >= 1)
		{
			if (Input.anyKeyDown)
			{
				Game.current.StartGame();
			}
		}
	}
}
