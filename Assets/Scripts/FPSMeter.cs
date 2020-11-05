using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSMeter : MonoBehaviour
{
    Text    text;
    float   accumulator;
    int     frameCount;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        accumulator = 0;
        frameCount = 0;
    }

    void Update()
    {
        if (frameCount > 10)
        {
            frameCount = 0;
            accumulator = 0;
        }

        accumulator = ((accumulator * frameCount) + Time.deltaTime) / (frameCount + 1);

        frameCount++;

        text.text = "FPS: " + (1.0f / accumulator);
    }
}
