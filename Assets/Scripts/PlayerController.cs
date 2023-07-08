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

    private Vector2 rotation;

	private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        rotation = Vector2.zero;
	}

	void Update() {
        UpdateLook();
        UpdateMovement();
        //UpdateJump();
    }

    void UpdateLook() {
        Transform camera = Camera.main.transform;

        rotation.y += Input.GetAxis("Mouse X");
        rotation.x += -Input.GetAxis("Mouse Y");
        rotation.x = Mathf.Clamp(rotation.x, -15f, 15f);

        transform.eulerAngles = new Vector2(0, rotation.y) * lookSpeed;
        camera.localRotation = Quaternion.Euler(rotation.x * lookSpeed, 0, 0);
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
