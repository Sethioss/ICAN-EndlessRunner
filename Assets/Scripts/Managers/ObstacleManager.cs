using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] Vector3 FirstTileLocation;
    [SerializeField] private ObstacleTile LeadingTile;
    [HideInInspector] public GameObject LeadingTileObject;
    [SerializeField] public List<ObstacleTile> Tiles = new List<ObstacleTile>();
    public List<GameObject> InstantiatedTiles = new List<GameObject>();
    private List<ObstacleTile> EasyTiles = new List<ObstacleTile>();
    private List<ObstacleTile> MediumTiles = new List<ObstacleTile>();
    private List<ObstacleTile> HardTiles = new List<ObstacleTile>();

    [SerializeField] private int NumberOfTilesSpawnedAtOnce = 5;

    // Start is called before the first frame update
    void Start()
    {
        LeadingTileObject = Instantiate(LeadingTile.gameObject, FirstTileLocation, Quaternion.identity);
        LeadingTile = LeadingTileObject.GetComponent<ObstacleTile>();

        for (int i = 0; i < NumberOfTilesSpawnedAtOnce; i++)
        {
            int randomID = Random.Range(0, i % Tiles.Count + 1);
            InstantiatedTiles.Insert(0, Instantiate(Tiles[randomID].gameObject, LeadingTile.tail.position, LeadingTile.tail.rotation));
            LeadingTile = InstantiatedTiles[0].GetComponent<ObstacleTile>();
        }
    }

    public void CreateNewObstacle(GameObject RemovedGO)
    {
        if(RemovedGO != LeadingTileObject)
        {
            InstantiatedTiles.RemoveAt(InstantiatedTiles.Count - 1);
        }
        Destroy(RemovedGO);

        int randomID = Random.Range(0, Tiles.Count);

        if (RemovedGO != LeadingTileObject)
        {
            InstantiatedTiles.Insert(0, Instantiate(Tiles[randomID].gameObject, LeadingTile.tail.position, Quaternion.identity));
            LeadingTile = InstantiatedTiles[0].GetComponent<ObstacleTile>();
        }
    }
}
