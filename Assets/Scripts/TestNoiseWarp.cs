using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNoiseWarp : MonoBehaviour {

    public int seed = 0;
    public float scale;
    public int width = 100;
    public int height = 100;

    public float distortionOneScale = 4f;
    public float distortionTwoScale = 4f;

    public Vector2 distortionOneOffsetOne;
    public Vector2 distortionOneOffsetTwo;
    public Vector2 distortionTwoOffsetOne;
    public Vector2 distortionTwoOffsetTwo;

    [Range(0, 2)]
    public int numDistortions;
    private Vector2[] octaveOffsets = new Vector2[8];
    System.Random rand;

    public void Run() {
        rand = new System.Random(seed);
        float[,] values = new float[width, height];

        for(int i = 0; i < 8; i++) {
            octaveOffsets[i].x = (float)(rand.NextDouble() * 20000f - 10000f);
            octaveOffsets[i].y = (float)(rand.NextDouble() * 20000f - 10000f);
        }

        float min = float.MaxValue;
        float max = float.MinValue;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if(numDistortions == 0){
                    values[x, y] = Fbm(new Vector2(x, y));
                }
                if (numDistortions == 1) {
                    values[x, y] = pattern1(new Vector2(x, y));
                }
                if (numDistortions == 2) {
                    values[x, y] = pattern2(new Vector2(x, y));
                }
            }
        }

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if(values[x, y] < min) {
                    min = values[x, y];
                }
                if (values[x, y] > max) {
                    max = values[x, y];
                }
            }
        }

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                values[x, y] = Mathf.InverseLerp(min, max, values[x, y]);
            }
        }

        Texture2D tex = new Texture2D(width, height);
        Color[] colorMap = new Color[width * height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, values[x, y]);
            }
        }
        tex.SetPixels(colorMap);
        tex.Apply();

        gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = tex;
    }

    private float Fbm(Vector2 p) {
        float frequency = 1;
        float amplitude = 1;
        float val = 0;
        
        for (int i = 0; i < 8; i++) {
            float sampleX = (p.x / scale) * frequency + octaveOffsets[i].x;
            float sampleY = (p.y / scale) * frequency + octaveOffsets[i].y;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;

            val += perlinValue * amplitude;

            frequency *= 2;
            amplitude *= 0.5f;
        }

        return val;
    }

    private float pattern1(Vector2 p) {
        Vector2 q = new Vector2(Fbm(p + distortionOneOffsetOne), Fbm(p + distortionOneOffsetTwo));

        return Fbm(p + distortionOneScale * q);
    }

    private float pattern2(Vector2 p) {
        Vector2 q = new Vector2(Fbm(p + distortionOneOffsetOne), Fbm(p + distortionOneOffsetTwo));
        Vector2 r = new Vector2(Fbm(p + distortionOneScale * q + distortionTwoOffsetOne), Fbm(p + distortionOneScale * q + distortionTwoOffsetTwo));

        return Fbm(p + distortionTwoScale * r);
    }
}
