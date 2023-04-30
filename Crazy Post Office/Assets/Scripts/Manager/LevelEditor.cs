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

	private bool[] triggeredColliders;
	private bool[] wasTrigger;
	private Collider[] colliderList;
	private Trigger[] triggerList;
	private Trigger.ObjectGotTriggeredDelegate[] myTriggerDelegates;

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

	private void colliderGotTriggered(int colliderNumber, Trigger.TriggerInfo info)
	{
		foreach (Collider collider in colliderList)
		{
			if (info.detectedCollider == collider) return;
		}
		
		if (info.isEntered)
		{
			triggeredColliders[colliderNumber] = true;
		}

		if (info.isExited)
		{
			triggeredColliders[colliderNumber] = false;
		}
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

			colliderList = hitInfo.collider.gameObject.GetComponentsInChildren<Collider>();
			triggeredColliders = new bool[colliderList.Length];
			triggerList = new Trigger[colliderList.Length];
			wasTrigger = new bool[colliderList.Length];
			myTriggerDelegates = new Trigger.ObjectGotTriggeredDelegate[colliderList.Length];
			
			for(int i=0; i<triggeredColliders.Length; i++ )
			{
				int myNumber = i;
				Collider collider = colliderList[myNumber];
				GameObject go = collider.gameObject;
				if (!go.TryGetComponent(out Trigger trigger))
				{
					trigger = go.AddComponent<Trigger>();
				};

				triggerList[myNumber] = trigger;

				void objectGotTriggered(Trigger.TriggerInfo info)
				{
					Debug.Log($"{go.name} was triggered.");
					colliderGotTriggered(myNumber, info);
				}

				trigger.objectGotTriggered += objectGotTriggered;

				myTriggerDelegates[myNumber] = objectGotTriggered;
				wasTrigger[myNumber] = collider.isTrigger;
				collider.isTrigger = true;
			}
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
			bool detectedCollision = false;
			for (int i=0; i< triggeredColliders.Length; i++)
			{
				detectedCollision |= triggeredColliders[i];
				triggerList[i].objectGotTriggered -= myTriggerDelegates[i];
				colliderList[i].isTrigger = wasTrigger[i];
			}
			
			if(detectedCollision){
				activeHitObject.transform.position = oldPosition;
				activeHitObject.transform.rotation = oldRotation;
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
