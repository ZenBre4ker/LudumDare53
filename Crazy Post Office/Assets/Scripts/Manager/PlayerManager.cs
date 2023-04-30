using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Singleton;
    public CrazyPostOffice playerActions;
    public PlayerInput playerInput;
    public delegate void OnFireDelegate();

    public static OnFireDelegate onUserFire;

    private bool is3D = false;
    
    public Vector3 cameraResetPosition;
    public Quaternion cameraResetRotation;

    public float moveSpeed3D = 1f;
    public float lookSpeed3D = 1f;
    
    public float moveSpeed2D = 1f;
    public float lookSpeed2D = 1f;
    public float zoomSpeed2D = 1f;

    private float minHeight = 1f;
    
    private void Start()
    {
        if (Singleton != null) return;
        Singleton = this;

        playerActions = new CrazyPostOffice();
        playerActions.Enable();
        
        playerInput.camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        cameraResetPosition = playerInput.camera.transform.position;
        cameraResetRotation = playerInput.camera.transform.rotation;

        LimitMaxCameraHeight();
        
        SceneManager.sceneLoaded += (arg0, mode) =>
        {
            playerInput.camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            cameraResetPosition = playerInput.camera.transform.position;
            cameraResetRotation = playerInput.camera.transform.rotation;
            
            LimitMaxCameraHeight();
        };
    }

    public void LimitMaxCameraHeight()
    {
        minHeight = playerInput.camera.orthographicSize - 0.5f;
    }
    
    public void ChangeTo3D(bool enable)
    {
        if (playerInput.camera == null) return;

        is3D = enable;
        playerInput.camera.orthographic = !enable;

        if (enable)
        {
            playerInput.camera.transform.position = cameraResetPosition;
            playerInput.camera.transform.rotation = cameraResetRotation;
        }
        else
        {
            playerInput.camera.transform.position = Vector3.back * 1000 + Vector3.up * 5;
            playerInput.camera.transform.rotation = Quaternion.LookRotation(Vector3.forward);
            
        }
    }

    public void OnFire(InputAction.CallbackContext inputAction)
    {
        if (!inputAction.performed) return;
        
        onUserFire?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext inputAction)
    {
        Vector2 moveVector = inputAction.ReadValue<Vector2>();
        if (is3D)
        {
        }
        else
        {
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        Vector2 moveVector = playerActions.Player.Move.ReadValue<Vector2>() * deltaTime;

        float upDownMovement = playerActions.Player.Jump.IsPressed() ?  deltaTime : 0f;
        upDownMovement += playerActions.Player.Crouch.IsPressed() ? -deltaTime : 0f;

        //Move around
        if (is3D)
        {
            moveVector *= moveSpeed3D;
            upDownMovement *= moveSpeed3D;
            playerInput.camera.transform.position +=
                playerInput.camera.transform.TransformDirection(moveVector.x, 0, moveVector.y);
            playerInput.camera.transform.position += Vector3.up * upDownMovement;
        }
        else
        {
            moveVector *= moveSpeed2D;
            upDownMovement *= moveSpeed2D;
            playerInput.camera.transform.position +=
                playerInput.camera.transform.TransformDirection(moveVector.x, moveVector.y + upDownMovement, 0);
        }

        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * deltaTime;
        
            //Look around
            if (is3D)
            {
                mouseDelta *= lookSpeed3D;
                playerInput.camera.transform.Rotate(Vector3.up, mouseDelta.x, Space.World);

                Quaternion oldRotation = playerInput.camera.transform.rotation;
                playerInput.camera.transform.Rotate(Vector3.right, -mouseDelta.y);
            
                float angle = Vector3.Angle(playerInput.camera.transform.TransformDirection(Vector3.up), Vector3.up);

                if (angle > 70)
                {
                    playerInput.camera.transform.rotation = oldRotation;
                }
            }
            else
            {
                mouseDelta *= lookSpeed2D;
                Vector3 newPos = new Vector3(playerInput.camera.transform.position.x - mouseDelta.x,
                    playerInput.camera.transform.position.y - mouseDelta.y, playerInput.camera.transform.position.z);
                playerInput.camera.transform.position = newPos;
            }
        }
        
        //Zoom
        if (!is3D && !Mouse.current.leftButton.isPressed)
        {
            float scrollValue = Mouse.current.scroll.y.ReadValue();

            playerInput.camera.orthographicSize = Math.Clamp(playerInput.camera.orthographicSize - scrollValue * zoomSpeed2D, 1, 1000);

            float oldHeight = minHeight;
            
            LimitMaxCameraHeight();
            
            playerInput.camera.transform.position -= Vector3.up * (oldHeight - minHeight);
        }
        
        if (!is3D)
        {
            playerInput.camera.transform.position = new Vector3(playerInput.camera.transform.position.x,
                Math.Clamp(playerInput.camera.transform.position.y, minHeight, 10000f), playerInput.camera.transform.position.z);
        }
        
    }
}
