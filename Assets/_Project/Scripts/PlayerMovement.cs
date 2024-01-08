using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    [SerializeField] private AnimationScript anim;

    [Space]
    [Header("Stats")]
    [SerializeField] private float speed = 10;
    [SerializeField] private float jumpForce = 50;
    [SerializeField] private float slideSpeed = 5;
    [SerializeField] private float wallJumpLerp = 10;
    [SerializeField] private float dashSpeed = 20;
    [SerializeField] private float movementSmoothing = 7.5f;
    [SerializeField] private float jumpBuffer = 0.15f;
    [SerializeField] private int maxJumpCount;
    [SerializeField] private int jumpCount;
    [SerializeField] private float wallJumpForce = 1.5f;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float wallJumpVerticalMultiplier;
    [SerializeField] private float timeBetweenSteps;

    [Space]
    [Header("Audio Effects")]
    [SerializeField] private AudioSource aud;
    [SerializeField] private AudioSource aud2;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip[] walkSounds;

  
    [Space]
    [Header("Booleans")]
    [SerializeField] private bool jumpBufferAvailable;
    public bool canMove;
    [SerializeField] private bool walking;
    [SerializeField] private bool wallJumped;
    public bool wallSlide;
    public bool dashing;
    //public bool wallGrab;

    [Space]

    private bool groundTouch;
    private bool hasDashed;

    public int side = 1;

    [Space]
    [Header("Polish")]
    //public ParticleSystem dashParticle;
    //public ParticleSystem jumpParticle;
    //public ParticleSystem wallJumpParticle;
    //public ParticleSystem slideParticle;
    private Vector2 dir;
    private bool jumping;
    private bool startedWalking;
    [SerializeField] private GameObject deathExplosion;
    [SerializeField] private GameObject visual;
    private bool dead;

    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();

        rb.gravityScale = 3f;
    }
    
    void Update()
    {
        if(dead) return;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        dir = new Vector2(xRaw, yRaw);

        Walk(dir);
        anim.SetHorizontalMovement(Mathf.Abs(rb.velocity.x), y, rb.velocity.y);

        if(Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }

        if(walking && !startedWalking && !aud.isPlaying)
        {
            startedWalking = true;
            InvokeRepeating("PlayWalkingSound", 0f, timeBetweenSteps);
        }

        if (!coll.onWall || !canMove || Mathf.Abs(x) < .3f)
        {
            wallSlide = false;
        }

        if(coll.onGround && !dashing)
        {
            wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;

            if(xRaw != 0) walking = true;
            else
            {
                walking = false;
                startedWalking = false;
                CancelInvoke("PlayWalkingSound");
            }
        }
        else
        {
            walking = false;
            startedWalking = false;
            CancelInvoke("PlayWalkingSound");
        }

        //if(!isDashing) rb.gravityScale = 3; // TODO: maybe remove this


        if (Input.GetKeyDown(KeyCode.LeftShift) && !hasDashed)
        {
            if(xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }
        
        if(coll.onWall && !coll.onGround && !jumping)
        {
            if (xRaw != 0)
            {
                wallSlide = true;
                WallSlide();
            }
        }
        
        if (!coll.onWall || coll.onGround)
            wallSlide = false;
        
        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("jump");
            if (coll.onWall && wallSlide) WallJump();
            else if (jumpBufferAvailable || jumpCount > 0)
            {
                Jump(Vector2.up, false, jumpBufferAvailable);
            }
        }
        
        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            jumpBufferAvailable = true;
            groundTouch = true;
        }

        if (!coll.onGround && groundTouch)
        {
            jumpCount--;
            Invoke("ResetJumpBuffer", jumpBuffer);
            groundTouch = false;
        }

        if (!canMove || wallSlide) return;

        if(xRaw > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (xRaw < 0)
        {
            side = -1;
            anim.Flip(side);
        }


    }

    private void PlayWalkingSound()
    {
        aud2.volume = 0.05f;
        aud2.pitch = 1.4f;
        int selectedWalkSound = Random.Range(0, 9);
        //int selectedWalkSound = 0;
        if(rb.velocity.magnitude < 1f) return;
        aud2.PlayOneShot(walkSounds[selectedWalkSound]);
        Debug.Log($"walking sound, {rb.velocity.x}");
    }

    private void ResetJumpBuffer()
    {
        if(!coll.onGround) jumpBufferAvailable = false;
    }

    /*private void FixedUpdate()
    {
        Walk(dir);
    }*/

    void GroundTouch()
    {
        jumpCount = maxJumpCount;
        hasDashed = false;
        dashing = false;

        side = anim.sr.flipX ? -1 : 1;

        //jumpParticle.Play();
    }

    private void Dash(float x, float y)
    {
        Camera.main.transform.DOComplete();
        //Camera.main.transform.DOShakePosition(.2f, .15f, 14, 90, false, true);
        //FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

        hasDashed = true;

        anim.SetTrigger("dash");

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);
        //rb.velocity += dir.normalized * dashSpeed;
        rb.AddForce(dir.normalized * dashSpeed, ForceMode2D.Impulse);
        aud.volume = 0.5f;
        aud.pitch = 1.0f;
        aud.PlayOneShot(dashSound);
        //Debug.Log($"{dir.normalized * dashSpeed}, {rb.velocity.magnitude}");
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        //FindObjectOfType<GhostTrail>().ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

        //dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<BetterJumping>().enabled = false;
        wallJumped = true;
        dashing = true;

        yield return new WaitForSeconds(.15f);

        //dashParticle.Stop();
        rb.gravityScale = 3;
        GetComponent<BetterJumping>().enabled = true;
        wallJumped = false;
        dashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(3.15f);
        if (coll.onGround) hasDashed = false;
    }
    
    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            anim.Flip(side);
        }

        //StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;
        wallDir = Vector2.ClampMagnitude(wallDir + Vector2.up * wallJumpVerticalMultiplier, 1f);
        
        if (maxJumpCount > 1) jumpCount = 1;
        Jump(wallDir * wallJumpForce, true, true);
        wallJumped = true;
    }

    private void WallSlide()
    {
        if(coll.wallSide != side)
            anim.Flip(side * -1);

        if (!canMove)
            return;

        bool pushingWall = false;
        if((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        if(!jumping) rb.velocity = new Vector2(push, -slideSpeed);
    }
    private void Walk(Vector2 dir)
    {
        if (!canMove) return;

        Vector2 targetVelocity = new Vector2(dir.x * speed, rb.velocity.y);
        
        //Debug.Log("bebebe");
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, movementSmoothing * Time.deltaTime * (coll.onGround ? 1 : airMultiplier));
    }

    private void Jump(Vector2 dir, bool wall, bool jumpingFromSurface = false)
    {
        //ParticleSystem particle = jumpParticle;
        if (jumpCount <= 0 && !wall && !jumpBufferAvailable) return;
        jumpBufferAvailable = false;
        jumping = true;
        if(!jumpingFromSurface) jumpCount--;
        Invoke("ResetJump", .2f);
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

        aud.Stop();
        if(wall)
        {
            aud.pitch = 1.1f;
            aud.volume = 0.7f;
        }
        else
        {
            aud.pitch = 0.9f;
            aud.volume = 0.6f;

        }
        aud.PlayOneShot(jumpSound);

        //particle.Play();
    }

    private void ResetJump()
    {
        jumping = false;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    public void OnDeath()
    {
        if(dead) return;
        dead = true;
        //DOVirtual.Float(rb.rotation, rb.rotation - side * 90, .5f, PlayerRotation);
        float x = 0;
        float y = 0;
        float xRaw = 0;
        float yRaw = 0;
        dir = Vector2.zero;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        Invoke("Explode", 0);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.layer == 7)
        {
            OnDeath();
        }
    }

    private void Explode()
    {
        Camera.main.transform.DOShakePosition(.5f, .5f, 14);
        deathExplosion.SetActive(true);
        deathExplosion.GetComponent<Animator>().SetTrigger("explode");
        deathExplosion.GetComponent<AudioSource>().Play();
        visual.SetActive(false);

        Invoke("ReloadScene", 1.5f);
    }

    private void ReloadScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    void PlayerRotation(float x)
    {
        rb.rotation = x;
    }
}
