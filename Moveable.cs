using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour {

    public float xSpeed = 0;
    public float ySpeed = 0;
    public float maxWalkSpeed = 0;
    public float maxRunSpeed = 0;
    private Rigidbody2D rigidBody;
    private RaycastHit2D hit;
    private bool fullWalkSpeed = false;
    private bool fullRunSpeed = false;

    // Use this for initialization
    void Start () {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool atFullWalkSpeed()
    {
        return fullWalkSpeed;
    }

    public bool atFullRunSpeed()
    {
        return fullRunSpeed;
    }

    public void changeDirection()
    {
        xSpeed = -xSpeed;
    }

    public void moveX()
    {
        rigidBody.AddForce(new Vector3(xSpeed, 0), ForceMode2D.Force);
    }

    public void moveY()
    {
        rigidBody.AddForce(new Vector3(0, ySpeed), ForceMode2D.Force);
    }

    public void moveXY()
    {
        rigidBody.AddForce(new Vector3(xSpeed, ySpeed), ForceMode2D.Force);
    }

    public void walk()
    {
        if (xSpeed > 0)
        {
            if (rigidBody.velocity.x < maxWalkSpeed)
            {
                moveX();
                if (rigidBody.velocity.x > maxWalkSpeed)
                {
                    rigidBody.AddForce(new Vector3(maxWalkSpeed - rigidBody.velocity.x, 0), ForceMode2D.Force);
                    fullWalkSpeed = true;
                }
                else
                {
                    fullWalkSpeed = false;
                }
            }
            else
            {
                fullWalkSpeed = true;
            }
        }
        else
        {
            if (rigidBody.velocity.x > -maxWalkSpeed)
            {
                moveX();
                if (rigidBody.velocity.x < -maxWalkSpeed)
                {
                    rigidBody.AddForce(new Vector3(-maxWalkSpeed - rigidBody.velocity.x, 0), ForceMode2D.Force);
                    fullWalkSpeed = true;
                }
                else
                {
                    fullWalkSpeed = false;
                }
            }
            else
            {
                fullWalkSpeed = true;
            }
        }
    }

    public void run()
    {
        if (xSpeed > 0)
        {
            if (rigidBody.velocity.x < maxRunSpeed)
            {
                moveX();
                print(rigidBody.velocity.x + " " + maxRunSpeed);
                if (rigidBody.velocity.x > maxRunSpeed)
                {
                    rigidBody.AddForce(new Vector3(maxRunSpeed - rigidBody.velocity.x, 0), ForceMode2D.Force);
                    fullRunSpeed = true;
                }
                else
                {
                    fullRunSpeed = false;
                }
            }
            else
            {
                fullRunSpeed = true;
            }
        }
        else
        {
            if (rigidBody.velocity.x > -maxRunSpeed)
            {
                moveX();
                if (rigidBody.velocity.x < -maxRunSpeed)
                {
                    rigidBody.AddForce(new Vector3(-maxRunSpeed - rigidBody.velocity.x, 0), ForceMode2D.Force);
                    fullRunSpeed = true;
                }
                else
                {
                    fullRunSpeed = false;
                }
            }
            else
            {
                fullRunSpeed = true;
            }
        }
    }

    public void jump()
    {
        if (rigidBody.velocity.y < ySpeed)
        {
            rigidBody.AddForce(new Vector3(0, ySpeed), ForceMode2D.Impulse);
            if(rigidBody.velocity.y > ySpeed)
            {
                rigidBody.AddForce(new Vector3(0, ySpeed - rigidBody.velocity.y), ForceMode2D.Impulse);
            }
            print("jump " + rigidBody.velocity);
        }
    }

    public void groundJump()
    {
        if(onGround())
        {
            jump();
        }
    }

    public bool onGround()
    {
        hit = Physics2D.Raycast(transform.position, Vector2.down, 0.55f);
        if(hit.collider != null)
        {
            //print(hit.collider.gameObject.name);
            if (hit.collider.gameObject.tag == "wall")
            {
                return true;
            }
        }
        return false;
    }
}
