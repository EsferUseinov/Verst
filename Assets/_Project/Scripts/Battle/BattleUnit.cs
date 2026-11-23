using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private LineRenderer tracer;
    [SerializeField] private float muzzleHeight = 0.04f;
    [SerializeField] private float tracerTime = 0.12f;

    private float tracerOffAt;

    public int Hp { get; private set; }
    public int Damage { get; private set; }
    public int Armor { get; private set; }
    public bool Alive => Hp > 0;

    private Vector3 Muzzle => transform.position + Vector3.up * muzzleHeight;

    public void Init(UnitStats stats, int bonusArmor)
    {
        Hp = stats.hp;
        Damage = stats.damage;
        Armor = stats.armor + bonusArmor;
        tracer.enabled = false;
    }

    public void ShootAt(BattleUnit target)
    {
        tracer.SetPosition(0, Muzzle);
        tracer.SetPosition(1, target.Muzzle);
        tracer.enabled = true;
        tracerOffAt = Time.time + tracerTime;
    }

    public void TakeDamage(int amount)
    {
        Hp -= amount;
        if (Hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (tracer.enabled && Time.time >= tracerOffAt)
        {
            tracer.enabled = false;
        }
    }
}
