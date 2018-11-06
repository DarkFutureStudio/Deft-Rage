using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    public static SpawnScript instance;

    public Transform Spawn;

    private void Awake()
    {
        instance = this;
    }
}
