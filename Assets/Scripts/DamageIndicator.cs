using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageIndicator : MonoBehaviour {
	public Text text;

	private int _damage = 0;
	public int damage
	{
		get
		{
			return _damage;
		}
		set
		{
			_damage = value;
			text.text = _damage.ToString();
		}
	}

	void Start()
	{
		Destroy(gameObject, 1);
	}

	void Update()
	{
		transform.LookAt(Camera.main.transform.position, Camera.main.transform.up);
		transform.Rotate(0, 180, 0);
	}
}
