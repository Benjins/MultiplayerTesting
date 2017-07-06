using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour
{

	public float speed = 4.0f;

	public GameObject pongBall;

	GameObject pongBallInstance;

	bool gameIsStarted = false;

	// Update is called once per frame
	void Update()
	{
		if (hasAuthority)
		{
			transform.position += Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;
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
		pongBallInstance = Instantiate(pongBall, Vector3.zero, Quaternion.identity);
		NetworkServer.Spawn(pongBallInstance);
	}
}
