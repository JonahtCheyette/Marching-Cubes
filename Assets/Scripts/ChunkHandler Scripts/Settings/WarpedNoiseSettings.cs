using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WarpedNoiseSettings : NoiseSettings {
    public float amplitude = 0;
    public float floorHeight = 0;
    [Min(0)]
    public float floorStrength = 10;

    public WarpSettings[] warpSettings = new WarpSettings[1];

    protected override void OnValidate() {
        if (warpSettings.Length > 3) {
            WarpSettings[] oldSettings = new WarpSettings[3];
            for (int i = 0; i < 3; i++) {
                oldSettings[i] = warpSettings[i];
            }
            warpSettings = new WarpSettings[3];
            for (int i = 0; i < 3; i++) {
                warpSettings[i] = oldSettings[i];
            }
        }

        if (warpSettings.Length == 0) {
            warpSettings = new WarpSettings[1];
        }

        base.OnValidate();
    }
}
