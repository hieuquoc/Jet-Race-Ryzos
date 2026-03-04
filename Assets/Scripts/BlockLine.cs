using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockLine : MonoBehaviour
{
    public Transform Obstacle;
    public List<Transform> Cubes = new List<Transform>();

    public void SpawnObstacle()
    {
        if(ObstacleController.Instance.IsSkippingObstacle() || ObstacleController.Instance.GetCurrentObstacleSet().Obstacles.Length == 0)
        {
            return;
        }
        Vector3 spawnPosition = GetLinePoint(ObstacleController.Instance.GetNextObstacleLineIndex());
        if (Obstacle != null)
        {
            ObstacleController.Instance.ReturnToPool(Obstacle.gameObject);
        }
        Obstacle = ObstacleController.Instance.SpawnRandom(spawnPosition, Quaternion.identity, transform).transform;
        BaseMoveObstacle moveObstacle = Obstacle.GetComponent<BaseMoveObstacle>();
        if (moveObstacle != null)
        {
            moveObstacle.SetUp();
        }
    }

    public void SpawnObstacleDelayed(float delay)
    {
        StartCoroutine(SpawnObstacleWithDelay(delay));
    }

    public Vector3 GetLinePoint(int lineIndex)
    {
        Vector3 cubePos = Cubes[lineIndex].position;
        return new Vector3(cubePos.x, -10, cubePos.z);
    }

    IEnumerator SpawnObstacleWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnObstacle();
    }

    public void ClearObstacle()
    {
        if (Obstacle != null)
        {
            ObstacleController.Instance.ReturnToPool(Obstacle.gameObject);
            Obstacle = null;
        }
    }

    public void CreateBlockLine()
    {
        // spawn cubes across width
            for (int x = 0; x < MapController.CubeMapWidth; x++)
            {
                float xPos = (x - (MapController.CubeMapWidth - 1) / 2f) * MapController.CubeSpacing;
                float y = MapController.GetRandomHeight();
                Vector3 worldPos = transform.TransformPoint(new Vector3(xPos, y, 0f));
                GameObject cube = Instantiate(MapController.Instance.CubePrefab, worldPos, Quaternion.identity, transform);
                // ensure local z is 0 so the line's z controls depth
                Vector3 lp = cube.transform.localPosition;
                cube.transform.localPosition = new Vector3(lp.x, lp.y, 0f);
                Cubes.Add(cube.transform);
            }
    }
}
