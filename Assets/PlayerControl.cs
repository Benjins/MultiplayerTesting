using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour
{

	public float speed = 4.0f;

	// How far forward the ball spawns from the player
	public float ballSpawnDistance = 1.0f;

	public GameObject pongBall;

	GameObject pongBallInstance;

	bool gameIsStarted = false;

	// TODO: Can this be awake?
	void Start() {
		if (!hasAuthority)
		{
			GetComponentInChildren<Camera>().enabled = false;
			GetComponentInChildren<AudioListener>().enabled = false;
		}
		else
		{
			if (transform.position.x > 0)
			{
				transform.rotation = Quaternion.LookRotation(-Vector3.right);
			}
			else
			{
				transform.rotation = Quaternion.LookRotation(Vector3.right);
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (hasAuthority)
		{
			transform.position += transform.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
			//transform.position += Vector3.up * Input.GetAxis("Vertical") * speed * Time.deltaTime;
		}

		if (!gameIsStarted && isServer && connectionToClient.isConnected)
		{
			OnStartGame();
		}
		// A bit hacky, but good for testing if a client DC's and we want to restart the game
		else if (gameIsStarted && !connectionToClient.isConnected)
		{
			gameIsStarted = false;
		}
	}

	void OnStartGame()
	{
		gameIsStarted = true;
		if (pongBallInstance)
		{
			Destroy(pongBallInstance);
		}
		Vector3 ballSpawnPos = transform.position + transform.forward * ballSpawnDistance;
		pongBallInstance = Instantiate(pongBall, ballSpawnPos, Quaternion.identity);
		NetworkServer.Spawn(pongBallInstance);
	}
}
