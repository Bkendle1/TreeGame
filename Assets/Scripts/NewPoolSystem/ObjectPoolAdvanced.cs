using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolAdvanced : MonoBehaviour
{
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();

    public GameObject GetObject(GameObject gameObject)
    {
        //if game object with given name is in the dictionary...
        if(objectPool.TryGetValue(gameObject.name, out Queue<GameObject> objectList))
        {
            //if object isn't in queue, create game object
            if (objectList.Count == 0)
            {
                return CreateNewObject(gameObject);
            }
            else // else dequeue object from queue, set active, and return it
            {
                GameObject obj = objectList.Dequeue();
                obj.SetActive(true);
                return obj;
            }
        }
        else // if game object isnt in the dictionary, create the game object and return it
        {
            return CreateNewObject(gameObject);
        }
    }

    private GameObject CreateNewObject(GameObject gameObject)
    {
        GameObject obj = Instantiate(gameObject);
        //name the new object to be the same as the given prefab gameObject
        //because the name of the gameObject is used for a key in the dictionary
        obj.name = gameObject.name;
        return obj;
    }

    public void ReturnGameObject(GameObject gameObject)
    {
        //if we have a key with the name of the game object in the dictionary,
        //then we add that game object back into the corresponding queue 
        if (objectPool.TryGetValue(gameObject.name, out Queue<GameObject> objectList))
        {
            objectList.Enqueue(gameObject);
        }
        // if we can't find a matching key in the dictionary, we create a queue
        //for that game object, adding it into that queue, then adding both the 
        //key for that game object and queue into the dictionary
        else 
        {
            Queue<GameObject> newObjectQueue = new Queue<GameObject>();
            newObjectQueue.Enqueue(gameObject);
            objectPool.Add(gameObject.name, newObjectQueue);
        }
        
        //deactivate given prefab 
        gameObject.SetActive(false);
    }

}
