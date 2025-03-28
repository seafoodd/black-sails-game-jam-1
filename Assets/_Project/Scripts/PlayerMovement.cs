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
    [SerializeField] private AudioSource aud3;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip[] walkSounds;
    [SerializeField] private AudioClip wheelSound;
    [SerializeField] private AudioClip[] jetpackSounds;
    [SerializeField] private AudioClip jetpackSound;

  
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
    [SerializeField] private ParticleSystem wheelParticles;
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private float maxJetpackFuel;
    [SerializeField] private float jetpackFuel;
    [SerializeField] private float jetpackForce;
    [SerializeField] private bool jetpackEnabled;
    [SerializeField] private float maxSpeed;
    private string currentSceneName;
    private bool animationPlaying;
    [SerializeField] private float timeBetweenJetpackSounds;

    void Awake()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();

        rb.gravityScale = 2.5f;
    }

    void Update()
    {
        if(dead || animationPlaying) return;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        dir = new Vector2(xRaw, yRaw);

        Walk(dir);
        
        if(jetpackEnabled)
        {
            rb.gravityScale = 1.5f;
            Jetpack();
        }

        else
        {
            rb.gravityScale = 2.5f;
        }

        anim.SetHorizontalMovement(Mathf.Abs(rb.velocity.x), y, rb.velocity.y);

        if(Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }

        if(walking && !startedWalking/* && !aud2.isPlaying*/)
        {
            startedWalking = true;
            aud2.volume = 0f;
            aud2.DOKill();
            aud2.DOFade(.05f, .5f);
            //aud2.Stop();
            Debug.Log($"play walking sound");
            InvokeRepeating("PlayWalkingSound", 0f, timeBetweenSteps);
            wheelParticles.enableEmission = true;
        }

        /*if (!coll.onWall || !canMove || Mathf.Abs(x) < .3f)
        {
            wallSlide = false;
        }*/

        if(coll.onGround/* && !dashing*/)
        {
            //wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;

            if(xRaw != 0 || rb.velocity.x > .2f) walking = true;
            else
            {
                walking = false;
                startedWalking = false;
                CancelInvoke("PlayWalkingSound");
                aud2.DOKill();
                aud2.DOFade(0f, .25f);
                wheelParticles.enableEmission = false;
            }
        }
        else
        {
            walking = false;
            startedWalking = false;
            aud2.DOKill();
            aud2.DOFade(0f, .25f);
            CancelInvoke("PlayWalkingSound");
            wheelParticles.enableEmission = false;
        }

        //if(!isDashing) rb.gravityScale = 3; // TODO: maybe remove this


        /*if (Input.GetKeyDown(KeyCode.LeftShift) && !hasDashed)
        {
            if(xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }*/
        
        /*if(coll.onWall && !coll.onGround && !jumping)
        {
            if (xRaw != 0)
            {
                wallSlide = true;
                WallSlide();
            }
        }*/
        
        /*if (!coll.onWall || coll.onGround)
            wallSlide = false;*/
        
        if (Input.GetButtonDown("Jump") && coll.onGround && (jumpBufferAvailable || jumpCount > 0))
        {
            //if (coll.onWall && wallSlide) WallJump();
            /*else */
            anim.SetTrigger("jump");
            Jump(Vector2.up, jumpBufferAvailable);
        }
        if(Input.GetButtonDown("Jump") && !coll.onGround && jetpackFuel > 0)
        {
            aud3.volume = 0f;
            aud3.Stop();
            aud3.DOKill();
            aud3.DOFade(0.08f, .5f);
            //InvokeRepeating("PlayJetpackSound", 0f, timeBetweenJetpackSounds);
            aud3.PlayOneShot(jetpackSound);
            anim.SetBool("jetpack", true);
            jetpackEnabled = true;
        }

        if((!Input.GetButton("Jump") || jetpackFuel <= 0 || coll.onGround) && !walking)
        {
            //CancelInvoke("PlayJetpackSound");
            //aud3.DOKill();
            aud3.DOFade(0f, .5f);
            anim.SetBool("jetpack", false);
            jetpackEnabled = false;
        }
        
        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            jetpackFuel = maxJetpackFuel;
            jumpBufferAvailable = true;
            groundTouch = true;
        }

        if (!coll.onGround && groundTouch)
        {
            jumpCount--;
            Invoke("ResetJumpBuffer", jumpBuffer);
            groundTouch = false;
        }

        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);

        if (!canMove/* || wallSlide*/) return;

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
    private void Jetpack()
    {
        float y = rb.velocity.y;
        //Debug.Log($"y velocity: {y}");
        jetpackFuel -= Time.deltaTime;
        if(y < 10)
        {
            rb.velocity += new Vector2(0f, jetpackForce * Time.deltaTime);
        }
    }

    private void PlayWalkingSound()
    {
        //aud2.DOFade(.06f, .5f);
        Debug.Log($"aud2 volume: {aud2.volume}");
        aud2.pitch = 1.4f;
        //int selectedSound = Random.Range(0, walkSounds.Length);
        //int selectedWalkSound = 0;
        //if(rb.velocity.magnitude < 1f) return;
        //aud2.PlayOneShot(walkSounds[selectedSound]);
        //if(aud2.isPlaying) return;
        aud2.PlayOneShot(wheelSound);
        //Debug.Log($"walking sound, {rb.velocity.x}");
    }

    /*private void PlayJetpackSound()
    {
        aud2.volume = Mathf.Lerp(aud2.volume, .07f, .4f);
        //aud2.volume = 0.07f;
        aud2.pitch = 1f;
        int selectedSound = Random.Range(0, jetpackSounds.Length);
        //int selectedWalkSound = 0;
        aud2.PlayOneShot(jetpackSounds[selectedSound]);
        //Debug.Log($"walking sound, {rb.velocity.x}");
    }*/

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
        //hasDashed = false;
        //dashing = false;

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
    
    /*private void WallJump()
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
    }*/

    /*private void WallSlide()
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
    }*/
    private void Walk(Vector2 dir)
    {
        if (!canMove) return;

        Vector2 targetVelocity = new Vector2(dir.x * speed, rb.velocity.y);
        
        //Debug.Log("bebebe");
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, movementSmoothing * Time.deltaTime * (coll.onGround ? 1 : airMultiplier));
    }

    private void Jump(Vector2 dir,/* bool wall,*/ bool jumpingFromSurface = false)
    {
        //ParticleSystem particle = jumpParticle;
        if (jumpCount <= 0/* && !wall*/ && !jumpBufferAvailable) return;
        jumpBufferAvailable = false;
        jumping = true;
        anim.SetBool("jumping", true);
        if(!jumpingFromSurface) jumpCount--;
        Invoke("ResetJump", .5f);
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
        var emitParams = new ParticleSystem.EmitParams();
        emitParams.startColor = Color.grey;
        emitParams.startSize = .1f;
        //jumpParticles.Emit(emitParams, 50);
        jumpParticles.Play();
        Invoke("StopJumpParticles", .5f);

        aud.Stop();
        /*if(wall)
        {
            aud.pitch = 1.1f;
            aud.volume = 0.7f;
        }
        else
        {

        }*/
        aud.pitch = 0.9f;
        aud.volume = 0.6f;
        aud.PlayOneShot(jumpSound);

        //particle.Play();
    }

    private void StopJumpParticles()
    {
        jumpParticles.Stop();
    }

    private void ResetJump()
    {
        jumping = false;
        anim.SetBool("jumping", false);
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
    public void OnAnimationPlaying(float time = 1)
    {
        if(animationPlaying)
            return;
        animationPlaying = true;
        //DOVirtual.Float(rb.rotation, rb.rotation - side * 90, .5f, PlayerRotation);
        float x = 0;
        float y = 0;
        float xRaw = 0;
        float yRaw = 0;
        dir = Vector2.zero;
        rb.velocity = Vector2.zero;
        Invoke("StopAnimation", time);
    }

    private void StopAnimation()
    {
        animationPlaying = false;
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
        SceneManager.LoadScene(currentSceneName);
    }

    void PlayerRotation(float x)
    {
        rb.rotation = x;
    }
}
