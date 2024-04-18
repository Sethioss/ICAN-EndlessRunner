using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] Vector3 FirstTileLocation;
    private ObstacleTile LeadingTile;
    [SerializeField] private List<ObstacleTile> Tiles = new List<ObstacleTile>();
    private List<GameObject> InstantiatedTiles = new List<GameObject>();
    private List<ObstacleTile> EasyTiles = new List<ObstacleTile>();
    private List<ObstacleTile> MediumTiles = new List<ObstacleTile>();
    private List<ObstacleTile> HardTiles = new List<ObstacleTile>();

    [SerializeField] private int NumberOfTilesSpawnedAtOnce = 5;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < NumberOfTilesSpawnedAtOnce; i++)
        {
            int randomID = Random.Range(0, i % Tiles.Count + 1);
            if (i == 0)
            {
                InstantiatedTiles.Insert(i, Instantiate(Tiles[randomID].gameObject, FirstTileLocation, Quaternion.identity));
                LeadingTile = InstantiatedTiles[i].GetComponent<ObstacleTile>();
            }
            else
            {
                InstantiatedTiles.Insert(i, Instantiate(Tiles[randomID].gameObject, LeadingTile.tail.position, Quaternion.identity));
                LeadingTile = InstantiatedTiles[i].GetComponent<ObstacleTile>();
            }
        }
    }
}
