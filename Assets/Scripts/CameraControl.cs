using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour 
{
	public float m_dampTime = 0.2f;
	public float m_screenEdgeBuffer = 4f;
	public float m_minSize = 6.5f;
	
	private Camera m_mainCamera;
	private GameObject[] m_player;
	private Vector2 m_desirePosition;
	private Vector2 m_moveVelocity;
	private float m_zoomSpeed;
	
	private void Awake()
	{
		m_mainCamera = GetComponentInChildren<Camera>();
	}
	
	private void Update()
	{
		Move();
		Zoom();
	}

	private void Move()
	{
		FindAvragePosition();
		transform.position = Vector2.SmoothDamp(transform.position, m_desirePosition, ref m_moveVelocity, m_dampTime);
	}

	private void FindAvragePosition()
	{
		m_player = GameObject.FindGameObjectsWithTag("Player");

		Vector3 avragePos = new Vector3();
		int num = 0;

		for (int i = 0; i < m_player.Length; i++)
		{
			avragePos += m_player[i].transform.position;
			num++;
		}

		if (num > 0)
			avragePos /= num;
			
		avragePos.z = transform.position.z;

		m_desirePosition = avragePos;
	}

	private void Zoom()
	{
		float requireSize = FindRequireSize();
		m_mainCamera.orthographicSize = Mathf.SmoothDamp(m_mainCamera.orthographicSize, requireSize, ref m_zoomSpeed, m_dampTime);
	}

	private float FindRequireSize()
	{
		Vector2 desireLocalPos = transform.InverseTransformPoint(m_desirePosition);
		float size = 0f;
		
		for (int i = 0; i < m_player.Length; i++)	
		{
			Vector2 targetLocalPos = transform.InverseTransformPoint(m_player[i].transform.position);

			Vector2 desirePosToTarget = targetLocalPos - desireLocalPos;

			size = Mathf.Max(size, Mathf.Abs(desirePosToTarget.y));
			size = Mathf.Max(size, Mathf.Abs(desirePosToTarget.x / m_mainCamera.aspect));
		}

		size += m_screenEdgeBuffer;
		size = Mathf.Max(size, m_minSize);

		return size;
	}
}
