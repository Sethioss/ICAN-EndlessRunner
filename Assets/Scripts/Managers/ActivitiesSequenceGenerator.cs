using FMOD.Studio;
using Sirenix.OdinInspector.Editor.Drawers;
using System.Collections.Generic;
using UnityEngine;

public class ActivitiesSequenceGenerator : MonoBehaviour
{
    [SerializeField] Transform FirstTileLocation;
    [SerializeField] private ObstacleTile LeadingTile;
    [HideInInspector] public GameObject LeadingTileObject;
    [SerializeField] public List<ObstacleTile> Tiles = new List<ObstacleTile>();
    [HideInInspector] public List<GameObject> InstantiatedTiles = new List<GameObject>();

    [SerializeField] private int NumberOfTilesSpawnedAtOnce = 5;

    [Header("New activity system")]
    //New activity system
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private ActivityGeometry LeadingActivityGeometry;
    [HideInInspector] public GameObject LeadingActivityObject;

    private List<ActivityData> PossibleActivities = new List<ActivityData>();
    private List<SOActivity> IncomingActivities = new List<SOActivity>();
    [HideInInspector] public List<GameObject> InstantiatedActivities = new List<GameObject>();
    [SerializeField] private List<ActivityGenerationPool> GenerationPools = new List<ActivityGenerationPool>();

    int CurrentPool = 0;
    int NextPoolMinDistance = 0;
    bool ReachedLastPool = false;

    // Start is called before the first frame update
    void Start()
    {
        //StartSpawningObstacles();

        ReachedLastPool = GetReachedLastPool();
        if (!ReachedLastPool)
        {
            NextPoolMinDistance = GenerationPools[CurrentPool + 1]._minDistance;
        }

        MakeNewIncomingActivitiesList();

        LeadingActivityObject = Instantiate(LeadingActivityGeometry.gameObject, FirstTileLocation.position, Quaternion.identity);
        InstantiatedActivities.Insert(0, LeadingActivityObject.gameObject);
        LeadingActivityGeometry = LeadingActivityObject.GetComponent<ActivityGeometry>();

        SpawnActivitiesGeometry();
    }

    private ActivityGenerationPool GetActivityGenerationPoolAt(int index)
    {
        return GenerationPools[index];
    }

    private bool GetReachedLastPool()
    {
        return CurrentPool + 1 >= GenerationPools.Count;
    }

    private void MakeNewIncomingActivitiesList()
    {
        IncomingActivities.Clear();

        ReachedLastPool = GetReachedLastPool();
        if (!ReachedLastPool)
        {
            CurrentPool = SelectPool();
        }

        ActivityGenerationPool CurrentGenPool = GenerationPools[CurrentPool];

        PossibleActivities = CurrentGenPool._activities;

        float RandomNumber = Random.Range(0.0f, 1.0f);
        float PoolLength = NextPoolMinDistance - CurrentGenPool._minDistance;
        float PlayerPosInPool = _gameManager.GetInstance().levelScroller.DistanceTraveled - CurrentGenPool._minDistance;
        float CurrentPoolRatio = PlayerPosInPool / PoolLength;

        for (int i = 0; i < NumberOfTilesSpawnedAtOnce; ++i)
        {
            IncomingActivities.Add(SelectActivity(CurrentGenPool).activitySO);
        }
    }

    public ActivityData SelectActivity(ActivityGenerationPool CurrentGenPool)
    {
        PossibleActivities = CurrentGenPool._activities;

        float RandomNumber = Random.Range(0.0f, 1.0f);
        float PoolLength = NextPoolMinDistance - CurrentGenPool._minDistance;
        float PlayerPosInPool = _gameManager.GetInstance().levelScroller.DistanceTraveled - CurrentGenPool._minDistance;
        float CurrentPoolRatio = Mathf.Clamp(PlayerPosInPool / PoolLength, 0.0f, 1.0f);

        for (int j = 0; j < CurrentGenPool._activities.Count; ++j)
        {
            float NumToSubtract = PossibleActivities[j].GetFinalWeightFromCurve(CurrentPoolRatio);
            RandomNumber -= NumToSubtract;
            if (RandomNumber <= 0)
            {
                return PossibleActivities[j];
            }
        }
        //RandomNumber never reached below 0, undefined behaviour
        //TODO: Make it so it's impossible to have a gen pool be null
        return PossibleActivities[PossibleActivities.Count - 1];
    }

    public void SpawnActivitiesGeometry()
    {
        for (int i = 0; i < IncomingActivities.Count; ++i)
        {
            InstantiatedActivities.Insert(0, Instantiate(IncomingActivities[i]._Geometry.gameObject, LeadingActivityObject.GetComponent<ActivityGeometry>()._tail.gameObject.transform.position, LeadingActivityObject.GetComponent<ActivityGeometry>()._tail.gameObject.transform.rotation).gameObject);
            LeadingActivityObject = InstantiatedActivities[0];
        }
    }

    public int SelectPool()
    {
        float distance = _gameManager.GetInstance().levelScroller.DistanceTraveled;
        if (distance >= NextPoolMinDistance)
        {
            if (CurrentPool + 1 < GenerationPools.Count)
            {
                NextPoolMinDistance = GenerationPools[CurrentPool + 1]._minDistance;
                return CurrentPool + 1;
            }
        }
        return CurrentPool;

    }
    public void StartSpawningObstacles()
    {
        LeadingTileObject = Instantiate(LeadingTile.gameObject, FirstTileLocation.position, Quaternion.identity);
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
        ActivityData NewActivity = SelectActivity(GetActivityGenerationPoolAt(CurrentPool));

        InstantiatedActivities.RemoveAt(InstantiatedActivities.Count - 1);
        LeadingActivityGeometry = InstantiatedActivities[0].GetComponent<ActivityGeometry>();
        InstantiatedActivities.Insert(0, Instantiate(NewActivity.activitySO._Geometry.gameObject, LeadingActivityGeometry._tail.gameObject.transform.position, Quaternion.identity));

        Destroy(RemovedGO);

        if(SelectPool() != CurrentPool)
        {
            MakeNewIncomingActivitiesList();
        }
    }
}
