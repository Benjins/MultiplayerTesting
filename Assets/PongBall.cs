using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PongBall : NetworkBehaviour
{

	public float speed = 1.0f;

	public float sign = 1.0f;

	// For now, the ball mostly travels back and forth on the X axis,
	// not including potential up/down dips
	float theirX = Mathf.Infinity;
	float ourX = Mathf.NegativeInfinity;

	void Start()
	{
		var players = FindObjectsOfType<PlayerControl>();
		
		if (players[0].isLocalPlayer)
		{
			ourX = players[0].transform.position.x;
			theirX = players[1].transform.position.x;
		}
		else
		{
			theirX = players[0].transform.position.x;
			ourX = players[1].transform.position.x;
		}

	}

	void Update () {
		// The server spawns the pongball,
		// so it's the one moving it
		if (isServer)
		{
			float minX = Mathf.Min(ourX, theirX);
			float maxX = Mathf.Max(ourX, theirX);
			transform.position += Vector3.right * Time.deltaTime * speed * sign;

			if (transform.position.x > maxX)
			{
				sign = -1.0f;
			}
			else if (transform.position.x < minX)
			{
				sign = 1.0f;
			}
		}
	}
}
