using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hackscript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("PackageManager").GetComponent<PackageManager>().StartLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
