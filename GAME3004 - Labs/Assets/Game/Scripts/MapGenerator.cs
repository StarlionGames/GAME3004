using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("World Properties")]
    [Range(8,64)] public int height, width, depth;

    [Header("Scale Values")]
    [Range(8, 64)] public float min = 16f;
    [Range(8, 64)] public float max = 24f;

    [Header("Tile Properties")]
    public GameObject tilePrefab;
    public Transform tileParent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Regenerate();
    }

    void Regenerate()
    {
        float ranScale = Random.Range(min, max);
        float offsetX = Random.Range(-1024f, 1024f);
        float offsetY = Random.Range(-1024f, 1024f);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    var perlinNoise = Mathf.PerlinNoise((x + offsetX) / ranScale
                        , (z + offsetX) / ranScale) * depth * 0.5f;

                    if (y< perlinNoise)
                    {
                        var tile = Instantiate(tilePrefab, new Vector3(x, y, z), Quaternion.identity);
                        tile.transform.SetParent(tileParent);
                    }
                }
            }
        }
    }
}
