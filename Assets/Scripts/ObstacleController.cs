using System;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public Dictionary<GameObject, Queue<GameObject>> obstaclePools = new Dictionary<GameObject, Queue<GameObject>>();
    public ObstacleSet[] ObstacleSets;
    public List<int> ObstacleLineIndexs = new List<int>();
    [SerializeField] private CheckPoint[] _checkPoints;
    public CheckPoint[] CheckPoints => _checkPoints;
    public static ObstacleController Instance;
    private int lineSkipIndex = 0;
    private int currentLineIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitializePools();
        GenerateObstacleLineIndexs(MapController.CubeMapWidth, MapController.CubeMapHeight);
    }

    void InitializePools()
    {
        if (ObstacleSets == null) return;

        foreach (var set in ObstacleSets)
        {
            if (set == null || set.Obstacles == null) continue;

            foreach (var prefab in set.Obstacles)
            {
                if (prefab == null) continue;
                if (obstaclePools.ContainsKey(prefab)) continue;

                var q = new Queue<GameObject>();
                int size = Mathf.Max(1, set.PoolSize);
                for (int i = 0; i < size; i++)
                {
                    var inst = Instantiate(prefab, transform);
                    inst.SetActive(false);
                    var tag = inst.GetComponent<ObstacleInstance>() ?? inst.AddComponent<ObstacleInstance>();
                    tag.Prefab = prefab;
                    q.Enqueue(inst);
                }

                obstaclePools.Add(prefab, q);
            }
        }
    }

    public GameObject SpawnRandomFromSet(int setIndex, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (ObstacleSets == null || setIndex < 0 || setIndex >= ObstacleSets.Length)
        {
            Debug.LogWarning("Invalid obstacle set index");
            return null;
        }

        var set = ObstacleSets[setIndex];
        if (set == null || set.Obstacles == null || set.Obstacles.Length == 0)
        {
            Debug.LogWarning("Obstacle set is empty");
            return null;
        }

        var prefab = set.Obstacles[UnityEngine.Random.Range(0, set.Obstacles.Length)];
        if (prefab == null) return null;

        var obj = GetFromPool(prefab);
        if (obj == null) return null;
        obj.transform.SetParent(parent != null ? parent : transform, true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        return obj;
    }

    public GameObject SpawnRandom(Vector3 position, Quaternion rotation, Transform parent = null){
        int setIndex = GetCurrentObstacleSetIndex();
        return SpawnRandomFromSet(setIndex, position, rotation, parent);
    }

    GameObject GetFromPool(GameObject prefab)
    {
        if (obstaclePools.TryGetValue(prefab, out var q) && q.Count > 0)
        {
            var obj = q.Dequeue();
            if (obj == null)
            {
                var inst = Instantiate(prefab, transform);
                var tag = inst.GetComponent<ObstacleInstance>() ?? inst.AddComponent<ObstacleInstance>();
                tag.Prefab = prefab;
                return inst;
            }

            return obj;
        }

        var newObj = Instantiate(prefab, transform);
        var newTag = newObj.GetComponent<ObstacleInstance>() ?? newObj.AddComponent<ObstacleInstance>();
        newTag.Prefab = prefab;
        return newObj;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;
        var tag = obj.GetComponent<ObstacleInstance>();
        if (tag == null || tag.Prefab == null)
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        if (!obstaclePools.TryGetValue(tag.Prefab, out var q))
        {
            q = new Queue<GameObject>();
            obstaclePools.Add(tag.Prefab, q);
        }

        q.Enqueue(obj);
    }

    public int GetCurrentObstacleSetIndex()
    {
        for (int i = CheckPoints.Length - 1; i >= 0; i--)
        {
            if (PlayerData.RunDistance >= CheckPoints[i].Distance)
            {
                return i;
            }
        }
        return 0;
    }

    public ObstacleSet GetCurrentObstacleSet()
    {
        int index = GetCurrentObstacleSetIndex();
        if (ObstacleSets != null && index >= 0 && index < ObstacleSets.Length)
        {
            return ObstacleSets[index];
        }
        return null;
    }

    public bool IsSkippingObstacle()
    {
        int currentSkipCount = GetCurrentObstacleSet().SkipLine;
        if(currentSkipCount == 0) return false;
        bool skipping = true;    
        lineSkipIndex--;    
        if(lineSkipIndex <= 0)
        {
            lineSkipIndex = currentSkipCount;
            skipping = false;
        }
        return skipping;
    }

    // Generate ObstacleLineIndexs for each map row (height).
    // Each index will be in [2, mapWidth - 2].
    // Constraints: |i - i+1| >= 2 and |i - i+2| >= 2 for all valid i.
    public bool GenerateObstacleLineIndexs(int mapWidth, int mapHeight)
    {
        ObstacleLineIndexs.Clear();

        if (mapWidth <= 4)
        {
            Debug.LogWarning("mapWidth must be at least 5 to allow margin of 2 on both sides.");
            return false;
        }

        if (mapHeight <= 0)
        {
            return true;
        }

        int min = 2;
        int max = mapWidth - 2; // inclusive
        int length = mapHeight;

        var result = new List<int>(length);
        int distinctCount = Mathf.Max(0, max - min + 1);
        if (length < distinctCount)
        {
            Debug.LogWarning("mapHeight is smaller than distinct available columns; cannot guarantee full coverage.");
        }

        var rnd = new System.Random();
        var unused = new HashSet<int>();
        for (int v = min; v <= max; v++) unused.Add(v);

        bool TryFill(int pos)
        {
            if (pos >= length) return true;

            var candidates = new List<int>();
            for (int v = min; v <= max; v++) candidates.Add(v);

            // shuffle
            for (int i = candidates.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                int tmp = candidates[i];
                candidates[i] = candidates[j];
                candidates[j] = tmp;
            }

            // prefer unused values first
            candidates.Sort((a, b) =>
            {
                bool ua = unused.Contains(a);
                bool ub = unused.Contains(b);
                if (ua == ub) return 0;
                return ua ? -1 : 1;
            });

            foreach (var cand in candidates)
            {
                if (pos >= 1 && Mathf.Abs(cand - result[pos - 1]) < 2) continue;
                if (pos >= 2 && Mathf.Abs(cand - result[pos - 2]) < 2) continue;

                // If remaining positions equal remaining unused values, force choosing unused
                if (unused.Count > 0 && (length - pos) <= unused.Count && !unused.Contains(cand)) continue;

                result.Add(cand);
                bool removed = false;
                if (unused.Contains(cand)) { unused.Remove(cand); removed = true; }

                if (TryFill(pos + 1)) return true;

                if (removed) unused.Add(cand);
                result.RemoveAt(result.Count - 1);
            }

            return false;
        }

        bool ok = TryFill(0);
        if (!ok)
        {
            Debug.LogWarning("Failed to generate ObstacleLineIndexs with the given constraints.");
            return false;
        }

        ObstacleLineIndexs.AddRange(result);
        return true;
    }

    public int GetNextObstacleLineIndex()
    {
        if (ObstacleLineIndexs.Count == 0) return 0;        
        currentLineIndex++;
        if(currentLineIndex >= ObstacleLineIndexs.Count)
        {
            currentLineIndex = 0;
        }
        return ObstacleLineIndexs[currentLineIndex];
    }
}

[Serializable]
public class ObstacleSet
{
    public GameObject[] Obstacles;
    public int PoolSize = 5;
    public int SafeDistance;
    public int SkipLine;    
}

[Serializable]
public class ObstacleInstance : MonoBehaviour
{
    public GameObject Prefab;
    
}
