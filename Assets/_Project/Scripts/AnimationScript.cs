using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{

    private Animator anim;
    private PlayerMovement pm;
    private Collision coll;
    [HideInInspector]
    public SpriteRenderer sr;

    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponentInParent<Collision>();
        pm = GetComponentInParent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        anim.SetBool("onGround", coll.onGround);
        //anim.SetBool("onWall", coll.onWall);
        //anim.SetBool("onRightWall", coll.onRightWall);
        //anim.SetBool("wallSlide", pm.wallSlide);
        anim.SetBool("canMove", pm.canMove);
        //anim.SetBool("isDashing", pm.dashing);
    }

    public void SetHorizontalMovement(float x,float y, float yVel)
    {
        anim.SetFloat("HorizontalAxis", x);
        anim.SetFloat("VerticalAxis", y);
        anim.SetFloat("VerticalVelocity", yVel);
    }

    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

    public void SetBool(string name, bool value)
    {
        anim.SetBool(name, value);
    }

    public void Flip(int side)
    {
        if (pm.wallSlide)
        {
            if (side == -1 && sr.flipX) return;

            if (side == 1 && !sr.flipX) return;

        }

        bool state = (side == 1) ? false : true;
        sr.flipX = state;
    }
}
