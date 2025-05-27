using UnityEngine;
public interface IRecoilData
{
    float RecoilX { get; }
    float RecoilY { get; }
    float RecoilZ { get; }

    float AimRecoilX { get; }
    float AimRecoilY { get; }
    float AimRecoilZ { get; }

    float Snappiness { get; }
    float ReturnSpeed { get; }
}