using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField][Range(0f, 5f)]
    float lookSpeed = 3f;
    [SerializeField][Range(0f, 50f)]
    float moveSpeed = 10f;
    [SerializeField][Range(0f, 5000f)]
    float jumpForce = 1000f;
    [SerializeField]
    Rigidbody physicsBody;

	private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
	}

	void Update() {
        UpdateLook();
        UpdateMovement();
        //UpdateJump();
    }

    void UpdateLook() {
        float horizontalLook = Input.GetAxis("Mouse X");
        transform.Rotate(transform.up * horizontalLook * lookSpeed);

        Transform camera = Camera.main.transform;
        float verticalLook = Mathf.Clamp(-Input.GetAxis("Mouse Y"), -15f, 15f);
        camera.Rotate(camera.right * verticalLook * lookSpeed, Space.World);
    }

    void UpdateMovement() {
        Vector3 fwdMovement = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime * Vector3.forward;
        Vector3 sideMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime * Vector3.right;
        Vector3 movement = fwdMovement + sideMovement;

        transform.Translate(movement,Space.Self);
    }

    void UpdateJump() {
        if (Input.GetButtonDown("Jump")) {
            physicsBody.AddForce(transform.up * jumpForce);
        }
    }
}
