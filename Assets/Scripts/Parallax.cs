using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    //multiplier used slow the rate at which the gameObject moves with camera's position
    //the further back an object is, the larger their multiplier should be
    //larger multiplier = faster movement with relation to camera's position
    [SerializeField] private Vector2 parallaxEffectMultiplier;

    private Transform camTransform;
    private Vector3 lastCamPosition;
    private float textureUnitSizeX;
    //private float textureUnitSizeY;
    
    void Start()
    {
        camTransform = Camera.main.transform;
        lastCamPosition = camTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        //textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
    }

    void LateUpdate()
    {
        //calculate camera's displacement
        Vector3 displacement = camTransform.position - lastCamPosition;
        
        
        //modify gameobject's position based on camera's displacement 
        transform.position += new Vector3(displacement.x * parallaxEffectMultiplier.x,displacement.y * parallaxEffectMultiplier.y);
        
        //update lastCamPosition
        lastCamPosition = camTransform.position;

        if (Mathf.Abs(camTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offsetPositionX = (camTransform.position.x - transform.position.x) % textureUnitSizeX; 
            transform.position = new Vector3(camTransform.position.x + offsetPositionX, transform.position.y);
        }
        
        //vertical parallax effect 
        // if (Mathf.Abs(camTransform.position.y - transform.position.y) >= textureUnitSizeY)
        // {
        //     float offsetPositionY = (camTransform.position.y - transform.position.y) % textureUnitSizeY; 
        //     transform.position = new Vector3(transform.position.x, camTransform.position.y + offsetPositionY);
        // }
    }
}
