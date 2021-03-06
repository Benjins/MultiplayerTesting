﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PongBall : NetworkBehaviour
{

	public float speed = 1.0f;

	public float sign = 1.0f;

	// players[0] = us, players[1] = them
	PlayerControl[] players;

	// For now, the ball mostly travels back and forth on the X axis,
	// not including potential up/down dips
	float theirX = Mathf.Infinity;
	float ourX = Mathf.NegativeInfinity;

	float minY = 0.0f;
	float maxY = 0.0f;

	// These determine the left/right bounds,
	// left/right being according to the players
	float fieldMinZ = -8.0f;
	float fieldMaxZ = 8.0f;

	float zVelocity = 0.0f;

	Vector3 startPos;

	void Start()
	{
		players = FindObjectsOfType<PlayerControl>();

		startPos = transform.position;

		// If players[0] is not local, swap them
		if (!players[0].isLocalPlayer)
		{
			PlayerControl temp = players[0];
			players[0] = players[1];
			players[1] = temp;
		}

		ourX = players[0].transform.position.x;
		theirX = players[1].transform.position.x;

		// TODO: Ensure that they're at the same height?
		maxY = players[0].transform.position.y;
	}

	void Reset()
	{
		sign = 1.0f;
		zVelocity = 0.0f;
		transform.position = new Vector3((ourX + theirX) / 2, players[0].transform.position.y, 0.0f);
	}

	void Update () {
		// The server spawns the pongball,
		// so it's the one moving it
		if (isServer)
		{
			float minX = Mathf.Min(ourX, theirX);
			float maxX = Mathf.Max(ourX, theirX);
			transform.position += Vector3.right * Time.deltaTime * speed * sign;
			transform.position += Vector3.forward * Time.deltaTime * zVelocity;

			float medX = (minX + maxX) / 2;
			float xDiff = maxX - minX;
			// In range [-1, 1]
			float xCoordNormalised = (transform.position.x - medX) / xDiff * 2;

			Vector3 pos = transform.position;
			pos.y = Mathf.Lerp(minY, maxY, Mathf.Abs(xCoordNormalised));
			transform.position = pos;

			if (transform.position.x > maxX)
			{
				sign = -1.0f;
				HandlePaddle((ourX == maxX) ? players[0] : players[1]);
				
			}
			else if (transform.position.x < minX)
			{
				sign = 1.0f;
				HandlePaddle((ourX == minX) ? players[0] : players[1]);
			}
		}
	}

	void HandlePaddle(PlayerControl player)
	{
		float ballZ = transform.position.z;
		// NOTE: We read from the x of localscale because the player is rotated
		// Basically, we want the width of the paddle
		// Also note that the ball's width is not taken into account here
		float minZ = player.transform.position.z - player.transform.localScale.x / 2;
		float maxZ = player.transform.position.z + player.transform.localScale.x / 2;

		if (ballZ < minZ || ballZ > maxZ)
		{
			Reset();
		}
		else
		{
			// This is in range [0, 1], 0 being all the way on the left side of the paddle
			// and 1 being on the right side
			float normalizedZ = (ballZ - minZ) / (maxZ - minZ);

			// Where we want the ball to end up on the far end
			// based on where it hit the paddle
			float desiredFinalFieldZ = Mathf.Lerp(fieldMinZ, fieldMaxZ, normalizedZ);
			float zDiff = desiredFinalFieldZ - ballZ;
			float xDiff = Mathf.Abs(ourX - theirX);
			float velocityRatio = zDiff / xDiff;

			Debug.Log("velocity ratio: " + velocityRatio);

			zVelocity = velocityRatio * speed;
		}
	}
}
