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

	[SerializeField] ParticleSystem leftBoosterThrust;
	[SerializeField] ParticleSystem rightBoosterThrust;
	[SerializeField] ParticleSystem successParticles;
	[SerializeField] ParticleSystem explosionParticles;

    bool immortalMode = false;

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
		successParticles.Play();
		audioSource.Stop();
		audioSource.PlayOneShot(levelFinishClip);
		state = State.Transcending;
		Invoke("startNextScene", 1);
	}

	private void startDeathSequence() {
        if (immortalMode) { return; }
		state = State.Dead;
		thrustParticlesStop();
		explosionParticles.Play();
		audioSource.Stop();
		audioSource.PlayOneShot(rocketDeadClip);
		Invoke("restartGame", 1);
	}

	private void startNextScene() {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings) {
            nextSceneIndex = 0;
        }

        SceneManager.LoadScene(nextSceneIndex);
	}

	private void restartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	// Update is called once per frame
	void Update () {
        if (state == State.Alive) {
			HandleThrust();
			HandleRotate();
		}

        if (Debug.isDebugBuild) {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys() {
        if (Input.GetKeyDown(KeyCode.L)) {
            startNextScene();
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            immortalMode = !immortalMode;
        }
    }

    private void HandleRotate() {
        rigidBody.angularVelocity = Vector3.zero;

		float rotationForFrame = rotationThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow)) {
			transform.Rotate(Vector3.forward * rotationForFrame);
        } else if (Input.GetKey(KeyCode.RightArrow)) {
			transform.Rotate(-Vector3.forward * rotationForFrame);
        }
	}

	private void HandleThrust() {
		if (Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
        }
        else {
            StopApplyingThrust();
        }
    }

    private void ApplyThrust() {
        float thrustForFrame = mainThrust * Time.deltaTime;

        rigidBody.AddRelativeForce(Vector3.up * thrustForFrame);
        startThrustSound();
    }

    private void StopApplyingThrust() {
        audioSource.Stop();
        thrustParticlesStop();
    }

    private void startThrustSound() {
        if (!audioSource.isPlaying) {
			audioSource.PlayOneShot(mainEngineClip);
        }

		thrustParticlesStart();
    }

	private void thrustParticlesStart() {
		leftBoosterThrust.Play();
		rightBoosterThrust.Play();		
	}

	private void thrustParticlesStop() {
		leftBoosterThrust.Stop();
		rightBoosterThrust.Stop();
	}
}
