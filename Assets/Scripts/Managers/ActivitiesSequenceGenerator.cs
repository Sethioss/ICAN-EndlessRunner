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

        TestProbabilities(100);

        MakeNewIncomingActivitiesList();

        LeadingActivityObject = Instantiate(LeadingActivityGeometry.gameObject, FirstTileLocation.position, Quaternion.identity);
        InstantiatedActivities.Insert(0, LeadingActivityObject.gameObject);
        LeadingActivityGeometry = LeadingActivityObject.GetComponent<ActivityGeometry>();

        //Apply processing
        ApplyInsertionProcessing();

        SpawnActivitiesGeometry();
    }

    private void TestProbabilities(int numberOfTests)
    {
        ActivityGenerationPool CurrentGenerationPool = GenerationPools[CurrentPool];
        PossibleActivities = CurrentGenerationPool._activities;
        int[] tests = new int[CurrentGenerationPool._activities.Count];

        for (int i = 0; i < numberOfTests; ++i)
        {
            ApplyRepetitionConstraints();

            float RandomNumber = Random.Range(0.0f, 1.0f);

            float CurrentPoolRatio = Mathf.InverseLerp(0, numberOfTests, i);

            float TotalWeight = PossibleActivities.Sum(x => x.GetFinalWeightAt(CurrentPoolRatio));
            float sum = TotalWeight;
            float rdValue = Random.value;
            float rd = rdValue * TotalWeight;

            for (int j = 0; j < CurrentGenerationPool._activities.Count; j++)
            {
                float weight = PossibleActivities[j].GetFinalWeightAt(CurrentPoolRatio);
                if (rd < weight)
                {
                    tests[j] += 1;
                    continue;
                }
                rd -= weight;
            }
        }

        for(int j = 0; j < tests.Length; ++j)
        {
            Debug.Log($"Number of spawned activities {j} : {tests[j]}");
        }
        Debug.Log($"Average: {tests.Sum(x => x) / tests.Length}");
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

        ActivityGenerationPool CurrentGenPool = GenerationPools[CurrentPool];
        PossibleActivities = CurrentGenPool._activities;

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

    public void CreateNewActivity(GameObject RemovedGO)
    {
        if (GetCurrentPool() != CurrentPool)
        {
            GoToNextPool();
            //MakeNewIncomingActivitiesList();
        }

        ActivityData NewActivity = SelectActivity(GetActivityGenerationPoolAt(CurrentPool));

        ApplyInsertionProcessingSingle(NewActivity.activitySO);

        InstantiatedActivities.RemoveAt(InstantiatedActivities.Count - 1);
        LeadingActivityGeometry = InstantiatedActivities[0].GetComponent<ActivityGeometry>();
        InstantiatedActivities.Insert(0, Instantiate(NewActivity.activitySO._Geometry.gameObject, LeadingActivityGeometry._tail.gameObject.transform.position, Quaternion.identity));

        Destroy(RemovedGO);

    }
}
