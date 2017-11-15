using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {
    [SerializeField] float period = 2f;
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);

    Vector3 startingPos;

	// Use this for initialization
	void Start () {
        startingPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        oscillate();
    }

    private void oscillate() {
        if (period <= Mathf.Epsilon) { return; }

        float cycles = Time.time / period;
        float tau = Mathf.PI * 2;
        float movementFactor = Mathf.Sin(tau * cycles) / 2f + 0.5f;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;        
    }
}
