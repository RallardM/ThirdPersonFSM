using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_backgroundHits;

    [SerializeField]
    private List<GameObject> m_3DTextHits;

    [SerializeField]
    private GameObject m_explosionPrefab;

    [SerializeField]
    private Transform m_explosionPosition;

    [SerializeField]
    private CameraShake m_cameraShake;

    [SerializeField]
    private float m_explosionStrenght = 10.0f;

    private static VFXManager s_instance;

    public static VFXManager GetInstance()
    {
        if (s_instance == null)
        {
            return new VFXManager();
        }

        return s_instance;
    }

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            DontDestroyOnLoad(this);
        }
        else if (s_instance != this)
        {
            Debug.LogWarning("VFXManager : Awake() - Attempted to create a second instance of the GameManagerSM singleton!");
            Destroy(this);
        }
    }

    public void InstantiateVFX(EVFX_Type vfxType, Vector3 pos, float cameraShakePower)
    {
        int randomIndex = 0;
        switch (vfxType)
        {
            case EVFX_Type.Hit:
                randomIndex = Random.Range(0, m_3DTextHits.Count);
                Instantiate(m_backgroundHits[randomIndex], pos, Quaternion.identity, transform);
                Instantiate(m_3DTextHits[randomIndex], pos, Quaternion.identity, transform);
                m_cameraShake.ShakeCamera(cameraShakePower);
                break;

            case EVFX_Type.Explosion:
                Instantiate(m_explosionPrefab, pos, Quaternion.identity, transform);
                m_cameraShake.ShakeCamera(cameraShakePower);
                break;

            default:
                Debug.LogError("VFXManager : InstantiateVFX() - Unknown VFX type!");
                break;
        }
    }

    public void SetExplosion()
    {
        InstantiateVFX(EVFX_Type.Explosion, m_explosionPosition.position, m_explosionStrenght);
    }
}

public enum EVFX_Type
{
    Hit,
    Explosion,
    count
}