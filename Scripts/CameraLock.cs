using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLock : MonoBehaviour
{

    public Transform face;


    // Update is called once per frame
    void Update()
    {
        transform.position = face.position;
    }
}
