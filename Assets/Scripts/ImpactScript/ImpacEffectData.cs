using UnityEngine;

[CreateAssetMenu(fileName = "ImpactEffectData", menuName = "Scriptable Objects/ImpactEffectData")]
public class ImpactEffectData : ScriptableObject
{
    public ImpactType type;
    public ParticleSystem effectPrefab;
    public int poolSize;
}
