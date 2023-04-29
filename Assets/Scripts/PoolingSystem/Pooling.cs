/* Copyright (c) 2022 Scott Tongue
 * 
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. THE SOFTWARE 
 * SHALL NOT BE USED IN ANY ABLEISM WAY.
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pooling :IDisposable
{
    protected GameObject Object { get; private set; }
    protected Queue<GameObject> _objectPool = new Queue<GameObject>();


    public Pooling(GameObject gameObject, int startsize)
    {
        if (gameObject.GetComponent<PoolObject>() == null)
            throw new System.ArgumentNullException();
        Object = gameObject;
        ExpandPool(startsize);
    }

    

    #region Public
    public GameObject Get()
    {
        if (_objectPool.Count == 0)
            return null;
        GameObject objectToGet = _objectPool.Dequeue();
        objectToGet.gameObject.SetActive(true);
        return objectToGet;

    }

    public GameObject Get(  Vector3 postion ,Quaternion rotation)
    {
        if (_objectPool.Count == 0)
            return null;
        GameObject objectToGet = _objectPool.Dequeue();
        objectToGet.transform.position = postion;
        objectToGet.transform.rotation = rotation;
        objectToGet.gameObject.SetActive(true);
        return objectToGet;

    }

    public GameObject Get(Vector3 postion, Quaternion rotation, Transform parent)
    {
        if (_objectPool.Count == 0)
            return null;
        GameObject objectToGet = _objectPool.Dequeue();
        objectToGet.transform.position = postion;
        objectToGet.transform.rotation = rotation;
        objectToGet.transform.SetParent(parent);
        objectToGet.gameObject.SetActive(true);
        return objectToGet;

    }

    public void RequestExpandPool(int amount)
    {
        ExpandPool(amount);
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        objectToReturn.transform.SetParent(null);
        objectToReturn.gameObject.SetActive(false);
        _objectPool.Enqueue(objectToReturn);
    }
    #endregion

    #region private
    private void ExpandPool(int count)
    {

        for (int i = 0; i <= count; i++)
        {
            GameObject objectToAdd = GameObject.Instantiate(Object, Vector3.zero, Quaternion.identity);
            objectToAdd.GetComponent<PoolObject>().SetupForPooling(this);
            objectToAdd.SetActive(false);
            _objectPool.Enqueue(objectToAdd);
        }
    }

   
    #endregion
    
    #region Interfaces
    public void Dispose()
    {
        while (_objectPool.Count != 0)
        {
            GameObject.Destroy(_objectPool.Dequeue());
        }
    }
    #endregion


}