using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform camTransform;
    private Vector3 lastCamPosition;
    
    void Start()
    {
        camTransform = Camera.main.transform;
        lastCamPosition = camTransform.position;
    }

    void LateUpdate()
    {
        //calculate camera's displacement
        Vector3 displacement = camTransform.position - lastCamPosition;
        
        //modify gameobject's position based on camera's displacement
        transform.position += displacement;
        
        //update lastCamPosition
        lastCamPosition = camTransform.position;
        
        
        
    }
}
