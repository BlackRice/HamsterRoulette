using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CircularProgressBarText:MonoBehaviour {
	public CircularProgressBar progressBar;

	private Text _text;
	public Text text {
		get {
			if (!_text) {
				_text = gameObject.GetComponent<Text>();
			}
			return _text;
		}
	}

	void Update() {
		if (!progressBar) {
			return;
		}
		transform.rotation = Camera.main.transform.rotation;
		transform.position = progressBar.transform.TransformPoint(progressBar.CalculateLocalEndPosition());
	}
}
