using UnityEngine.Pool;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    //[SerializeField] private GameObject projectilePrefab;
    //[SerializeField] private float timeToReturnToPool = 10f;
    //[SerializeField] private int maxPoolSize = 100;

    //private ObjectPool<GameObject> objectPool;

    //void Awake() => objectPool = new ObjectPool<GameObject>(CreateProjectile, OnGetProjectile, OnReleaseProjectile, null, true, maxPoolSize);

    //private void Awake()
    //{
    //    objectPool = new ObjectPool<GameObject>(
    //        createFunc: () =>
    //        {
    //            GameObject projectile = Instantiate(projectilePrefab);
    //            projectile.SetActive(false);
    //            return projectile;
    //        },
    //        actionOnGet: (projectile) => projectile.SetActive(true),
    //        actionOnRelease: (projectile) => projectile.SetActive(false),
    //        actionOnDestroy: null,
    //        collectionCheck: true,
    //        defaultCapacity: maxPoolSize
    //    );
    //}

    //private void Update()
    //{
    //    GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
    //    projectile.transform.position = transform.position;
    //    projectile.GetComponent<Rigidbody>().AddForce(transform.forward * 1000f);
    //}

    //public GameObject GetProjectile()
    //{
    //    GameObject projectile = objectPool.Get();
    //    Invoke(nameof(ReturnToPool), timeToReturnToPool);
    //    return projectile;
    //}

    //private void ReturnToPool(GameObject projectile)
    //{
    //    objectPool.Release(projectile);
    //}
}
