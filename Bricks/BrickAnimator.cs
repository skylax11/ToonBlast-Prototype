using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickAnimator : MonoBehaviour
{
    private Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetAnimation(string name , bool situation) => _animator.SetBool(name, situation);

    public void SingleBrick()
    {
        SetAnimation("SingleBrick", true);
    }
    public void DisableBoolean(string name)
    {
        SetAnimation(name, false);
    }

}
