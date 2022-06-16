using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class Camp : MonoBehaviour
{
    public const float waveDelay = 12;

    public CampsManager manager;
    public InvasionProgress progress;
    public BigPopupUI bigPopup;
    public SmallPopupUI smallPopup;
    public Wave[] waves;
    public Transform[] spawnPoints;

    public GameObject enemyHolder;

    public int currentWave;
    public float minStartDistance = 20;
    public float sphereRadius = 5;

    public VisualEffect campFire;

    Transform player;

    bool active;
    private void Start()
    {
        player = PlayerMovement.player.transform;
    }
    void Update()
    {
        if(!active && Vector3.Distance(player.position,transform.position) < minStartDistance && !manager.invasionActive)
        {
            OnStartInvasion();
        }

        if (active)
        {
            UpdateInvasionProgress();
        }
    }

    async void OnStartInvasion()
    {
        //Start of invasion
        active = true;
        manager.invasionActive = true;
        print("Invasion Started!");

        string mainText = $"Invading Camp {manager.campsCompleted + 1}!";
        string secondaryText = $"Camp {manager.campsCompleted + 1} / {manager.camps.Length}";

        bigPopup.Popup(mainText,secondaryText);
        await Task.Delay(Milliseconds(bigPopup.actionTime + bigPopup.strengthWaitTime + 1));

        progress.AssignCamp(this);

        //While invasion is happening
        for (int i = 0; i < waves.Length; i++)
        {
            progress.SwitchProgressMode(ProgressMode.WaveProgression);
            //begin of wave
            Wave currentWave = waves[i];
            this.currentWave = i + 1;

            currentWave.OnStartWave();
            print($"Wave {this.currentWave} has started!");

            //Spawn a enemy on all spawnpoints

            while (currentWave.EnemiesToSpawn > 0)
            {
                if (!Application.isPlaying) return; //prevent enemies from spawning in edit mode

                for (int j = 0; j < spawnPoints.Length; j++)
                {
                    if (currentWave.EnemiesToSpawn <= 0) break;
                    int enemyIndex = currentWave.GetRandomEnemyIndex();

                    BaseEnemy enemy = Instantiate(enemyHolder, spawnPoints[j].position, spawnPoints[j].rotation).GetComponent<BaseEnemy>();
                    enemy.InitializeEnemy(currentWave.enemies[enemyIndex].enemyPrefab,currentWave);

                    currentWave.enemies[enemyIndex].amount--;
                }

                await Task.Delay(Milliseconds(Wave.enemySpawnDelay));
            }

            //Wait till the wave is over
            while (currentWave.Progress != 1) 
            {
                await Task.Yield(); 
            }
            //End of a wave

            string text = $"Wave { this.currentWave} / {waves.Length} completed!";

            float waitTime = smallPopup.strengthWaitTime + smallPopup.popupTime;

            if (this.currentWave <= waves.Length) 
            {
                smallPopup.Popup(text);
                progress.SwitchProgressMode(ProgressMode.TimeBetweenWaves);
                progress.SetTimer(waitTime + Camp.waveDelay);
            }

            await Task.Delay(Milliseconds(waitTime));

            // Finish the invasion
            if (this.currentWave >= waves.Length)
            {
                OnInvasionEnd();
                return;
            }

            // Continue to the next wave
            print($"Finished Wave{ this.currentWave}, prepare for Wave{this.currentWave + 1}");
            await Task.Delay(Milliseconds(waveDelay));
        }
    }

    void UpdateInvasionProgress()
    {

    }

    void OnInvasionEnd()
    {
        manager.campsCompleted++;
        manager.invasionActive = false;

        string mainText = $"Finished invading camp {manager.campsCompleted}!";
        string secondaryText = $"{manager.CampsToGo} camps to go";
        bigPopup.Popup(mainText, secondaryText);

        progress.AssignCamp(null);
        campFire.Stop();
    }

    public int Milliseconds(float value)
    {
        return Mathf.RoundToInt(value * 1000);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(spawnPoints[i].position, sphereRadius);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minStartDistance);
    }
}

[System.Serializable]
public class Wave
{
    public WaveEnemy[] enemies;
    public WaveMultiplier multiplier = WaveMultiplier.Standard;
    public const float enemySpawnDelay = 0.3F;
    [HideInInspector]public int killed;

    /// <summary>
    /// Value is in between 0 - 1
    /// </summary>
    public float Progress { get { return Mathf.InverseLerp(0,  maxEnemyCount, killed); } }

    public int EnemiesToSpawn {get {
            int value = 0;
            for (int i = 0; i < enemies.Length; i++)
            {
                value += enemies[i].amount;
            }
            return value;
    } }

    int maxEnemyCount;

    public void OnStartWave()
    {
        //Determine max enemy amount
        for (int i = 0; i < enemies.Length; i++)
        {
            maxEnemyCount += enemies[i].amount;
        }
    }

    public int GetRandomEnemyIndex()
    {
        int index = -1;
        bool validIndex = false;

        while(validIndex == false)
        {
            index = Random.Range(0, enemies.Length);
            if (enemies[index].CanSpawn) validIndex = true;
        }
        return index;
    }
}

[System.Serializable]
public struct WaveEnemy
{
    public EnemyData enemyPrefab;
    public int amount;

    public bool CanSpawn { get { return amount > 0; } }

}

[System.Serializable]
public struct WaveMultiplier
{
    public float damage;
    public float hp;
    public float resistance;
    public float movementSpeed;
    public float rotationSpeed;
    public float dataDrops;
    
    /// <summary>
    /// Creates a group of multipliers used in waves to make enemies stronger or weaker
    /// </summary>
    /// <param name="damage">Multiplies the amount of damage the enemy does</param>
    /// <param name="hp">Multiplies the amount of hitpoints the enemy has</param>
    /// <param name="resistance">Multiplies the amount of resistance the enemy has</param>
    /// <param name="movementSpeed">Multiplies the movement speed of the enemy</param>
    /// <param name="rotationSpeed">Multiplies the rotation speed of the enemy</param>
    /// <param name="dataDrops">Multiplies the amount of data the enemy drops on death</param>
    public WaveMultiplier(float damage, float hp, float resistance, float movementSpeed, float rotationSpeed, float dataDrops)
    {
        this.damage = damage;
        this.hp = hp;
        this.resistance = resistance;
        this.movementSpeed = movementSpeed;
        this.rotationSpeed = rotationSpeed;
        this.dataDrops = dataDrops;
    }

    /// <summary>
    /// Creates a group of multipliers used in waves to make enemies stronger or weaker
    /// </summary>
    /// <param name="baseMultiplier"> All multipliers are baseMultiplier</param>
    public WaveMultiplier(float baseMultiplier)
    {
        damage = baseMultiplier;
        hp = baseMultiplier;
        resistance = baseMultiplier;
        movementSpeed = baseMultiplier;
        rotationSpeed = baseMultiplier;
        dataDrops = baseMultiplier;
    }

    public static WaveMultiplier Standard { get { return new WaveMultiplier(1); } }
}
