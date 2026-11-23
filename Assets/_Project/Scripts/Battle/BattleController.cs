using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public enum BattleState
{
    Ready,
    Fighting,
    Victory,
    Defeat
}

public class BattleController : MonoBehaviour
{
    [SerializeField] private BattleConfig config;
    [SerializeField] private ShipmentZone shipment;
    [SerializeField] private RawSupply supply;

    [Header("Table")]
    [SerializeField] private Transform allyArea;
    [SerializeField] private Transform enemyArea;
    [SerializeField] private float spacing = 0.08f;
    [SerializeField, Min(1)] private int rowLength = 6;

    [Header("Feedback")]
    [SerializeField] private TMP_Text board;
    [SerializeField] private Renderer[] lifeLamps;
    [SerializeField] private CraftButtonFeedback startButton;
    [SerializeField] private Color lifeOnColor = new Color(0.2f, 0.8f, 0.3f);
    [SerializeField] private Color lifeOffColor = new Color(0.25f, 0.2f, 0.2f);

    private readonly List<BattleUnit> allies = new List<BattleUnit>();
    private readonly List<BattleUnit> enemies = new List<BattleUnit>();
    private readonly List<(BattleUnit target, int damage)> hits = new List<(BattleUnit, int)>();

    private int waveIndex;
    private int lives;
    private int wonWaves;
    private string lastResult = "";
    private bool skipRequested;

    public BattleState State { get; private set; }

    private void Awake()
    {
        ResetGame();
    }

    private void OnEnable()
    {
        shipment.Changed += RefreshBoard;
    }

    private void OnDisable()
    {
        shipment.Changed -= RefreshBoard;
    }

    public void StartBattle()
    {
        if (State == BattleState.Fighting)
        {
            if (!skipRequested)
            {
                skipRequested = true;
                startButton.ShowCrafted();
                RefreshBoard();
            }
            return;
        }

        startButton.ShowCrafted();
        if (State != BattleState.Ready)
        {
            ResetGame();
            supply.Refill();
            return;
        }

        StartCoroutine(RunWave(shipment.TakeCargo()));
    }

    private void ResetGame()
    {
        waveIndex = 0;
        lives = config.Lives;
        wonWaves = 0;
        lastResult = "";
        State = BattleState.Ready;
        RefreshBoard();
    }

    private IEnumerator RunWave(List<ItemDefinition> cargo)
    {
        State = BattleState.Fighting;
        skipRequested = false;
        WaveDefinition wave = config.Waves[waveIndex];

        int reinforcements = CountEffect(cargo, BattleEffect.Reinforcement);
        int armorBonus = CountEffect(cargo, BattleEffect.Armor) * config.ArmorPerProduct;
        int pierce = CountEffect(cargo, BattleEffect.Piercing) * config.PiercePerProduct;

        SpawnGroup(allies, allyArea, config.Soldier, config.BaseSquad + reinforcements * config.SoldiersPerReinforcement, armorBonus, 0);
        int placed = SpawnGroup(enemies, enemyArea, config.Swarm, wave.swarm, 0, 0);
        placed = SpawnGroup(enemies, enemyArea, config.Shooter, wave.shooters, 0, placed);
        SpawnGroup(enemies, enemyArea, config.Armored, wave.armored, 0, placed);

        lastResult = $"Посилення: {DescribeCargo(cargo)}";
        RefreshBoard();

        int ticks = 0;
        while (AnyAlive(allies) && AnyAlive(enemies) && ticks < config.MaxTicks)
        {
            if (!skipRequested)
            {
                yield return new WaitForSeconds(config.TickSec);
            }
            Exchange(pierce);
            ticks++;
        }

        bool won = AnyAlive(allies) && !AnyAlive(enemies);
        yield return new WaitForSeconds(config.ResultPauseSec);
        ClearUnits();
        FinishWave(won);
    }

    private void Exchange(int pierce)
    {
        hits.Clear();
        foreach (BattleUnit ally in allies)
        {
            BattleUnit target = PickTarget(ally, enemies);
            if (target != null)
            {
                hits.Add((target, Mathf.Max(0, ally.Damage - Mathf.Max(0, target.Armor - pierce))));
            }
        }

        foreach (BattleUnit enemy in enemies)
        {
            BattleUnit target = PickTarget(enemy, allies);
            if (target != null)
            {
                hits.Add((target, Mathf.Max(1, enemy.Damage - target.Armor)));
            }
        }

        foreach ((BattleUnit target, int damage) in hits)
        {
            target.TakeDamage(damage);
        }
    }

    private static BattleUnit PickTarget(BattleUnit shooter, List<BattleUnit> targets)
    {
        if (!shooter.Alive)
        {
            return null;
        }

        int alive = 0;
        foreach (BattleUnit unit in targets)
        {
            if (unit.Alive)
            {
                alive++;
            }
        }
        if (alive == 0)
        {
            return null;
        }

        int pick = Random.Range(0, alive);
        foreach (BattleUnit unit in targets)
        {
            if (unit.Alive && pick-- == 0)
            {
                shooter.ShootAt(unit);
                return unit;
            }
        }
        return null;
    }

