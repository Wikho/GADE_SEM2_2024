using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCastDamageTowers : MonoBehaviour
{
    [SerializeField] private float _damage = 10.0f;
    [SerializeField] private float _attackCooldown = 0.1f;

    [SerializeField] List<GameObject> m_towersInRange; // Going to use the GameObject name as a key since there should hopefully not be duplication

    private Coroutine attackCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attackable") && !m_towersInRange.Contains(other.gameObject))
        {
            m_towersInRange.Add(other.transform.parent.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Attackable") && m_towersInRange.Contains(other.gameObject))
        {
            m_towersInRange.Remove(other.transform.parent.gameObject);
        }
    }

    private void Start()
    {
        attackCoroutine = StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        for (int i = 0; i < m_towersInRange.Count; i++)
        {
            if (m_towersInRange[i] == null)
            {
                // The gameObject was destroyed since it died
                m_towersInRange.RemoveAt(i);
                continue;
            }

            HealthComponent healthComponent = m_towersInRange[i].GetComponent<HealthComponent>();
            if (healthComponent != null && !healthComponent.IsDead)
            {
                healthComponent.TakeDamage(_damage);
            }
        }
        yield return new WaitForSeconds(_attackCooldown);
        StartCoroutine(AttackLoop());
    }

    private void OnDestroy()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
    }

    public void DamageMultiplyer(float value)
    {
        _damage *= value;
    }
}
