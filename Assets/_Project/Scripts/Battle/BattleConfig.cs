using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitStats
{
    public BattleUnit prefab;
    public string displayName;
    [Min(1)] public int hp = 10;
    [Min(0)] public int damage = 3;
    [Min(0)] public int armor;
}

[Serializable]
public class WaveDefinition
{
    [Min(0)] public int swarm;
    [Min(0)] public int shooters;
    [Min(0)] public int armored;
}

[CreateAssetMenu(fileName = "BattleConfig", menuName = "Verst/Battle Config")]
public class BattleConfig : ScriptableObject
{
    [Header("Squad")]
    [SerializeField] private UnitStats soldier = new UnitStats { displayName = "боєць", hp = 10, damage = 3 };
    [SerializeField, Min(1)] private int baseSquad = 4;

    [Header("Enemies")]
    [SerializeField] private UnitStats swarm = new UnitStats { displayName = "рій", hp = 3, damage = 2 };
    [SerializeField] private UnitStats shooter = new UnitStats { displayName = "стрілець", hp = 10, damage = 10 };
    [SerializeField] private UnitStats armored = new UnitStats { displayName = "броньований", hp = 12, damage = 3, armor = 3 };

    [Header("Product effects, one wave each")]
    [SerializeField, Min(0)] private int soldiersPerReinforcement = 2;
    [SerializeField, Min(0)] private int armorPerProduct = 6;
    [SerializeField, Min(0)] private int piercePerProduct = 3;

    [Header("Flow")]
    [SerializeField, Min(1)] private int lives = 3;
    [SerializeField, Min(0.1f)] private float tickSec = 0.6f;
    [SerializeField, Min(10)] private int maxTicks = 60;
    [SerializeField, Min(0f)] private float resultPauseSec = 2f;

    [SerializeField] private List<WaveDefinition> waves = new List<WaveDefinition>
    {
        new WaveDefinition { swarm = 5 },
        new WaveDefinition { shooters = 4 },
        new WaveDefinition { armored = 3, swarm = 2 },
        new WaveDefinition { swarm = 12 },
        new WaveDefinition { armored = 2, shooters = 3, swarm = 3 }
    };

    public UnitStats Soldier => soldier;
    public int BaseSquad => baseSquad;
    public UnitStats Swarm => swarm;
    public UnitStats Shooter => shooter;
    public UnitStats Armored => armored;
    public int SoldiersPerReinforcement => soldiersPerReinforcement;
    public int ArmorPerProduct => armorPerProduct;
    public int PiercePerProduct => piercePerProduct;
    public int Lives => lives;
    public float TickSec => tickSec;
    public int MaxTicks => maxTicks;
    public float ResultPauseSec => resultPauseSec;
    public IReadOnlyList<WaveDefinition> Waves => waves;
}