    private void FinishWave(bool won)
    {
        waveIndex++;
        if (won)
        {
            wonWaves++;
            lastResult = $"Хвилю {waveIndex} відбито";
        }
        else
        {
            lives--;
            lastResult = $"Хвилю {waveIndex} програно, втрачено життя";
        }

        if (lives <= 0)
        {
            State = BattleState.Defeat;
        }
        else if (waveIndex >= config.Waves.Count)
        {
            State = BattleState.Victory;
        }
        else
        {
            State = BattleState.Ready;
            supply.Deliver();
        }
        RefreshBoard();
    }

    private int SpawnGroup(List<BattleUnit> group, Transform area, UnitStats stats, int count, int bonusArmor, int startSlot)
    {
        for (int i = 0; i < count; i++)
        {
            int slot = startSlot + i;
            int column = slot % rowLength;
            int row = slot / rowLength;
            Vector3 position = area.position
                + area.right * ((column - (rowLength - 1) * 0.5f) * spacing)
                - area.forward * (row * spacing);

            BattleUnit unit = Instantiate(stats.prefab, position, area.rotation, area);
            unit.Init(stats, bonusArmor);
            group.Add(unit);
        }
        return startSlot + count;
    }

    private void ClearUnits()
    {
        foreach (BattleUnit unit in allies)
        {
            Destroy(unit.gameObject);
        }
        foreach (BattleUnit unit in enemies)
        {
            Destroy(unit.gameObject);
        }
        allies.Clear();
        enemies.Clear();
    }

    private static bool AnyAlive(List<BattleUnit> group)
    {
        foreach (BattleUnit unit in group)
        {
            if (unit.Alive)
            {
                return true;
            }
        }
        return false;
    }

    private static int CountEffect(List<ItemDefinition> cargo, BattleEffect effect)
    {
        int count = 0;
        foreach (ItemDefinition definition in cargo)
        {
            if (definition.BattleEffect == effect)
            {
                count++;
            }
        }
        return count;
    }

    private void RefreshBoard()
    {
        int total = config.Waves.Count;
        StringBuilder text = new StringBuilder();

        switch (State)
        {
            case BattleState.Ready:
                text.AppendLine($"<b>Хвиля {waveIndex + 1} з {total}</b>");
                text.AppendLine($"Вороги: {DescribeWave(config.Waves[waveIndex])}");
                text.AppendLine($"Поставка: {DescribeCargo(CurrentCargo())}");
                if (lastResult.Length > 0)
                {
                    text.AppendLine(lastResult);
                }
                text.AppendLine("Натисни «До битви», коли будеш готовий");
                break;
            case BattleState.Fighting:
                text.AppendLine($"<b>Хвиля {waveIndex + 1} з {total}: бій</b>");
                text.AppendLine(lastResult);
                text.AppendLine(skipRequested ? "Бій дораховується..." : "Натисни «До битви» ще раз, щоб завершити бій одразу");
                break;
            case BattleState.Victory:
                text.AppendLine("<b>Перемога!</b>");
                text.AppendLine($"Відбито хвиль: {wonWaves} з {total}");
                text.AppendLine("Натисни «До битви», щоб почати знову");
                break;
            case BattleState.Defeat:
                text.AppendLine("<b>Поразка</b>");
                text.AppendLine($"Відбито хвиль: {wonWaves} з {total}");
                text.AppendLine("Натисни «До битви», щоб почати знову");
                break;
        }

        text.Append("<size=70%>Привід: +бійці · Редуктор: броня · Манометр: пробиття броні</size>");
        board.text = text.ToString();

        for (int i = 0; i < lifeLamps.Length; i++)
        {
            lifeLamps[i].material.color = i < lives ? lifeOnColor : lifeOffColor;
        }
        startButton.SetReady(!skipRequested || State != BattleState.Fighting);
    }

    private List<ItemDefinition> CurrentCargo()
    {
        List<ItemDefinition> cargo = new List<ItemDefinition>();
        foreach (Item item in shipment.Cargo)
        {
            if (item != null)
            {
                cargo.Add(item.Definition);
            }
        }
        return cargo;
    }

    private string DescribeWave(WaveDefinition wave)
    {
        List<string> parts = new List<string>();
        AddPart(parts, config.Swarm.displayName, wave.swarm);
        AddPart(parts, config.Shooter.displayName, wave.shooters);
        AddPart(parts, config.Armored.displayName, wave.armored);
        return string.Join(", ", parts);
    }

    private static string DescribeCargo(List<ItemDefinition> cargo)
    {
        if (cargo.Count == 0)
        {
            return "порожньо";
        }

        Dictionary<string, int> counts = new Dictionary<string, int>();
        foreach (ItemDefinition definition in cargo)
        {
            counts.TryGetValue(definition.DisplayName, out int count);
            counts[definition.DisplayName] = count + 1;
        }

        List<string> parts = new List<string>();
        foreach (KeyValuePair<string, int> pair in counts)
        {
            AddPart(parts, pair.Key, pair.Value);
        }
        return string.Join(", ", parts);
    }

    private static void AddPart(List<string> parts, string name, int count)
    {
        if (count > 0)
        {
            parts.Add($"{name} ×{count}");
        }
    }
}
