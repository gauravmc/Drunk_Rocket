using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
    Rigidbody rigidBody;
    AudioSource audioSource;

	[SerializeField] float rotationThrust = 250f;
	[SerializeField] float mainThrust = 1000f;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update () {
		Thrust();
		Rotate();
	}

    private void Rotate() {
		rigidBody.freezeRotation = true; // take manual control of rotation

		float rotationForFrame = rotationThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow)) {
			transform.Rotate(Vector3.forward * rotationForFrame);
        } else if (Input.GetKey(KeyCode.RightArrow)) {
			transform.Rotate(-Vector3.forward * rotationForFrame);
        }

		rigidBody.freezeRotation = false; // give automatic physics rotate control
	}

	private void Thrust() {
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
            audioSource.Play();
        }
    }
}
