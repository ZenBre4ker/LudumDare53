using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    private Vector3 resetMovePosition;
    private Vector3 finalMovePosition;
    private Vector3 globalMoveDelta;

    public bool shouldMove = true;

    public float moveSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        resetMovePosition = transform.position;

        transform.localPosition += new Vector3(0.04f/transform.lossyScale.x, 0, 0);;
        finalMovePosition = transform.position;
        globalMoveDelta = finalMovePosition - resetMovePosition;
        
        transform.position = resetMovePosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!shouldMove) return;
        
        gameObject.GetComponent<Rigidbody>().position = resetMovePosition;
        gameObject.GetComponent<Rigidbody>().MovePosition(resetMovePosition + globalMoveDelta * moveSpeed);
    }
}
