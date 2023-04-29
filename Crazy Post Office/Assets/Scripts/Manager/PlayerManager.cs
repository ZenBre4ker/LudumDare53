using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public delegate void OnFireDelegate();

    public static OnFireDelegate onUserFire;

    void OnFire()
    {
        onUserFire?.Invoke();
    }
}
