using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorPauser : MonoBehaviour
{
    private Animator animator;
    
    private void OnEnable()
    {
        Game.TimeScaleChanged += OnTimeScaleChanged;

        animator = GetComponent<Animator>();
        OnTimeScaleChanged();
    }

    private void OnDisable()
    {
        Game.TimeScaleChanged -= OnTimeScaleChanged;
    }
    
    private void OnTimeScaleChanged()
    {
        animator.enabled = Game.TimeScale > 0;
    }
}
