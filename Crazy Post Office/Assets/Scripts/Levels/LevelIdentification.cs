using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelIdentification : MonoBehaviour
{
	public bool infiniteSpawns = false;
	public bool checkStillTime = true;
	public int numberOfPackages = 1;
	public float timeForLevelCompletion = 60f;
	public float timeBetweenPackages = 2f;
	public float allowedStillTime = 3f;
}
