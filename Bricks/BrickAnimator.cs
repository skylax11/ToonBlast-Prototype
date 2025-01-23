using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickAnimator : MonoBehaviour
{
    private Animator _animator;

    [Header("Animation Infos")]
    [SerializeField] AnimationClip _transformClip;
    public static float TransformClipLength;
    public static float RelocateAnimationDuration = 1f;


    private void Start()
    {
        TransformClipLength = _transformClip.length;
        _animator = GetComponent<Animator>();
    }

    public void SetAnimation(string name , bool situation) => _animator.SetBool(name, situation);

    public void SingleBrick()
    {
        SetAnimation("SingleBrick", true);
    }
    public void HandleTransformAnim()
    {
        SetAnimation("Transform", true);
    }
    public void DisableBoolean(string name)
    {
        SetAnimation(name, false);
    }

}
