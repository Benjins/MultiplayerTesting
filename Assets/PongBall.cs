using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PongBall : NetworkBehaviour
{

	public float speed = 1.0f;

	public float sign = 1.0f;

	float theirY = Mathf.Infinity;
	float ourY = Mathf.NegativeInfinity;

	void Start()
	{
		var players = FindObjectsOfType<PlayerControl>();
		
		if (players[0].isLocalPlayer)
		{
			ourY = players[0].transform.position.y;
			theirY = players[1].transform.position.y;
		}
		else
		{
			theirY = players[0].transform.position.y;
			ourY = players[1].transform.position.y;
		}

	}

	void Update () {
		// The server spawns the pongball,
		// so it's the one moving it
		if (isServer)
		{
			float minY = Mathf.Min(ourY, theirY);
			float maxY = Mathf.Max(ourY, theirY);
			transform.position += Vector3.up * Time.deltaTime * speed * sign;

			if (transform.position.y > maxY)
			{
				sign = -1.0f;
			}
			else if (transform.position.y < minY)
			{
				sign = 1.0f;
			}
		}
	}
}
