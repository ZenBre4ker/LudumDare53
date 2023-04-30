using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelEditor : MonoBehaviour
{
	private Camera mainCamera;
	private Plane mainPlane;

	private GameObject activeHitObject;
	private Vector3 hitOffset;
	private bool isStarted = false;
	private void Start()
	{
		mainPlane = new Plane(Vector3.zero, Vector3.right, Vector3.up);
		
		mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		PlayerManager.onUserFire += OnFire;
		SceneManager.sceneLoaded += (arg0, mode) =>
		{
			isStarted = false;
			mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
			PlayerManager.onUserFire += OnFire;
		};
		
		
		LevelManager.onLevelStarted += () =>
		{
			isStarted = true;
		};
		LevelManager.onLevelStopped += () =>
		{
			isStarted = false;
		};
		LevelManager.onLevelTested += () =>
		{
			isStarted = true;
		};

	}

	public void OnFire()
	{
		Vector3 mousePosition = Mouse.current.position.ReadValue();

		if (Physics.Raycast(mainCamera.ScreenPointToRay(mousePosition), out RaycastHit hitInfo) && hitInfo.collider.gameObject.CompareTag("Editable"))
		{
			Debug.Log(hitInfo.collider.gameObject.name);
			activeHitObject = hitInfo.collider.gameObject;
			hitOffset = activeHitObject.transform.position - hitInfo.point;
			hitOffset.z = 0;
		}
		else
		{
			activeHitObject = null;
		}

	}

	private void Update()
	{
		if (Mouse.current.leftButton.wasReleasedThisFrame)
		{
			activeHitObject = null;
		}

		if (isStarted) return;
		
		if (activeHitObject != null && Mouse.current.leftButton.isPressed)
		{
			Vector3 mousePosition = Mouse.current.position.ReadValue();
			Ray ray = mainCamera.ScreenPointToRay(mousePosition);
			mainPlane.Raycast(ray, out float distance);

			int mouseScroll = Math.Sign(Mouse.current.scroll.y.ReadValue());

			if (mouseScroll != 0)
			{
				Vector3 inverseHitOffset = activeHitObject.transform.InverseTransformDirection(hitOffset);
				activeHitObject.transform.Rotate(Vector3.forward, mouseScroll * 7.5f);
				hitOffset = activeHitObject.transform.TransformDirection(inverseHitOffset);
			}
			
			activeHitObject.transform.position = ray.GetPoint(distance) + hitOffset;
		}
	}
}
