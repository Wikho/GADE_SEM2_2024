using System.Collections.Generic;
using UnityEngine;

public class TowerTurret : MonoBehaviour
{
    #region Variables

    [Header("Settings")]
    [SerializeField] private float damage = 1.0f;
    [SerializeField] private float fireRate = 1.0f;
    [SerializeField] private float range = 10.0f;

    [Space]
    [Header("Requre Componets")]
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Space]
    [Header("Enemy List")]
    [SerializeField] private List<GameObject> enemiesInRange = new List<GameObject>();

    //private
    private GameObject currentTarget;
    private float fireCooldown = 0f;
    private Quaternion originalRotation;

    //Components added when tower is spawned
    private SphereCollider col;

    #endregion

    #region Unity Methods
    private void Awake()
    {
        AddTowerComponet();
        originalRotation = pivotPoint.localRotation;
    }

    private void FixedUpdate()
    {
        HandleRotation();
        HandleTargetingAndShooting();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    #endregion

    #region Functions
    private void HandleRotation()
    {
            if (currentTarget != null)
            {
                Vector3 direction = currentTarget.transform.position - pivotPoint.position;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                pivotPoint.rotation = Quaternion.Slerp(pivotPoint.rotation, lookRotation, Time.deltaTime * fireRate);
            }
            else
            {
                // Return to the original rotation when no enemies are present
                pivotPoint.localRotation = Quaternion.Slerp(pivotPoint.localRotation, originalRotation, Time.deltaTime * fireRate);
            }
    }

    private void HandleTargetingAndShooting()
    {
        //Decrease the cooldown timer
        fireCooldown -= Time.deltaTime;

        //Ensure that the list contains only valid enemies
        enemiesInRange.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);

        //If there is no current target or the current target is invalid, select the next available target
        if (currentTarget == null || !enemiesInRange.Contains(currentTarget))
        {
            currentTarget = enemiesInRange.Count > 0 ? enemiesInRange[0] : null;
        }

        //If it have a valid target, proceed shooting
        if (currentTarget != null)
        {
            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = 1f / fireRate;
            }
        }
    }

    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null && currentTarget != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            //Give Bullet target position
            bullet.GetComponent<TowerBullet>().target = currentTarget.transform;
            bullet.GetComponent<TowerBullet>().twr = this.gameObject.GetComponent<TowerTurret>();
        }
    }

    private void AddTowerComponet()
    {
        //Collider/Trigger
        col = this.gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;
        UpdateRange();
    }

    public void UpdateRange()
    {
        col.radius = range;
    }


    #endregion

    #region GetSet

    public float GetDamage()
    {
        return damage;
    }

    #endregion

    #region Gizmo

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        if (firePoint != null && currentTarget != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(firePoint.position, currentTarget.transform.position);
        }
    }

    #endregion

}
