using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    #region Variables
    [SerializeField] private float maxHealth = 10;
    [SerializeField] float currentHealth;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float damage = 100;

    [SerializeField] private Transform TestLocation;
    private Transform target;
    private NavMeshAgent navMeshAgent;
    #endregion

    #region Unity Methods

    private void Start()
    {
        currentHealth = maxHealth;
        SetNavMesh();
    }

    private void Update()
    {
        CheckHealth();
    }

    #endregion

    #region functions

    private void SetNavMesh()
    {
        try
        {
            target = TerrainGenerator.Instance.GetMainTower().transform;
        }
        catch
        {
            target = TestLocation;
        }
   

        navMeshAgent = GetComponent<NavMeshAgent>();

        //Set the speed of the NavMeshAgent
        navMeshAgent.speed = speed;

        //Start navigating to the target
        if (target != null)
        {
            navMeshAgent.SetDestination(target.position);
        }
    }

    private void CheckHealth()
    {
        if(currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }



    #endregion

    #region GetSet

    //Multiplier
    public void healthMultiplier(float mul)
    {
        maxHealth *= mul;
    }
    public void speedMultiplier(float mul)
    {
        speed *= mul;
    }
    public void damageMultiplier(float mul)
    {
        damage *= mul;
    }

    public void Damage(float dmg)
    {
        currentHealth -= dmg;
    }

    #endregion
}
