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
	
	private Vector3 oldPosition;
	private Quaternion oldRotation;
	
	private bool isStarted = false;

	private List<Collider> boundsList;
	private void Start()
	{
		boundsList = new List<Collider>();
		mainPlane = new Plane(Vector3.zero, Vector3.right, Vector3.up);
		
		mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		PlayerManager.onUserFire += OnFire;
		SceneManager.sceneLoaded += (arg0, mode) =>
		{
			isStarted = false;
			mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
			PlayerManager.onUserFire += OnFire;
			
			GameObject[] allObjects = FindObjectsOfType<GameObject>();
			
			boundsList = new List<Collider>();
			foreach(GameObject go in allObjects)
			{
				if (go.activeInHierarchy && go.TryGetComponent(out Collider collider))
				{
					boundsList.Add(collider);
				}
			}
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
			
			oldPosition = activeHitObject.transform.position;
			oldRotation = activeHitObject.transform.rotation;
		}
		else
		{
			activeHitObject = null;
		}

	}

	private void Update()
	{
		if (activeHitObject != null && Mouse.current.leftButton.wasReleasedThisFrame)
		{
			Collider[] activeColliders = activeHitObject.GetComponentsInChildren<Collider>();

			foreach (Collider collider in boundsList)
			{
				bool hasCollision = false;
				foreach (Collider activeCollider in activeColliders)
				{
					if (collider == activeCollider)
					{
						hasCollision = true;
						break;
					}
				}

				if (hasCollision) continue;
				
				foreach (Collider activeCollider in activeColliders)
				{
					if (activeCollider.bounds.Intersects(collider.bounds))
					{
						activeHitObject.transform.position = oldPosition;
						activeHitObject.transform.rotation = oldRotation;
					
						Debug.Log($"Collided with {collider.gameObject.name}");
						hasCollision = true;
						break;
					}
				}

				if (hasCollision) break;
			}
			
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
