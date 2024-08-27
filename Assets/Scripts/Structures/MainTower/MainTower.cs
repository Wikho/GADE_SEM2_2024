using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTower : MonoBehaviour
{
    #region Variables

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDestroyed = false;

    [Header("Attack Settings")]
    public float attackRange = 10f;
    public float attackPower = 20f;
    public float attackCooldown = 1f;
    private Transform target;

    #endregion

    #region Unity Methods
    void Start()
    {
        currentHealth = maxHealth;
    }
    #endregion

    #region Functions



    #endregion

    #region GetSet

    #endregion

}
