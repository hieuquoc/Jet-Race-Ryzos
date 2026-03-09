using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZyroX
{
    public class MapController : MonoBehaviour
{
    [SerializeField] private float[] cubeHeights;
    [SerializeField] private float cubeSpacing = 2f;
    [SerializeField] private int cubeMapWidth = 10;
    [SerializeField] private int cubeMapHeight = 10;
    public GameObject CubePrefab;
    [SerializeField] private float lineSpacing = 10f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private Transform linesParent;
    [SerializeField] private bool isMoving = false;
    [SerializeField] float _loopStartDistance = 0f;
    [SerializeField] private float _runDistance = 0f;


    private List<BlockLine> lines = new List<BlockLine>();
    private BlockLine lastSpawnedLine;
    private float cubeDepth = 1f;

    public static int CubeMapWidth => Instance.cubeMapWidth;
    public static float CubeSpacing => Instance.cubeSpacing;
    public static int CubeMapHeight => Instance.cubeMapHeight;
    public static float RunDistance
    {
        get => Instance._runDistance;
        set => Instance._runDistance = value;
    }    
    public static float LoopStartDistance
    {
        get => Instance._loopStartDistance;
        set => Instance._loopStartDistance = value;
    }

    public static bool IsMoving => Instance.isMoving;

    public static MapController Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (CubePrefab == null)
        {
            Debug.LogError("CubePrefab is not assigned in MapController.");
            return;
        }

        // Use prefab scale to determine spacing when possible
        Vector3 prefabScale = CubePrefab.transform.localScale;
        if (prefabScale.x > 0f) cubeSpacing = prefabScale.x;
        if (prefabScale.z > 0f)
        {
            lineSpacing = prefabScale.z;
            cubeDepth = prefabScale.z;
        }

        SpawnInitialLines();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving) return;
        float move = moveSpeed * speedMultiplier * Time.deltaTime;
        RunDistance += move;
        LoopStartDistance += move;
        UIManager.Instance.DistanceText.SetText($"{RunDistance:F1}m");
        if(LoopStartDistance > ObstacleController.LoopLength)
        {
            LoopStartDistance %= ObstacleController.LoopLength;
        }
        for (int i = 0; i < lines.Count; i++)
        {
            BlockLine line = lines[i];
            line.transform.position += Vector3.back * move;

            if (line.transform.position.z < 0f)
            {
                line.transform.position = new Vector3(GetXOffSetPlayerPosition(), line.transform.position.y, lastSpawnedLine.transform.position.z + lineSpacing);
                RegenerateLineHeights(line.transform);
                line.ClearObstacle();
                line.Obstacle = ObstacleController.Instance.SpawnRandom(line);
                lastSpawnedLine = line;
                line.gameObject.SetActive(!ObstacleController.Instance.CurrentCheckPoint.HideLine); // Example method to check if line should be active
            }            
        }
        ObstacleController.Instance.UpdateObstacles(Vector3.back * move);
    }

    public float GetXOffSetPlayerPosition()
        {
            return Mathf.Round(GameManager.Instance.SpaceShipPoint.position.x / cubeSpacing) * cubeSpacing;
        }

    private void SpawnInitialLines()
    {
        lines.Clear();

        for (int row = 0; row < cubeMapHeight; row++)
        {
            GameObject lineGO = new GameObject($"Line_{row}");
            if (linesParent != null) lineGO.transform.parent = linesParent;
            else lineGO.transform.parent = transform;

            float z = row * lineSpacing;
            lineGO.transform.localPosition = new Vector3(0f, 0f, z);      
            BlockLine blockLine = lineGO.AddComponent<BlockLine>();
            blockLine.CreateBlockLine();
            lines.Add(blockLine);
        }
        lastSpawnedLine = lines[lines.Count - 1];
    }

    public static float GetRandomHeight()
    {
        if (Instance.cubeHeights != null && Instance.cubeHeights.Length > 0)
        {
            int idx = Random.Range(0, Instance.cubeHeights.Length);
            return Instance.cubeHeights[idx];
        }
        // fallback random small range
        return Random.Range(0.5f, 2.5f);
    }

    private void RegenerateLineHeights(Transform line)
    {
        for (int i = 0; i < line.childCount; i++)
        {
            Transform cube = line.GetChild(i);
            Vector3 lp = cube.localPosition;
            lp.y = GetRandomHeight();
            cube.localPosition = lp;
        }
    }

    public void SetSpeedMultiplier(float newMultiplier)
    {
        speedMultiplier = newMultiplier;
    }

    public void StartGame()
    {
        isMoving = true;
        RunDistance = 0f;
        LoopStartDistance = 0f;
        UIManager.Instance.DistanceText.SetText("0.0m");
        foreach (var line in lines)
        {
            line.gameObject.SetActive(true);
            line.Obstacle = null;
        }
    }

    public void GameOver()
    {
        isMoving = false;
    }

    public void Stop()
    {
        isMoving = false;
    }
}

}


