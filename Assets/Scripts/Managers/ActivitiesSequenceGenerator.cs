using FMOD.Studio;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActivitiesSequenceGenerator : MonoBehaviour
{
    [SerializeField] Transform FirstTileLocation;

    [SerializeField] private int NumberOfActivitiesSpawnedOnStart = 5;

    //New activity system
    [SerializeField] private LevelSystemsHolder _levelSystemsHolder;
    [SerializeField] private ActivityGeometry LeadingActivityGeometry;
    [HideInInspector] public GameObject LeadingActivityObject;

    private List<ActivityData> PossibleActivities = new List<ActivityData>();
    private List<SOActivity> IncomingActivities = new List<SOActivity>();
    public List<SOActivity> SelectedActivities = new List<SOActivity>();
    /*[HideInInspector]*/
    public List<GameObject> InstantiatedActivities = new List<GameObject>();

    [SerializeField] private List<RepetitionObstacleConstraint> RepetitionConstraints;
    [SerializeField] private List<ActivityGenerationPool> GenerationPools = new List<ActivityGenerationPool>();
    [SerializeField] private List<InsertionObstacleProcessor> InsertionProcessors = new List<InsertionObstacleProcessor>();

    int CurrentPool = 0;
    int NextPoolMinDistance = 0;
    bool ReachedLastPool = false;

    // Start is called before the first frame update
    void Start()
    {
        ReachedLastPool = GetReachedLastPool();
        if (!ReachedLastPool)
        {
            NextPoolMinDistance = GenerationPools[CurrentPool + 1]._minDistance;
        }

        MakeNewIncomingActivitiesList();

        LeadingActivityObject = Instantiate(LeadingActivityGeometry.gameObject, FirstTileLocation.position, Quaternion.identity);
        InstantiatedActivities.Insert(0, LeadingActivityObject.gameObject);
        LeadingActivityGeometry = LeadingActivityObject.GetComponent<ActivityGeometry>();

        //Apply processing
        ApplyInsertionProcessing();

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
            CurrentPool = GetCurrentPool();
        }
        else
        {
            CurrentPool = 0;
        }

        ActivityGenerationPool CurrentGenPool = GenerationPools[CurrentPool];

        PossibleActivities = CurrentGenPool._activities;

        float RandomNumber = Random.Range(0.0f, 1.0f);
        float PoolLength = ReachedLastPool ? (NextPoolMinDistance - CurrentGenPool._minDistance) + 999.0f :
            NextPoolMinDistance - CurrentGenPool._minDistance;

        float PlayerPosInPool = _levelSystemsHolder.levelScroller.DistanceTraveled - CurrentGenPool._minDistance;
        float CurrentPoolRatio = PlayerPosInPool / PoolLength;

        for (int i = 0; i < NumberOfActivitiesSpawnedOnStart; ++i)
        {
            ActivityData rdActivity = SelectActivity(CurrentGenPool);
            rdActivity.SetCooldown();

            IncomingActivities.Add(rdActivity.activitySO);

            foreach (var activity in PossibleActivities)
            {
                activity.DecreaseCooldown();
            }
        }
    }

    private List<float> drawnNumbers = new List<float>();
    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Space)) 
    //    {
    //        float sum = drawnNumbers.Sum();
    //        sum/=drawnNumbers.Count;
    //        Debug.Log($"Moyenne des tirs {sum}");
    //    }
    //}

    public ActivityData SelectActivity(ActivityGenerationPool CurrentGenPool)
    {
        //Apply constraints
        ApplyRepetitionConstraints();

        PossibleActivities = CurrentGenPool._activities;

        float RandomNumber = Random.Range(0.0f, 1.0f);
        float PoolLength = NextPoolMinDistance - CurrentGenPool._minDistance;
        float PlayerPosInPool = _levelSystemsHolder.levelScroller.DistanceTraveled - CurrentGenPool._minDistance;
        float CurrentPoolRatio = Mathf.Clamp(PlayerPosInPool / PoolLength, 0.0f, 1.0f);



        float TotalWeight = PossibleActivities.Sum(x => x.GetFinalWeightAt(CurrentPoolRatio));
        float sum = TotalWeight;
        float rdValue = Random.value;
        float rd = rdValue * TotalWeight;
        //Debug.Log($"Random value : {rd} over {TotalWeight}");
        drawnNumbers.Add(rdValue);

        for (int j = 0; j < CurrentGenPool._activities.Count; j++)
        {
            float weight = PossibleActivities[j].GetFinalWeightAt(CurrentPoolRatio);
            if (rd < weight)
            {
                //Debug.Log($"Ratio {CurrentPoolRatio} & rd : {rd} & weight at {weight} (itar : {j}) & sum {TotalWeight}");
                return PossibleActivities[j];
            }
            rd -= weight;
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

    public void ApplyInsertionProcessing()
    {
        foreach (InsertionObstacleProcessor Processor in InsertionProcessors)
        {
            IncomingActivities = Processor.Process(IncomingActivities);
        }
    }

    public void ApplyInsertionProcessingSingle(SOActivity Activity)
    {
        foreach (InsertionObstacleProcessor Processor in InsertionProcessors)
        {
            //Activity = Processor.ProcessSingle(Activity, InstantiatedActivities);
        }
    }

    public void ApplyRepetitionConstraints()
    {
        foreach (RepetitionObstacleConstraint Constraint in RepetitionConstraints)
        {
            if (Constraint.DoesConstraintApply(GenerationPools[CurrentPool]._activities))
            {
                GenerationPools[CurrentPool]._activities.First(x => x.activitySO == Constraint._ActivitySO).SetCooldown(Constraint._ForcedCooldown);
            }
        }
    }

    public int GetCurrentPool()
    {
        float distance = _levelSystemsHolder.levelScroller.DistanceTraveled;
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

    public void CreateNewActivity(GameObject RemovedGO)
    {
        ActivityData NewActivity = SelectActivity(GetActivityGenerationPoolAt(CurrentPool));

        ApplyInsertionProcessingSingle(NewActivity.activitySO);

        InstantiatedActivities.RemoveAt(InstantiatedActivities.Count - 1);
        LeadingActivityGeometry = InstantiatedActivities[0].GetComponent<ActivityGeometry>();
        InstantiatedActivities.Insert(0, Instantiate(NewActivity.activitySO._Geometry.gameObject, LeadingActivityGeometry._tail.gameObject.transform.position, Quaternion.identity));

        Destroy(RemovedGO);

        if (GetCurrentPool() != CurrentPool)
        {
            MakeNewIncomingActivitiesList();
        }
    }
}
