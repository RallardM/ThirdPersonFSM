using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class VFXManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_hitPSBackground;
    [SerializeField]
    private GameObject m_hitPS3DText;

    private static VFXManager s_instance;

    public static VFXManager GetInstance()
    {
        if (s_instance == null)
        {
            return new VFXManager();
        }

        return s_instance;
    }

    public VFXManager()
    {
        s_instance = this;
    }

    public void InstantiateVFX(EVFX_Type vfxType, Vector3 pos)
    {
        switch (vfxType)
        {
            case EVFX_Type.Hit:
                Instantiate(m_hitPSBackground, pos, Quaternion.identity, transform);
                Instantiate(m_hitPS3DText, pos, Quaternion.identity, transform);
                break;

            default:
                break;
        }
    }
}

public enum EVFX_Type
{
    Hit,
    count
}