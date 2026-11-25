using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public static TerrainGenerator Instance;

    void Awake()
    {
        Instance = this;
    }

    public void Generate()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.gameObject == gameObject || meshFilter.mesh == null)
                continue;
            
            CombineInstance combineInstance = new CombineInstance
            {
                mesh = meshFilter.sharedMesh,
                transform = transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix
            };

            combineInstances.Add(combineInstance);
        }

        Mesh combinedMesh = new Mesh();

        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        if (GetComponent<MeshFilter>() != null) GetComponent<MeshFilter>().mesh = combinedMesh;
        else
        {
            MeshFilter filter = gameObject.AddComponent<MeshFilter>();
            filter.mesh = combinedMesh;
        }

        if (GetComponent<MeshCollider>() != null) GetComponent<MeshCollider>().sharedMesh = combinedMesh;
        else
        {
            MeshCollider collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = combinedMesh;
        }

        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
