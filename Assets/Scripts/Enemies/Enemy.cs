using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected int health = 100, speed = 3, damage = 10, crystalReward;
    [SerializeField] protected float attackSpeed = 1.5f, attackRange = 1.2f, tileDistance = 0.15f, 
            separationRadius = 1.5f, separationStrength = 1f, rotationSpeed = 3f;
    [SerializeField] protected Rigidbody body;
    [SerializeField] protected LayerMask enemyLayer;

    protected int currentHealth;
    protected bool isDead;
    protected float attackCooldown = 0;
    protected Building targetBuilding;
    protected List<Vector2Int> path;
    protected int currentPathIndex;

    protected Collider[] nearbyEnemies = new Collider[20];

    protected GridManager grid;
    protected Animator animator;

    protected void Init()
    {
        currentHealth = health;
        grid = GridManager.Instance;
        animator = GetComponentInChildren<Animator>();
    }

    public abstract void Pathfind();

    protected void SetPath(List<Vector2Int> path)
    {
        this.path = path;
        currentPathIndex = path != null && path.Count > 1 ? 1 : 0;
    }

    protected void PathfindToTarget()
    {
        if (grid == null)
            grid = GridManager.Instance;

        if (grid == null)
            throw new InvalidOperationException($"{GetType().Name} requires a GridManager.");

        if (targetBuilding == null || targetBuilding.isDestroyed)
        {
            SetPath(null);
            return;
        }

        Vector2Int? targetPosition = grid.WorldToGridPosition(targetBuilding.transform.position);
        if (!targetPosition.HasValue)
        {
            SetPath(null);
            Debug.LogError($"{GetType().Name} target '{targetBuilding.name}' is outside the grid.");
            return;
        }

        if (Pathfinding.Instance == null)
            throw new InvalidOperationException($"{GetType().Name} requires a Pathfinding instance.");

        Vector2Int startPosition = grid.ClampToGridPosition(transform.position);
        SetPath(Pathfinding.Instance.FindPath(startPosition, targetPosition.Value));
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

    protected void ApplyMovement(Vector3 offset, bool includeSeparation = true)
    {
        Vector3 desiredDirection = offset.normalized;
        Vector3 separation = includeSeparation ? CalculateSeparation() : Vector3.zero;
        Vector3 movementDirection = (desiredDirection + separation * separationStrength).normalized;
        Vector3 movement = movementDirection * speed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.LookRotation(movementDirection);
        
        animator.SetBool("isRunning", true);

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

            ApplyMovement(attackPosition - body.position, includeSeparation: false);
            return;
        }

        animator.SetBool("isRunning", false);
        Attack(building);
    }

    protected void Attack(Building building)
    {
        attackCooldown -= Time.fixedDeltaTime;

        if (building == null || building.isDestroyed) return;

        if (attackCooldown <= 0)
        {
            building.TakeDamage(damage);
            animator.SetTrigger("Attack");
            Debug.Log("Take that!");
            attackCooldown = attackSpeed; 
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
        if (isDead) return;

        currentHealth -= dmg;

        if (currentHealth <= 0) HandleDeath();
    }

    protected void HandleDeath()
    {
        if (isDead) return;

        GameState.Instance.AddCrystals(crystalReward);

        isDead = true;
        EnemySpawner.Instance.RemoveEnemy(this);

        if (animator == null)
        {
            Destroy(gameObject);
            return;
        }

        animator.SetBool("isRunning", false);
        animator.SetTrigger("Die");
        StartCoroutine(DestroyAfterDeathAnimation());
        enabled = false;
    }

    private IEnumerator DestroyAfterDeathAnimation()
    {
        const int animatorLayer = 0;
        const string deathState = "Base Layer.die";

        while (!animator.GetCurrentAnimatorStateInfo(animatorLayer).IsName(deathState))
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(animatorLayer).normalizedTime < 1f)
            yield return null;

        Destroy(gameObject);
    }
}
