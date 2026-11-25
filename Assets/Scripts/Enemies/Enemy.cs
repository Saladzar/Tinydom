using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int health = 100, speed = 3, damage = 10;
    [SerializeField] protected float attackCooldown = 1.5f, attackRange = 1.2f, tileDistance = 0.15f, 
            separationRadius = 1.5f, separationStrength = 2f;
    [SerializeField] protected Rigidbody body;
    [SerializeField] protected LayerMask enemyLayer;

    protected int currentHealth;
    protected float cooldown = 0;
    protected Building targetBuilding;
    protected List<Vector2Int> path;
    protected int currentPathIndex;

    protected Collider[] nearbyEnemies = new Collider[20];

    protected GridManager grid;

    protected void Init()
    {
        currentHealth = health;
        grid = GridManager.Instance;
    }

    protected void SetPath(List<Vector2Int> path)
    {
        this.path = path;
        currentPathIndex = 0;
    }

    protected void MoveTowards(Vector3 destination)
    {
        Vector3 offset = destination - body.position;
        offset.y = 0;

        if (offset.magnitude <= tileDistance)
        {
            currentPathIndex++;
            return;
        }

        ApplyMovement(offset);
    }

    protected void ApplyMovement(Vector3 offset)
    {
        Vector3 desiredDirection = offset.normalized;
        Vector3 separation = CalculateSeparation();
        Vector3 movementDirection = (desiredDirection + separation * separationStrength).normalized;
        Vector3 movement = movementDirection * speed * Time.fixedDeltaTime;

        body.MovePosition(body.position + movement);
    }

    protected void HandleBuilding(Building building)
    {
        Vector3 offset = body.position - building.transform.position;
        offset.y = 0;

        float distance = offset.magnitude;

        if (distance > attackRange)
        {
            if (distance < 0.001f) offset = -transform.forward;

            Vector3 attackPosition = building.transform.position + offset.normalized * attackRange;

            ApplyMovement(attackPosition - body.position);
            return;
        }

        Attack(building);
    }

    protected void Attack(Building building)
    {
        cooldown -= Time.fixedDeltaTime;

        if (building == null || building.isDestroyed) return;

        if (cooldown <= 0)
        {
            building.TakeDamage(damage);
            //Play animation
            Debug.Log("Take that!");
            cooldown = attackCooldown; 
        }
    }

    protected Vector3 CalculateSeparation()
    {
        Vector3 separation = Vector3.zero;

        int count = Physics.OverlapSphereNonAlloc(body.position, separationRadius, nearbyEnemies, enemyLayer);

        int neighbours = 0;
        for (int i = 0; i < count; i++)
        {
            Enemy other = nearbyEnemies[i].GetComponentInParent<Enemy>();

            if (other == null || other == this) continue;

            Vector3 offset = body.position - other.body.position;
            offset.y = 0;

            float distance = offset.magnitude;

            if (distance <= 0.001f) continue;

            separation += offset.normalized / distance;
            neighbours++;
        }

        if (neighbours > 0) separation /= neighbours;

        return separation;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0) HandleDeath();
    }

    protected void HandleDeath()
    {
        EnemySpawner.Instance.RemoveEnemy(this);
        Destroy(gameObject);
    }
}
