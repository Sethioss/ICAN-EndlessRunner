using FMOD.Studio;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActivitiesSequenceGenerator : MonoBehaviour
{
    [SerializeField] Transform FirstTileLocation;
    [HideInInspector] public int Temp = 0;

    [SerializeField] public int NumberOfActivitiesSpawnedOnStart = 5;

    //New activity system
    [SerializeField] private LevelSystemsHolder _levelSystemsHolder;
    [SerializeField] private ActivityGeometry LeadingActivityGeometry;
    [HideInInspector] public GameObject LeadingActivityObject;

    private List<ActivityData> PossibleActivities = new List<ActivityData>();
    private List<SOActivity> IncomingActivities = new List<SOActivity>();
    private List<SOActivity> WaitingActivities = new List<SOActivity>();
    /*[HideInInspector]*/
    public List<GameObject> InstantiatedActivities = new List<GameObject>();
    public List<GameObject> PassedActivities = new List<GameObject>();

    [SerializeField] private List<RepetitionObstacleConstraint> RepetitionConstraints;
    [SerializeField] private List<ActivityGenerationPool> GenerationPools = new List<ActivityGenerationPool>();
    [SerializeField] private List<InsertionObstacleProcessor> InsertionProcessors = new List<InsertionObstacleProcessor>();

    int CurrentPool = 0;
    int AccumulatedDistance = 0;

    int NextPoolStart = 0;
    bool ReachedLastPool = false;

    // Start is called before the first frame update
    void Start()
    {
        ReachedLastPool = GetReachedLastPool();
        if (!ReachedLastPool)
        {
            NextPoolStart = NextPoolStart + GenerationPools[CurrentPool]._poolLength;
        }

        LeadingActivityObject = Instantiate(LeadingActivityGeometry.gameObject, FirstTileLocation.position, Quaternion.identity);

        MakeNewIncomingActivitiesList(true);

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

    public void MakeNewIncomingActivitiesList(bool FirstSpawn = false)
    {
        if (GetCurrentPool() != CurrentPool)
        {
            GoToNextPool();
        }

        ActivityGenerationPool CurrentGenPool = GenerationPools[CurrentPool];
        PossibleActivities = CurrentGenPool._activities;

        int NumberToGenerate = FirstSpawn ? NumberOfActivitiesSpawnedOnStart : PassedActivities.Count;
        for (int i = 0; i < NumberToGenerate; ++i)
        {
            ActivityData rdActivity = SelectActivity(CurrentGenPool);
            rdActivity.SetCooldown();

            WaitingActivities.Add(rdActivity.activitySO);
            //IncomingActivities.Add(rdActivity.activitySO);

            foreach (var activity in PossibleActivities)
            {
                activity.DecreaseCooldown();
            }
        }

        ApplyInsertionProcessing();
    }

    public ActivityData SelectActivity(ActivityGenerationPool CurrentGenPool)
    {
        //Apply constraints
        ApplyRepetitionConstraints();

        PossibleActivities = CurrentGenPool._activities;

        float RandomNumber = Random.Range(0.0f, 1.0f);

        float CurrentPoolRatio = Mathf.InverseLerp(AccumulatedDistance, NextPoolStart, _levelSystemsHolder.levelScroller.DistanceTraveled);

        float TotalWeight = PossibleActivities.Sum(x => x.GetFinalWeightAt(CurrentPoolRatio));
        float sum = TotalWeight;
        float rdValue = Random.value;
        float rd = rdValue * TotalWeight;

        for (int j = 0; j < CurrentGenPool._activities.Count; j++)
        {
            float weight = PossibleActivities[j].GetFinalWeightAt(CurrentPoolRatio);
            if (rd < weight)
            {
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
        for (int i = 0; i < WaitingActivities.Count; ++i)
        {
            InstantiatedActivities.Insert(0, Instantiate(WaitingActivities[i]._Geometry.gameObject, LeadingActivityObject.GetComponent<ActivityGeometry>()._tail.gameObject.transform.position, LeadingActivityObject.GetComponent<ActivityGeometry>()._tail.gameObject.transform.rotation).gameObject);
            LeadingActivityObject = InstantiatedActivities[0];
        }
        WaitingActivities.Clear();
    }

    public void ApplyInsertionProcessing()
    {
        foreach (InsertionObstacleProcessor Processor in InsertionProcessors)
        {
            IncomingActivities = Processor.Process(WaitingActivities);
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

        if (distance >= NextPoolStart)
        {
            if (CurrentPool + 1 < GenerationPools.Count)
            {
                return CurrentPool + 1;
            }
            else
            {
                return 0;
            }
        }

        return CurrentPool;

    }

    public void GoToNextPool()
    {
        AccumulatedDistance += GenerationPools[CurrentPool]._poolLength;

        if (!GetReachedLastPool())
        {
            CurrentPool += 1;
        }
        else
        {
            CurrentPool = 0;
        }

        NextPoolStart += GenerationPools[CurrentPool]._poolLength;
    }

    public void DeleteActivity(GameObject RemovedGO)
    {
        InstantiatedActivities.RemoveAt(InstantiatedActivities.Count - 1);
        PassedActivities.Insert(0, RemovedGO);
        RemovedGO.SetActive(false);

        if (PassedActivities.Count > NumberOfActivitiesSpawnedOnStart / 2)
        {
            MakeNewIncomingActivitiesList(false);

            for (int i = 0; i < PassedActivities.Count; ++i)
            {
                Destroy(PassedActivities[i].gameObject);
            }
            PassedActivities.Clear();

            ApplyInsertionProcessing();

            SpawnActivitiesGeometry();
        }


    }
    public void CreateNewActivity(GameObject RemovedGO)
    {
        if (GetCurrentPool() != CurrentPool)
        {
            GoToNextPool();
        }

        ActivityData NewActivity = SelectActivity(GetActivityGenerationPoolAt(CurrentPool));

        ApplyInsertionProcessingSingle(NewActivity.activitySO);

        LeadingActivityGeometry = InstantiatedActivities[0].GetComponent<ActivityGeometry>();
        InstantiatedActivities.Insert(0, Instantiate(NewActivity.activitySO._Geometry.gameObject, LeadingActivityGeometry._tail.gameObject.transform.position, Quaternion.identity));
        InstantiatedActivities.RemoveAt(InstantiatedActivities.Count - 1);

        PassedActivities.Insert(0, RemovedGO);
        RemovedGO.SetActive(false);

        if (PassedActivities.Count > NumberOfActivitiesSpawnedOnStart / 2)
        {
            MakeNewIncomingActivitiesList();
            SpawnActivitiesGeometry();

            for(int i = 0; i < PassedActivities.Count; ++i)
            {
                Destroy(PassedActivities[i].gameObject);
            }
            PassedActivities.Clear();
        }

    }
}
