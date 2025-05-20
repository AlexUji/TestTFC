using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance {  get { return _instance; } }

    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;

    public Dictionary<Vector2Int, OverlayTile> map;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    void Start()
    {
       
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        map = new Dictionary<Vector2Int, OverlayTile>();

        BoundsInt bounds = tileMap.cellBounds;

        for (int z = bounds.max.z; z > bounds.min.z; z--)
        {
            Debug.Log("z");
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                Debug.Log("y");
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    Debug.Log("x");
                    var tileLocation = new Vector3Int(x, y, z);

                    var tileKey = new Vector2Int(x, y);

                    if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                    {
                        
                        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                        var cWorldPos = tileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cWorldPos.x, cWorldPos.y, cWorldPos.z + 1);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                        map.Add(tileKey, overlayTile);
                    }
                }
            }
        }
    }


}
