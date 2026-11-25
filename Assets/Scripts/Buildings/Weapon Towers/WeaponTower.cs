using System;
using UnityEngine;

public class WeaponTower : Building
{
    [SerializeField] protected int damage, range;
    [SerializeField] protected float attackSpeed, rotationSpeed;
    [SerializeField] protected GameObject rangeIndicator, projectilePrefab;
    [SerializeField] protected Transform projectileSpawn;
    [SerializeField] protected LayerMask enemyLayer;

    protected Enemy targetEnemy;
    protected float attackCooldown;

    protected override void Init()
    {
        base.Init();

        rangeIndicator.transform.localScale = new Vector3(range*2, 0.01f, range*2);
        rangeIndicator.SetActive(false);
    }

    void Update()
    {
        if (!GameState.Instance.inCombat) return;
        
        attackCooldown -= Time.deltaTime;

        if (targetEnemy != null && Vector3.Distance(transform.position, targetEnemy.transform.position) > range) 
            targetEnemy = null;

        if (targetEnemy == null) GetTarget();

        if (targetEnemy == null) return;
        
        Aim();

        if (attackCooldown <= 0f)
        {
            Attack();
            attackCooldown = 1f / attackSpeed;
        }
    }

    private void GetTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, range, enemyLayer);

        Enemy closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in enemies)
        {
            Enemy enemy = collider.GetComponentInParent<Enemy>();

            if (enemy == null) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        targetEnemy = closestEnemy;
    }

    private void Attack()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawn.position, projectileSpawn.rotation);

        Projectile projectileScript = projectile.GetComponent<Projectile>();

        projectileScript.Initialize(targetEnemy, damage);
    }

    private void Aim()
    {
        Vector3 direction = targetEnemy.transform.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.001f) return;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 
                rotationSpeed * Time.deltaTime);
    }

    public void ToggleRangeIndicator(bool toggle) { rangeIndicator.SetActive(toggle); }

    public int GetDamage() { return damage; }

    public int GetRange() { return range; }

    public float GetAttackSpeed() { return attackSpeed; }

    public override string GetBuildingName() { return null; }
}
