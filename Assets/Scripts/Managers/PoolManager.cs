using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShapePool
{
    public ShapeType shapeType;
    public GameObject prefab;
    public int size;
}

public class PoolManager : MonoBehaviour
{
    public List<ShapePool> shapePools;
    
    private Dictionary<ShapeType, Queue<GameObject>> poolDictionary;
    
    void Start()
    {
        InitializePools();
    }
    
    void InitializePools()
    {
        poolDictionary = new Dictionary<ShapeType, Queue<GameObject>>();
        
        foreach (ShapePool pool in shapePools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            
            poolDictionary.Add(pool.shapeType, objectPool);
        }
    }
    
    public GameObject GetShape(ShapeType shapeType, Vector3 position)
    {
        if (!poolDictionary.ContainsKey(shapeType))
        {
            Debug.LogError($"No pool for shape: {shapeType}");
            return null;
        }
        
        if (poolDictionary[shapeType].Count == 0)
        {
            Debug.LogWarning($"Pool empty for {shapeType}, creating new object");
            ShapePool pool = shapePools.Find(p => p.shapeType == shapeType);
            if (pool != null)
            {
                GameObject newObj = Instantiate(pool.prefab);
                newObj.transform.position = position;
                return newObj;
            }
            return null;
        }
        
        GameObject obj = poolDictionary[shapeType].Dequeue();
        obj.SetActive(true);
        obj.transform.position = position;
        return obj;
    }
    
    public void ReturnToPool(GameObject obj, ShapeType shapeType)
    {
        if (poolDictionary.ContainsKey(shapeType))
        {
            obj.SetActive(false);
            poolDictionary[shapeType].Enqueue(obj);
        }
    }
}
