using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ZyroX
{
    public class ObstacleController : MonoBehaviour
    {
        public Dictionary<GameObject, Queue<GameObject>> obstaclePools = new Dictionary<GameObject, Queue<GameObject>>();
        public List<int> ObstacleLineIndexs = new List<int>();
        public List<int> EffectLineIndexs = new List<int>();
        [SerializeField] private CheckPoint[] _checkPoints;
        public CheckPoint[] CheckPoints => _checkPoints;
        public static ObstacleController Instance;
        private float loopLength = 0;
        public static float LoopLength => Instance.loopLength;
        private int currentLineIndex = -1;
        private int currentEffectLineIndex = -1;
        private int _currentCheckPoint;
        private GameObject curBigObstacle;
        [SerializeField] private float spawnDistance = 0f;
        private List<GameObject> activeObstacles = new List<GameObject>();
        private List<GameObject> activeEffects = new List<GameObject>();
        public CheckPoint CurrentCheckPoint => CheckPoints[_currentCheckPoint];
        private int currentSkipEffectLine = 0;
        public int SkipEffectLine = 10;
        public int EffectAvoidPreviousLines = 3;

        void Awake()
        {
            Instance = this;
            foreach (var cp in CheckPoints)
            {
                loopLength += cp.Length;
            }
            _currentCheckPoint = 0;
        }

        void Start()
        {
            InitializePools();
            GenerateObstacleLineIndexs(MapController.CubeMapWidth, 100);
            GenerateEffectLineIndexs();
            ConvertCheckPointLengthToDistance();
            spawnDistance = MapController.CubeSpacing * MapController.CubeMapHeight;
        }

        void InitializePools()
        {
            if (_checkPoints == null) return;

            foreach (var cp in _checkPoints)
            {
                var set = cp.ObstacleSet;
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

        public GameObject SpawnRandomFromSet(int checkPoint, Vector3 position, Quaternion rotation)
        {

            var set = _checkPoints[checkPoint].ObstacleSet;
            if (set == null || set.Obstacles == null || set.Obstacles.Length == 0)
            {
                Debug.LogWarning("Obstacle set is empty");
                return null;
            }

            var prefab = set.Obstacles[UnityEngine.Random.Range(0, set.Obstacles.Length)];
            if (prefab == null) return null;

            var obj = GetFromPool(prefab);
            if (obj == null) return null;
            obj.transform.SetParent(transform);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            var moveObstacles = obj.GetComponents<BaseMoveObstacle>();
            if (moveObstacles != null)
            {
                foreach (var moveObstacle in moveObstacles)
                {
                    moveObstacle.SetUp();
                }
                //Debug.Log($"Spawned obstacle from checkpoint {checkPoint}  : {obj.name} at {position}");
            }

            activeObstacles.Add(obj);
            if (CurrentCheckPoint.SingleObstacle)
            {
                return null;
            }
            return obj;
        }

        public GameObject SpawnEffectFromSet(int checkPoint, Vector3 position, Quaternion rotation)
        {

            var set = _checkPoints[checkPoint].EffectSet;
            if (set == null || set.Obstacles == null || set.Obstacles.Length == 0)
            {
                Debug.LogWarning("Effect set is empty");
                return null;
            }

            var prefab = set.Obstacles[UnityEngine.Random.Range(0, set.Obstacles.Length)];
            if (prefab == null) return null;

            var obj = GetFromPool(prefab);
            if (obj == null) return null;
            obj.transform.SetParent(transform);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            activeEffects.Add(obj);
            return obj;
        }

        public GameObject SpawnRandom(BlockLine line)
        {
            if (IsSkippingObstacle())
            {
                return null;
            }
            int lineIndex = GetNextObstacleLineIndex();
            Vector3 position = line.GetLinePoint(lineIndex);

            if(!IsSkippingEffect())
            {
                int effectLineIndex = GetNextEffectLineIndex();
                Vector3 effectPosition = line.GetLinePoint(effectLineIndex);
                line.Effect = SpawnEffectFromSet(_currentCheckPoint, effectPosition, Quaternion.identity);
            }

            return SpawnRandomFromSet(_currentCheckPoint, position, Quaternion.identity);
        }

        // Pre-generate effect indexes tránh N obstacle index liền trước (kể cả cùng line)
        public void GenerateEffectLineIndexs()
        {
            EffectLineIndexs.Clear();
            if (ObstacleLineIndexs.Count == 0) return;

            int min = 5;
            int max = MapController.CubeMapWidth - 5; // exclusive upper
            var rnd = new System.Random();

            for (int i = 0; i < ObstacleLineIndexs.Count; i++)
            {
                // forbidden: N line trước, cùng line, và N line sau
                var forbidden = new HashSet<int>();
                int from = Mathf.Max(0, i - EffectAvoidPreviousLines);
                int to = Mathf.Min(ObstacleLineIndexs.Count - 1, i + EffectAvoidPreviousLines);
                for (int k = from; k <= to; k++)
                    forbidden.Add(ObstacleLineIndexs[k]);

                // candidates: tất cả slot không bị cấm
                var candidates = new List<int>();
                for (int v = min; v < max; v++)
                    if (!forbidden.Contains(v)) candidates.Add(v);

                // fallback nếu không còn slot
                if (candidates.Count == 0)
                {
                    EffectLineIndexs.Add(Random.Range(min, max));
                    continue;
                }

                int pick = candidates[rnd.Next(candidates.Count)];
                EffectLineIndexs.Add(pick);
            }
        }

        public int GetNextEffectLineIndex()
        {
            if (EffectLineIndexs.Count == 0) return 2;
            currentEffectLineIndex++;
            if (currentEffectLineIndex >= EffectLineIndexs.Count)
                currentEffectLineIndex = 0;
            return EffectLineIndexs[currentEffectLineIndex];
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

        public void ReturnObstacle(GameObject obj)
        {
            if (obj == null) return;
            CheckPoint currentCP = CurrentCheckPoint;
            if (curBigObstacle != null && MapController.LoopStartDistance > currentCP.CheckPointDistance - currentCP.Length)
            {
                ReturnToPool(curBigObstacle);
                curBigObstacle = null;
            }
            ReturnToPool(obj);
        }

        public void ReturnEffect(GameObject obj)
        {
            if (obj == null) return;
            ReturnToPool(obj);
        }

        private void ReturnToPool(GameObject obj)
        {
            if(activeObstacles.Contains(obj))
                activeObstacles.Remove(obj);
            else if(activeEffects.Contains(obj)){
                activeEffects.Remove(obj);
            }
             else
            {
                Debug.LogWarning("Returned object is not in active lists: " + obj.name);
            }
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



        public bool IsSkippingObstacle()
        {
            int currentSkipCount = CheckPoints[_currentCheckPoint].SkipLine;
            bool skipping = true;
            CheckPoints[_currentCheckPoint].CurrentSkipCount--;
            if (CheckPoints[_currentCheckPoint].CurrentSkipCount <= 0)
            {
                CheckPoints[_currentCheckPoint].CurrentSkipCount = currentSkipCount;
                skipping = false;
            }
            return skipping;
        }

        public bool IsSkippingEffect()
        {
            if (currentSkipEffectLine <= 0)
            {
                currentSkipEffectLine = SkipEffectLine;
                return false;
            }
            currentSkipEffectLine--;
            return true;
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

            int min = 5;
            int max = mapWidth - 5; // inclusive
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
            if (CurrentCheckPoint.FixedMidpoint)
            {
                return MapController.CubeMapWidth / 2;
            }
            if (ObstacleLineIndexs.Count == 0) return 0;
            currentLineIndex++;
            if (currentLineIndex >= ObstacleLineIndexs.Count)
            {
                currentLineIndex = 0;
            }
            return ObstacleLineIndexs[currentLineIndex];
        }

        private void EnterCheckPoint(int checkpointIndex)
        {
            Debug.Log("Entering checkpoint: " + CurrentCheckPoint.Name);
            if (checkpointIndex < 0 || checkpointIndex >= CheckPoints.Length) return;
            if (checkpointIndex < 0 || checkpointIndex >= CheckPoints.Length) return;
            var cp = CheckPoints[checkpointIndex];
            cp.CurrentSkipCount = 0;
            CheckPoints[checkpointIndex] = cp;
        }

        public void UpdateObstacles(Vector3 move)
        {
            foreach (var obj in activeObstacles)
            {
                obj.transform.position += move;
            }
            foreach (var obj in activeEffects)
            {
                obj.transform.position += move;
            }
            UpdateCheckPoint();
        }

        public void ConvertCheckPointLengthToDistance()
        {
            float distance = 0;
            for (int i = 0; i < CheckPoints.Length; i++)
            {
                var cp = CheckPoints[i];
                distance += cp.Length;
                cp.CheckPointDistance = distance;
                CheckPoints[i] = cp;
            }
        }

        public void UpdateCheckPoint()
        {
            int checkPointIndex = GetCheckPointIndexByDistance();
            if (checkPointIndex == _currentCheckPoint) return;
            _currentCheckPoint = checkPointIndex;
            if (_currentCheckPoint >= CheckPoints.Length)
            {
                _currentCheckPoint = CheckPoints.Length - 1;
            }
            EnterCheckPoint(_currentCheckPoint);
        }

        public int GetCheckPointIndexByDistance()
        {
            float lengthCovered = 0;
            for (int i = 0; i < CheckPoints.Length; i++)
            {
                lengthCovered += CheckPoints[i].Length;
                if (MapController.LoopStartDistance <= lengthCovered)
                {
                    return i;
                }
            }
            return 0;
        }

        public void Reset()
        {
            for (int i = activeObstacles.Count - 1; i >= 0; i--)
            {
                if (activeObstacles[i] != null) ReturnToPool(activeObstacles[i]);
            }
            activeObstacles.Clear();
            if (curBigObstacle != null)
            {
                ReturnToPool(curBigObstacle);
                curBigObstacle = null;
            }

            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                if (activeEffects[i] != null) ReturnToPool(activeEffects[i]);
            }
            activeEffects.Clear();
        }
        


    }

    [Serializable]
    public class ObstacleInstance : MonoBehaviour
    {
        public GameObject Prefab;

    }

    [System.Serializable]
    public struct CheckPoint
    {
        public float CheckPointDistance;
        public ObstacleSet ObstacleSet;
        public ObstacleSet EffectSet;
        public float Length;
        public string Name;
        public bool SingleObstacle;
        public int SkipLine;
        public int CurrentSkipCount;
        public bool FixedMidpoint;
        public bool HideLine;
    }

}
