using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 4)] float speed = 1;
    float currentYRotation = 0;
    // Update is called once per frame
    void Update()
    {
        if (currentYRotation >= 360) currentYRotation = 0;
        currentYRotation += speed;
        Quaternion rotation = Quaternion.Euler(0, currentYRotation, 0);
        transform.rotation = rotation;
    }
}
