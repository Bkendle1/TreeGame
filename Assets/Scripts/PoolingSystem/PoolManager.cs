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
using System.Collections.Generic;
using UnityEngine;

public static class PoolManager 
{
    private static readonly Dictionary<string,Pooling> _objectPools = new Dictionary<string, Pooling>();

    #region Public
    public static bool DoesPoolExist(string poolName)
    {
        if (!_objectPools.ContainsKey(poolName))
            return false;
        return true;
    }

    public static Pooling GetPool(string poolName)
    {
        if (!_objectPools.ContainsKey(poolName))
        {
            Debug.LogWarning(poolName + " pool doesn't exist");
            return null;
        }
         
        return _objectPools[poolName];
    }

    public static void CreatePool(string poolName, GameObject gameObjectPool, int StartingPool)
    {
        if(_objectPools.ContainsKey(poolName))
        {
            Debug.LogWarning(poolName + " pool already exitst");
            return;
        }


        Pooling newPool = new Pooling(gameObjectPool, StartingPool);
        _objectPools.Add(poolName, newPool);
    }

    public static void ExpandPool (string poolName, int expand)
    {
        if (!_objectPools.ContainsKey(poolName))
        {
            Debug.LogWarning(poolName + " pool Doesn't exitst");
            return;
        }
        _objectPools[poolName].RequestExpandPool(expand);
    }

    public static void DeletePool(string poolName)
    {
        if (!_objectPools.ContainsKey(poolName))
        {
            Debug.LogWarning(poolName + " pool Doesn't exitst, wont remove pool");
            return;
        }
        _objectPools[poolName].Dispose();
        _objectPools.Remove(poolName);
             
    }
    #endregion
}
