using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField][Range(0.0f, 2.0f)]
    float speed = 1.0f;
    [SerializeField][Range(0.0f, 5000.0f)]
    float jumpForce = 1000.0f;
    [SerializeField]
    Rigidbody physicsBody;

    void Update() {
        Vector3 fwdMovement = Input.GetAxis("Vertical") * speed * transform.forward;
        Vector3 sideMovement = Input.GetAxis("Horizontal") * speed * transform.right;
        Vector3 movement = fwdMovement + sideMovement;

        transform.Translate(movement);

        if(Input.GetButtonDown("Jump")) {
            physicsBody.AddForce(transform.up * jumpForce);
		}
    }
}
