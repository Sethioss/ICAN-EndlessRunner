using UnityEngine;

[RequireComponent (typeof(HealthComponent))]
public class Player : MonoBehaviour
{
    [SerializeField] CoinStreakManager coinStreakManager;
    [SerializeField] OnSplineMovementController MovementController;
    [SerializeField] LevelScroller LevelScroller;

    [SerializeField] public MeshRenderer PlayerMesh;
    [SerializeField] Material StandardMat;
    [SerializeField] Material HurtMat;
    [SerializeField] Material RichMat;

    private HealthComponent healthComponent;

    bool isSpecialStatus = false;
    bool invincible = false;
    [SerializeField] float specialStatusTime;
    [SerializeField] float hitCooldownTime;
    float tempSpecialStatusTime;
    float tempHitCooldownTime;

    //TODO: Put this in a component, interface or system that can easily be placed in multiple scripts ("GameStateDependantExecution?")
    private bool AllowedToProceed = true;

    protected virtual void OnEnable()
    {
        GameManager.GetInstance()._onPause.AddListener(() => AllowedToProceed = false);
        GameManager.GetInstance()._onDeath.AddListener(() => AllowedToProceed = false);
        GameManager.GetInstance()._onResume.AddListener(() => AllowedToProceed = true);
    }

    private void OnDisable()
    {
        GameManager.GetInstance()._onPause.RemoveListener(() => AllowedToProceed = false);
        GameManager.GetInstance()._onDeath.RemoveListener(() => AllowedToProceed = false);
        GameManager.GetInstance()._onResume.RemoveListener(() => AllowedToProceed = true);
    }

    private void Start()
    {
        //PlayerMesh.material = StandardMat;
        healthComponent = GetComponent<HealthComponent>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Obstacle"))
        {
            if(!invincible)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Player/SOUND-HitPlayer");
                //PlayerMesh.material = HurtMat;
                LevelScroller.SlowLevelBecauseOfHit();

                tempSpecialStatusTime = 0;
                tempHitCooldownTime = 0;

                isSpecialStatus = true;
                invincible = true;

                healthComponent.RemoveHealth();
                if (healthComponent.IsDead())
                {
                    GameManager _gm = GameManager.GetInstance();
                    if (_gm)
                    {
                        GameManager.GetInstance().KillPlayer();
                    }
                    else
                    {
                        Debug.LogError("Attempting to acces GameManager but no Game Manager is present in the scene!");
                    }
                }

            }

            other.gameObject.SetActive(healthComponent.IsDead());
        }

        if (other.gameObject.CompareTag("Bonus"))
        {
            isSpecialStatus = true;
            tempSpecialStatusTime = 0;
            other.gameObject.SetActive(false);
            coinStreakManager.PickupCoin(other.GetComponent<Coin>());
            //PlayerMesh.material = RichMat;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //PlayerMesh.material = StandardMat;
    }

    private void Update()
    {
        if(AllowedToProceed)
        {
            if (isSpecialStatus)
            {
                tempSpecialStatusTime += Time.deltaTime;
                if (tempSpecialStatusTime > specialStatusTime)
                {
                    tempSpecialStatusTime = 0;
                    isSpecialStatus = false;
                    //PlayerMesh.material = StandardMat;
                }
            }

            if (invincible)
            {
                tempHitCooldownTime += Time.deltaTime;
                if (tempHitCooldownTime > hitCooldownTime)
                {
                    tempHitCooldownTime = 0;
                    invincible = false;
                    //PlayerMesh.material = StandardMat;
                }
            }
        }      
    }
}
