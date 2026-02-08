using UnityEngine;
using System.Collections.Generic;

public class TreeScatterGenerator : MonoBehaviour
{
    [Header("Ground")]
    public GameObject ground;

    [Header("Trees")]
    public List<GameObject> treePrefabs;
    public int treeCount = 100;
    public float minSpacing = 2f;

    [Header("Scale")]
    public Vector2 scaleRange = new Vector2(0.9f, 0.9f);

    [Header("Placement")]
    public bool randomRotation = true;
    public float raycastHeight = 50f;

    [Header("Layer Rules")]
    public LayerMask groundLayer;
    public LayerMask excludeLayer;

    private readonly List<Vector3> placedPositions = new();

    void Start()
    {
        GenerateTrees();
    }

    void GenerateTrees()
    {
        if (ground == null || treePrefabs.Count == 0)
        {
            Debug.LogWarning("TreeScatterGenerator: Missing ground or tree prefabs.");
            return;
        }

        for (int i = 0; i < treeCount; i++)
        {
            if (!TryGetValidPosition(out Vector3 position))
                continue;

            placedPositions.Add(position);

            GameObject prefab =
                treePrefabs[Random.Range(0, treePrefabs.Count)];

            GameObject tree = Instantiate(
                prefab,
                position,
                randomRotation
                    ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                    : Quaternion.identity
            );

            float scale = Random.Range(scaleRange.x, scaleRange.y);
            tree.transform.localScale = prefab.transform.localScale * scale;

            tree.transform.SetParent(transform, true);
        }
    }

    bool TryGetValidPosition(out Vector3 position)
    {
        Bounds bounds = GetGroundBounds();

        for (int attempt = 0; attempt < 50; attempt++)
        {
            Vector3 origin = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.max.y + raycastHeight,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            if (!Physics.Raycast(
                    origin,
                    Vector3.down,
                    out RaycastHit hit,
                    raycastHeight * 2f,
                    groundLayer | excludeLayer))
                continue;

            // Reject excluded layers
            if (((1 << hit.collider.gameObject.layer) & excludeLayer) != 0)
                continue;

            position = hit.point;

            // Spacing check
            foreach (Vector3 placed in placedPositions)
            {
                if (Vector3.Distance(position, placed) < minSpacing)
                    goto TryAgain;
            }

            return true;

        TryAgain:;
        }

        position = Vector3.zero;
        return false;
    }

    Bounds GetGroundBounds()
    {
        Renderer[] renderers = ground.GetComponentsInChildren<Renderer>();

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        return bounds;
    }
}
