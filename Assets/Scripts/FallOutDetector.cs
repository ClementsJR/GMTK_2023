using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallOutDetector : MonoBehaviour {

    [SerializeField]
    private Vector3 respawnLocation;

    void OnTriggerExit(Collider other) {
        other.attachedRigidbody.Sleep();
        other.transform.position = respawnLocation;
	}
}
