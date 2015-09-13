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

	public float leftCapWidth = 0;
	public float rightCapWidth = 0;

	public int floorSegments = 0;

	public bool tile = false;

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
			float v = _value;
			if (floorSegments > 0)
			{
				v = Mathf.Floor(v*floorSegments)/floorSegments;
			}
			return v;
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

	protected override void OnPopulateMesh(Mesh m)
	{
		base.OnPopulateMesh(m);

		m.Clear();

		List<UIVertex> vbo = new List<UIVertex>();

		float xUVStart = 0;
		float xUVEnd = 1;

		//float yUVStart = 0;
		//float yUVEnd = 1;

		if (sprite) {
			xUVStart = sprite.border.x/sprite.rect.width;
			xUVEnd = 1.0f-sprite.border.z/sprite.rect.width;

			//yUVStart = sprite.border.y/sprite.rect.height;
			//yUVEnd = 1.0f-sprite.border.w/sprite.rect.height;
		}

		float valueEndAngle = Mathf.Lerp(startAngle, endAngle, value);

		int actualSegments = Mathf.Max(Mathf.CeilToInt(segments*value), 1);

		float segmentLength = rangeLength/segments;

		if (leftCapWidth > 0) {
			CreateSegmentQuad(vbo, startAngle+leftCapWidth, startAngle, 0, xUVStart);
		}

		for (int i = 0; i < actualSegments; i++) {
			float ratio = (((float)i)*segmentLength)/rangeLength;

			float angle = Mathf.LerpAngle(startAngle, endAngle, ratio);

			float nextRatio = (((float)(i+1))*segmentLength)/rangeLength;
			float nextAngle = Mathf.LerpAngle(startAngle, endAngle, nextRatio);
			if (i >= actualSegments-1) {
				nextAngle = valueEndAngle;
			}

			float xUV = xUVStart;
			float nextXUV = xUVEnd;

			if (!tile) {
				xUV = Mathf.Lerp(xUVStart, xUVEnd, ratio);
				nextXUV = Mathf.Lerp(xUVStart, xUVEnd, nextRatio);
			}

			CreateSegmentQuad(vbo, angle, nextAngle, xUV, nextXUV);
		}

		if (rightCapWidth > 0) {
			CreateSegmentQuad(vbo, valueEndAngle, valueEndAngle-rightCapWidth, xUVEnd, 1);
		}

		Vector3[] vertices = new Vector3[vbo.Count];
		Vector2[] uvs = new Vector2[vertices.Length];
		//Vector3[] normals = new Vector3[vertices.Length];
		Color32[] colors = new Color32[vertices.Length];
		int[] triangles = new int[vertices.Length/4*2*3];

		for (int i = 0; i < vertices.Length; i++) {
			UIVertex uiv = vbo[i];
			vertices[i] = uiv.position;
			uvs[i] = uiv.uv0;
			//normals[i] = uiv.normal;
			colors[i] = uiv.color;
		}

		for (int i = 0; i < vertices.Length; i += 4) {
			triangles[i/4*2*3+0] = i+0;
			triangles[i/4*2*3+1] = i+1;
			triangles[i/4*2*3+2] = i+2;

			triangles[i/4*2*3+3] = i+2;
			triangles[i/4*2*3+4] = i+3;
			triangles[i/4*2*3+5] = i+0;
		}

		m.vertices = vertices;
		m.uv = uvs;
		//m.normals = normals;
		m.colors32 = colors;
		m.triangles = triangles;
	}

	private void CreateSegmentQuad(List<UIVertex> vertices, float angle0, float angle1, float uvX0, float uvX1) {
		Vector2 direction = AngleToDirection(angle0);
		Vector2 nextDirection = AngleToDirection(angle1);

		UIVertex vert = UIVertex.simpleVert;

		vert.position = direction*innerRadius;
		vert.uv0 = new Vector2(uvX0, 0);
		vert.color = color;
		vertices.Add(vert);

		vert.position = direction*outerRadius;
		vert.uv0 = new Vector2(uvX0, 1);
		vert.color = color;
		vertices.Add(vert);

		vert.position = nextDirection*outerRadius;
		vert.uv0 = new Vector2(uvX1, 1);
		vert.color = color;
		vertices.Add(vert);

		vert.position = nextDirection*innerRadius;
		vert.uv0 = new Vector2(uvX1, 0);
		vert.color = color;
		vertices.Add(vert);
	}

	private Vector2 AngleToDirection(float angle) {
		Quaternion q = Quaternion.Euler(0, 0, angle);
		Vector3 vector = q*Vector3.up;
		return vector;
	}

	public Vector2 CalculateLocalEndPosition() {
		float valueEndAngle = Mathf.Lerp(startAngle, endAngle, value);
		Vector2 direction = AngleToDirection(valueEndAngle);
		Vector2 position = direction*((innerRadius+outerRadius)/2);
		return position;
	}
}
