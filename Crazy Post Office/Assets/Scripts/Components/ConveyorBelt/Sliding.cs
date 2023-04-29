using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    public Transform parentTransform;
    
    private Vector3 resetMovePosition;
    private Vector3 localMoveDelta;

    public bool shouldMove = true;

    public float moveSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        //parentTransform = GetComponentInParent<Transform>();
        resetMovePosition = transform.localPosition;

        localMoveDelta = new Vector3(0.4f/transform.lossyScale.x, 0, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!shouldMove) return;

        Vector3 globalResetPosition = parentTransform.position + transform.TransformDirection(resetMovePosition);
        gameObject.GetComponent<Rigidbody>().position = globalResetPosition;
        gameObject.GetComponent<Rigidbody>().MovePosition(globalResetPosition + transform.TransformDirection(localMoveDelta) * moveSpeed);
    }
}
