using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class AnimationStateManager : MonoBehaviour
{
    [SerializeField,ReadOnly] AnimationState _animationState = AnimationState.Stopped;

    private List<GenericInteractable> interactablesWithAnimation = new List<GenericInteractable>();
    private List<SplineAnimate> splineAnimates = new List<SplineAnimate>();


    public void StartAnimation()
    {
        SetAnimationState(AnimationState.Playing);
    }

    public void StopAnimation()
    {
        SetAnimationState(AnimationState.Stopped);
    }

    public void PauseAnimation(){
        SetAnimationState(AnimationState.Paused);
    }
    private void SetAnimationState(AnimationState state)
    {
        switch (_animationState)
        {
            case AnimationState.Stopped:
                switch (state)
                {
                    case AnimationState.Stopped:
                        break;
                    case AnimationState.Playing:
                        PlayAnimations();
                        break;
                    case AnimationState.Paused:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
                break;
            case AnimationState.Playing:
                switch (state)
                {
                    case AnimationState.Stopped:
                        StopAnimations();
                        break;
                    case AnimationState.Playing:
                        break;
                    case AnimationState.Paused:
                        PauseAnimations();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
                break;
            case AnimationState.Paused:
                switch (state)
                {
                    case AnimationState.Stopped:
                        StopAnimations();
                        break;
                    case AnimationState.Playing:
                        ResumeAnimations();
                        break;
                    case AnimationState.Paused:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _animationState = state;
    }

    private void ResumeAnimations()
    {
        splineAnimates = FindObjectsOfType<SplineAnimate>().ToList();
        foreach (var splineAnimate in splineAnimates)
        {
            if (splineAnimate == null) { continue; }
            if (splineAnimate.enabled && splineAnimate.Container != null)
            {
                splineAnimate.Loop = splineAnimate.Container.Spline.Closed ? SplineAnimate.LoopMode.Loop : SplineAnimate.LoopMode.Once;
                splineAnimate.Play();
            }
        }
    }

    private void StopAnimations()
    {
        splineAnimates = FindObjectsOfType<SplineAnimate>().ToList();

        foreach (var splineAnimate in splineAnimates)
        {
            if (splineAnimate == null) { continue; }

            if (splineAnimate.enabled && splineAnimate.Container != null)
            {
                splineAnimate.Restart(false);
            }
        }
    }

    private void PauseAnimations()
    {
        splineAnimates = FindObjectsOfType<SplineAnimate>().ToList();

        foreach (var splineAnimate in splineAnimates)
        {
            if (splineAnimate == null) { continue; }

            if (splineAnimate.enabled && splineAnimate.Container != null)
            {
                splineAnimate.Pause();
            }
        }
        //interactablesWithAnimation.ForEach(e => e.PauseAnimation());
    }

    private void PlayAnimations()
    {
        splineAnimates = FindObjectsOfType<SplineAnimate>().ToList();
        
        foreach (var splineAnimate in splineAnimates)
        {
            if (splineAnimate == null) { continue; }

            if (splineAnimate.enabled && splineAnimate.Container != null)
            {
                splineAnimate.Loop = splineAnimate.Container.Spline.Closed ? SplineAnimate.LoopMode.Loop : SplineAnimate.LoopMode.Once;
                splineAnimate.Play();
            }
        }
        
        //interactablesWithAnimation = FindObjectsOfType<GenericInteractable>().ToList();
        //interactablesWithAnimation.ForEach(e => e.PlayAnimation());
    }
}
