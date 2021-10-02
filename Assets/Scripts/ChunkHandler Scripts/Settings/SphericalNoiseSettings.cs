using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SphericalNoiseSettings : NoiseSettings {
    [Min(0)]
    public float amplitude = 1;
}
