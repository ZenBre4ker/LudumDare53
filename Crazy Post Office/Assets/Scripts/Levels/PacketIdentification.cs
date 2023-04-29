using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketIdentification : MonoBehaviour
{
    public int packetNumber;
    public bool isStill;
    public float stillSince;

    public Vector3 lastMovementSpeed;
    public Vector3 lastRotationSpeed;
}
