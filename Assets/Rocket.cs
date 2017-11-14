using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
	enum State { Alive, Dead, Transcending }
	State state;

	Rigidbody rigidBody;
    AudioSource audioSource;

	[SerializeField] float rotationThrust = 250f;
	[SerializeField] float mainThrust = 1000f;
	[SerializeField] AudioClip mainEngineClip;
	[SerializeField] AudioClip levelFinishClip;
	[SerializeField] AudioClip rocketDeadClip;

	// Use this for initialization
	void Start () {
		state = State.Alive;
		rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}

	void OnCollisionEnter(Collision collision) {
		if (state != State.Alive) { return; }

		switch (collision.gameObject.tag)
		{
		case "Friendly":
			print ("Okay");
			break;
		case "Finish":
			startSuccessSequence();
			break;
		default:
			startDeathSequence();
			break;
		}
	}

	private void startSuccessSequence() {
		audioSource.Stop();
		audioSource.PlayOneShot(levelFinishClip);
		state = State.Transcending;
		Invoke("startNextScene", 1);
	}

	private void startDeathSequence() {
		state = State.Dead;
		audioSource.Stop();
		audioSource.PlayOneShot(rocketDeadClip);
		Invoke("restartGame", 1);
	}

	private void startNextScene() {
		SceneManager.LoadScene(1);
	}

	private void restartGame() {
		SceneManager.LoadScene(0);
	}

	// Update is called once per frame
	void Update () {
		if (state == State.Alive) {
			HandleThrust();
			HandleRotate();
		}
	}

	private void HandleRotate() {
		rigidBody.freezeRotation = true; // take manual control of rotation

		float rotationForFrame = rotationThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow)) {
			transform.Rotate(Vector3.forward * rotationForFrame);
        } else if (Input.GetKey(KeyCode.RightArrow)) {
			transform.Rotate(-Vector3.forward * rotationForFrame);
        }

		rigidBody.freezeRotation = false; // give automatic physics rotate control
	}

	private void HandleThrust() {
		if (Input.GetKey(KeyCode.Space)) {
			float thrustForFrame = mainThrust * Time.deltaTime;

			rigidBody.AddRelativeForce(Vector3.up * thrustForFrame);
			startThrustSound();
		} else {
			audioSource.Stop();
		}
	}

    private void startThrustSound() {
        if (!audioSource.isPlaying) {
			audioSource.PlayOneShot(mainEngineClip);
        }
    }
}
