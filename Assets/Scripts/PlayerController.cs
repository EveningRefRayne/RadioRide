using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float jumpPower = 100;
    public float moveSpeed = 10;
    public float accelTime = 0.1f;
    public bool preJump = false;
    public bool canJump = false;
    public float jumpCD = 0f;
    public bool inWifiRange = false;
    public Rigidbody2D phys;
    public bool canMove = true;
    public Animator anim;


    // Use this for initialization
    void Awake()
    {
        //phys = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (jumpCD > 0) jumpCD--;
        if (inWifiRange)
        {
            phys.velocity = new Vector2(Mathf.Lerp(phys.velocity.x, Input.GetAxis("Horizontal"), accelTime) * moveSpeed,
                Mathf.Lerp(phys.velocity.y, Input.GetAxis("Vertical"), accelTime) * moveSpeed)*Time.deltaTime;
            if (phys.velocity.y >= 0)
            {
                anim.SetInteger("animState", 3);//ascend
            }
            else if (phys.velocity.y < 0)
            {
                anim.SetInteger("animState", 4);//descend
            }
        }


        else
        {
            if (phys.velocity.x < 0)
            {
                if (Input.GetAxis("Horizontal") < 0)
                {
                    phys.AddForce(new Vector2(Input.GetAxis("Horizontal") * moveSpeed, 0), ForceMode2D.Force);
                    if (canJump) anim.SetInteger("animState", 1);//walk
                    else anim.SetInteger("animState", 3);//ascend
                }
                else if (Input.GetAxis("Horizontal") > 0)
                {
                    phys.AddForce(new Vector2(Input.GetAxis("Horizontal") * moveSpeed + phys.velocity.x*phys.mass*-5f, 0), ForceMode2D.Force);
                    if (canJump) anim.SetInteger("animState", 1);//walk
                    else anim.SetInteger("animState", 3);//ascend
                }
                else if (Input.GetAxis("Horizontal")==0)
                {
                    phys.AddForce(new Vector2(phys.velocity.x*phys.mass*-20f,0), ForceMode2D.Force);
                    anim.SetInteger("animState", 0);
                }
            }
            else if (phys.velocity.x > 0)
            {
                if (Input.GetAxis("Horizontal") < 0)
                {
                    phys.AddForce(new Vector2(Input.GetAxis("Horizontal") * moveSpeed + phys.velocity.x * phys.mass * -5f, 0), ForceMode2D.Force);
                    if (canJump) anim.SetInteger("animState", 1);//walk
                    else anim.SetInteger("animState", 3);//ascend
                }
                else if (Input.GetAxis("Horizontal") > 0)
                {
                    phys.AddForce(new Vector2(Input.GetAxis("Horizontal") * moveSpeed, 0), ForceMode2D.Force);
                    if (canJump) anim.SetInteger("animState", 1);//walk
                    else anim.SetInteger("animState", 3);//ascend
                }
                else if (Input.GetAxis("Horizontal") == 0)
                {
                    phys.AddForce(new Vector2(phys.velocity.x * phys.mass * -20f, 0), ForceMode2D.Force);
                    anim.SetInteger("animState", 0);//idle
                }
            }
            else if (phys.velocity.x == 0 && Input.GetAxis("Horizontal")!=0)
            {
                phys.AddForce(new Vector2(Input.GetAxis("Horizontal") * moveSpeed*20, 0), ForceMode2D.Force);
                if (canJump) anim.SetInteger("animState", 1);//walk
                else anim.SetInteger("animState", 3);//ascend
            }
            else if (phys.velocity.x==0 && Input.GetAxis("Horizontal")==0)
            {
                if (canJump) anim.SetInteger("animState", 0);
                else anim.SetInteger("animState", 3);//ascend
            }
            
            phys.velocity = Vector3.ClampMagnitude(phys.velocity, moveSpeed);
            //phys.velocity = new Vector2(Mathf.Lerp(phys.velocity.x, Input.GetAxis("Horizontal"), accelTime) * moveSpeed, phys.velocity.y);
            if (preJump == true)
            {
                preJump = false;
                canJump = false;
                jumpCD = 30;
                phys.AddForce(Vector2.up * jumpPower,ForceMode2D.Impulse);
                anim.SetInteger("animState", 3);//ascend
                //Debug.Log("Should be 3: " + anim.GetInteger("animState"));
                //Do the jump animation
            }
            if (Input.GetAxis("Vertical") > 0)
            {
                if (jumpCD > 0)
                {
                    phys.AddForce(Vector2.up * jumpCD);
                }
                if (canJump && jumpCD==0)
                {
                    if (preJump == false)
                    {
                        preJump = true;
                        //Debug.Log("Jump Pressed, jumping prep");
                        anim.SetInteger("animState", 3);//ascend
                        //Debug.Log("Anim State: " + anim.GetInteger("animState"));
                        //Do the prejump animation
                    }
                }
            }
            else if(Input.GetAxis("Vertical")<=0)
            {
                jumpCD = 0;
            }
        }
        if(phys.velocity.y<0)
        {
            phys.AddForce(new Vector2(0, -18f * phys.mass));
            //Debug.Log("Falling, trigger falling anim");
            anim.SetInteger("animState", 4);//descend
            //Do the falling animation
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer==9)
        {
            bool isAbove = false;
            ContactPoint2D[] points = collision.contacts;
            foreach(ContactPoint2D point in points)
            {
                if(point.point.y<(transform.position.y-GetComponent<SpriteRenderer>().bounds.extents.y*0.9f))
                {
                    //Debug.Log("The point: " + point.point + " Is below: " + (transform.position.y - GetComponent<SpriteRenderer>().bounds.extents.y * 0.9f));
                    isAbove = true;
                    canJump = true;
                    if(Input.GetAxis("Horizontal")!=0)
                    {
                        anim.SetInteger("animState", 1);//walk
                    }
                    phys.velocity = Vector2.zero;
                    anim.SetInteger("animState", 0);
                    //jumpCD = 0;
                }
            }
            //canJump = true;
            //jumpCD = 0;
            //Do the landing animation
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Debug.Log("Collision Exit");
        if (!GetComponent<CapsuleCollider2D>().IsTouchingLayers(512))
        {
            //Debug.Log("Not touching ground, can't jump.");
            canJump = false;
            anim.SetInteger("animState", 4);//descend
        }
    }
}
