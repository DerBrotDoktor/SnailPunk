using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramesPerSecondLimit : MonoBehaviour
{
    [SerializeField] private int maxFramesPerSecond = 60;
    private void Awake()
    {
        Application.targetFrameRate = maxFramesPerSecond;
    }
}
