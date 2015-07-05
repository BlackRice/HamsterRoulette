using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CircularProgressBar:MaskableGraphic {
	[SerializeField]
	private float _startAngle = 30;

	[SerializeField]
	private float _endAngle = -30;

	[SerializeField]
	private float _outerRadius = 5;

	[SerializeField]
	private float _innerRadius = 4;

	[SerializeField]
	private int _segments = 30;

	[SerializeField]
	private Sprite _sprite;

	[SerializeField]
	[Range(0, 1)]
	private float _value = 0;
	
	private float rangeLength {
		get {
			return Mathf.Abs(startAngle-endAngle);
		}
	}


	public override Texture mainTexture
	{
		get
		{
			if (!sprite || !sprite.texture)
			{
				return s_WhiteTexture;
			}
			return sprite.texture;
		}
	}

	public float startAngle
	{
		get
		{
			return _startAngle;
		}
		set
		{
			if (_startAngle == value)
				return;

			_startAngle = value;
			SetVerticesDirty();
		}
	}

	public float endAngle
	{
		get
		{
			return _endAngle;
		}
		set
		{
			if (_endAngle == value)
				return;

			_endAngle = value;
			SetVerticesDirty();
		}
	}

	public float outerRadius
	{
		get
		{
			return _outerRadius;
		}
		set
		{
			if (_outerRadius == value)
				return;

			_outerRadius = value;
			SetVerticesDirty();
		}
	}

	public float innerRadius
	{
		get
		{
			return _innerRadius;
		}
		set
		{
			if (_innerRadius == value)
				return;

			_innerRadius = value;
			SetVerticesDirty();
		}
	}

	public int segments
	{
		get
		{
			return _segments;
		}
		set
		{
			value = Mathf.Clamp(value, 0, 360);
			if (_segments == value)
				return;

			_segments = value;
			SetVerticesDirty();
		}
	}

	public Sprite sprite
	{
		get
		{
			return _sprite;
		}
		set
		{
			if (_sprite == value)
				return;

			_sprite = value;
			SetVerticesDirty();
			SetMaterialDirty();
		}
	}

	public float value
	{
		get
		{
			return _value;
		}
		set
		{
			value = Mathf.Clamp01(value);
			if (_value == value)
				return;

			_value = value;
			SetVerticesDirty();
		}
	}

	protected override void OnFillVBO(List<UIVertex> vbo)
	{
		vbo.Clear();

		float xUVStart = 0;
		float xUVEnd = 1;

		float yUVStart = 0;
		float yUVEnd = 1;

		if (sprite) {
			xUVStart = sprite.border.x/sprite.rect.width;
			xUVEnd = 1.0f-sprite.border.z/sprite.rect.width;

			yUVStart = sprite.border.y/sprite.rect.height;
			yUVEnd = 1.0f-sprite.border.w/sprite.rect.height;
		}

		UIVertex vert = UIVertex.simpleVert;

		float valueEndAngle = Mathf.Lerp(startAngle, endAngle, value);

		int actualSegments = Mathf.Max(Mathf.RoundToInt(segments*value), 1);

		for (int i = 0; i < actualSegments; i++) {
			float ratio = ((float)i)/actualSegments;
			float angle = Mathf.LerpAngle(startAngle, valueEndAngle, ratio);

			float nextRatio = ((float)(i+1))/actualSegments;
			float nextAngle = Mathf.LerpAngle(startAngle, valueEndAngle, nextRatio);

			float xUV = Mathf.Lerp(xUVStart, xUVEnd, ratio);
			float nextXUV = Mathf.Lerp(xUVStart, xUVEnd, nextRatio);

			Vector2 direction = AngleToDirection(angle);
			Vector2 nextDirection = AngleToDirection(nextAngle);

			vert.position = direction*innerRadius;
			vert.uv0 = new Vector2(xUV, 0);
			vert.color = color;
			vbo.Add(vert);

			vert.position = direction*outerRadius;
			vert.uv0 = new Vector2(xUV, 1);
			vert.color = color;
			vbo.Add(vert);

			vert.position = nextDirection*outerRadius;
			vert.uv0 = new Vector2(nextXUV, 1);
			vert.color = color;
			vbo.Add(vert);

			vert.position = nextDirection*innerRadius;
			vert.uv0 = new Vector2(nextXUV, 0);
			vert.color = color;
			vbo.Add(vert);
		}
	}

	private Vector2 AngleToDirection(float angle) {
		Quaternion q = Quaternion.Euler(0, 0, angle);
		Vector3 vector = q*Vector3.up;
		return vector;
	}
}
