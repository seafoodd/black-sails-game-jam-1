using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{

	[Header("Layers")]
	public LayerMask groundLayer;

	[Space]

	public bool onGround;
	public bool onWall;
	public bool onRightWall;
	public bool onLeftWall;
	public int wallSide;

	[Space]

	[Header("Collision")]
	public float collisionRadius = 0.25f;
	public Vector2 bottomOffset, rightOffset, leftOffset;
	private Color debugCollisionColor = Color.red;
    
	void Update()
	{  
		onGround = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, new Vector2(.8f, collisionRadius * 2), 0, groundLayer);
		onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer) || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
		onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
		onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
		
		wallSide = onRightWall ? -1 : 1;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = debugCollisionColor;
        
		Gizmos.DrawWireCube((Vector2)transform.position  + bottomOffset, new Vector2(.8f, collisionRadius * 2));
		Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
		Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
	}
}