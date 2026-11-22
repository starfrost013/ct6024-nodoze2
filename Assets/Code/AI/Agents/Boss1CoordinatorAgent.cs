using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;


/// <summary>
/// GOAP Boss1 coordinator (Nov 12, 2026)
/// Finally implemented Nov 22, 2026
/// </summary>
class Boss1CoordinatorAgent : Agent
{
    /// <summary>
    /// the internal coordinator structure that holds the data that is used to coordinate the wreckfest style agents
    /// </summary>
    internal struct BossCoordinator
    {
        internal int maxNumOfCars;
        internal int carSpawnTime;

        /// <summary>
        /// (maxNumOfCars / carSpawnTIme)
        /// </summary>
        private Timer carSpawnTimer; 

    };

    enum BossPhase
    {
        Spawn,
        Phase1,
        Phase2,
        Phase3,
        Phase4,
        Beaten, 
    };
    
    private BossPhase phase = BossPhase.Spawn; 
    private BossCoordinator coordinator = new();
    private List<EnemyCarAgent> enemyCars = new();
    private int nextCarToSpawn = 0;
    private Timer spawnPhaseTimer = new();
    private const string BOSS_COORDINATOR_CONFIG_PATH = "Races/Boss1Coordinator";
    private TextAsset bossCoordinatorText = null;

    internal override void Start()
    {
        base.Start();
        LoadConfig();
    }

    private void LoadConfig()
    {
        bossCoordinatorText = AssetManager.LoadAsset<TextAsset>(BOSS_COORDINATOR_CONFIG_PATH);

        ConfigParser.Parse(bossCoordinatorText.text);

        bool success = int.TryParse(ConfigParser.GetValue("Spawn", "NumOfCars"), out coordinator.maxNumOfCars)
        | int.TryParse(ConfigParser.GetValue("Spawn", "CarSpawnTime"), out coordinator.carSpawnTime);

        if (!success)
            Debug.LogWarning("Boss1CoordinatorAgent::LoadConfig - failed to load some parameters! Maybe it doesn't matter");
    }

    /// <summary>
    /// handle the GOAP spawn phase
    /// </summary>
    private void HandleSpawnPhase()
    {
        if (!spawnPhaseTimer.HasStarted())
            spawnPhaseTimer.Start(coordinator.carSpawnTime);
        else
        {
            if (spawnPhaseTimer.GetElapsedTime() > coordinator.carSpawnTime * ((float)nextCarToSpawn) / (float)coordinator.maxNumOfCars)
            {
                EnemyCarAgent carAgent = new();
                carAgent.Spawn(GameManager.player.carInWorld);
                enemyCars.Add(carAgent);
                nextCarToSpawn++;
            }
        }
    }

    internal override void FixedUpdate()
    {
        switch (phase)
        {
            case BossPhase.Spawn:
                HandleSpawnPhase();
                break;
        }
    }
}