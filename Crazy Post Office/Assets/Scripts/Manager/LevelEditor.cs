using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelEditor : MonoBehaviour
{
	private Camera mainCamera;
	private Plane mainPlane;

	private GameObject activeHitObject;
	private void Start()
	{
		mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		mainPlane = new Plane(Vector3.zero, Vector3.right, Vector3.up);
		PlayerManager.onUserFire += OnFire;
	}

	public void OnFire()
	{
		Vector3 mousePosition = Mouse.current.position.ReadValue();

		if (Physics.Raycast(mainCamera.ScreenPointToRay(mousePosition), out RaycastHit hitInfo) && hitInfo.collider.gameObject.CompareTag("Editable"))
		{
			Debug.Log(hitInfo.collider.gameObject.name);
			activeHitObject = hitInfo.collider.gameObject;
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
		
		if (activeHitObject != null && Mouse.current.leftButton.isPressed)
		{
			Vector3 mousePosition = Mouse.current.position.ReadValue();
			Ray ray = mainCamera.ScreenPointToRay(mousePosition);
			mainPlane.Raycast(ray, out float distance);

			activeHitObject.transform.position = ray.GetPoint(distance);

			int mouseScroll = Math.Sign(Mouse.current.scroll.y.ReadValue());
				
			activeHitObject.transform.Rotate(Vector3.forward, mouseScroll * 7.5f);
		}
	}
}
