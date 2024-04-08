// Source : https://unity.com/how-to/use-object-pooling-boost-performance-c-scripts-unity
using UnityEngine.Pool;
using UnityEngine;
using System.Collections;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_projectilePrefab;
    [SerializeField] private GameObject m_gunBarrel;
    [SerializeField] private float m_timeToReturnToPool = 10.0f;
    [SerializeField] private float m_projectileSpeed = 5.0f;
    [SerializeField] private float m_fireRate = 1.0f;
    [SerializeField] private int m_maxPoolSize = 100;
    [SerializeField] private bool m_isActive = false;

    private ObjectPool<GameObject> m_projectilePool;

    private void Awake()
    {
        m_projectilePool = new ObjectPool<GameObject>(
            CreateProjectile,
            ActivateProjectile,
            DeactivateProjectile,
            null,
            true,
            m_maxPoolSize
        );
    }

    private GameObject CreateProjectile()
    {
        GameObject projectile = Instantiate(m_projectilePrefab);
        projectile.SetActive(false);
        return projectile;
    }

    private void ActivateProjectile(GameObject projectile)
    {
        projectile.SetActive(true);
    }

    private void DeactivateProjectile(GameObject projectile)
    {
        projectile.SetActive(false);
    }

    private void Start()
    {
        // Source : https://sd.blackball.lv/library/Unity_Games_by_Tutorials_3rd_Edition_(2018).pdf page 94
        // Source : https://youtu.be/j-28BbzvgGk
        // Repeat invoke makes the turret shoot automatically multiple bullets
        InvokeRepeating(nameof(ShootProjectile), 0f, m_fireRate);
    }

    private void ShootProjectile()
    {
        if (!m_isActive)
        {
            return;
        }

        GameObject projectile = m_projectilePool.Get();
        if (projectile != null)
        {
            // Set the projectile's position and rotation
            projectile.transform.position = m_gunBarrel.transform.position;
            projectile.transform.rotation = m_gunBarrel.transform.rotation;

            // Get the Rigidbody component of the projectile
            Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();

            // Set the velocity of the projectile in the forward direction of the gun barrel
            projectile.transform.position = m_gunBarrel.transform.position;
            projectile.transform.rotation = m_gunBarrel.transform.rotation;
            projectileRigidbody.velocity = -m_gunBarrel.transform.up * m_projectileSpeed;

            // Schedule the projectile to return to the pool after the specified delay
            StartCoroutine(ReturnToPoolAfterDelay(projectile, m_timeToReturnToPool));
        }
    }

    public GameObject GetProjectile()
    {
        GameObject projectile = m_projectilePool.Get();

        // Shoot projectile from gun barrel in its forward direction
        Vector3 forwardDirection = m_gunBarrel.transform.forward;
        projectile.transform.position = m_gunBarrel.transform.position;
        projectile.transform.rotation = Quaternion.LookRotation(forwardDirection);

        return projectile;
    }

    private IEnumerator ReturnToPoolAfterDelay(GameObject projectile, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(projectile);
    }

    private void ReturnToPool(GameObject projectile)
    {
        //Debug.Log("Return projectile to pool");
        m_projectilePool.Release(projectile);
    }
}