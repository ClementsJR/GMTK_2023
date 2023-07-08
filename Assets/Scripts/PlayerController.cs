using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : GenericController {

    /*[Header("Debug")]
    [SerializeField]
    TextMeshProUGUI screenLog;*/

    private new Transform camera;
    private Vector2 rotation;
    private bool onGround;

	private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        rotation = Vector2.zero;
        onGround = true;
        camera = Camera.main.transform;
	}

	private void FixedUpdate() {
        UpdateMovement();
    }

	void Update() {
        UpdateLook();
        onGround = CheckIfGrounded();

        if (Input.GetButtonDown("Fire1") && gun.CanFire()) {
            HandleFire();
		}

        if (Input.GetButtonDown("Jump")) {
            HandleJump();
        }

        if (Input.GetKeyDown(KeyCode.Backspace)) {
           healthSystem.Respawn();
		}
    }

    void UpdateLook() {
        rotation.y += Input.GetAxis("Mouse X");
        rotation.x += -Input.GetAxis("Mouse Y");
        rotation.x = Mathf.Clamp(rotation.x, -20f, 20f);

        transform.eulerAngles = new Vector2(0, rotation.y) * lookSpeed;
        camera.localRotation = Quaternion.Euler(rotation.x * lookSpeed, 0, 0);
    }

    void UpdateMovement() {
        if (!onGround) return;

        Vector3 fwdMovement = Input.GetAxis("Vertical") * moveSpeed * Vector3.forward;
        Vector3 sideMovement = Input.GetAxis("Horizontal") * moveSpeed * Vector3.right;
        Vector3 movement = fwdMovement + sideMovement;
        movement = transform.TransformDirection(movement);
        movement.y = physicsBody.velocity.y;
        physicsBody.velocity = movement;
    }

    void HandleJump() {
        if (!onGround) return;

        Vector3 jump = (Vector3.up + (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"))).normalized * jumpForce;
        physicsBody.AddForce(jump);
        onGround = false;
    }

	void HandleFire() {
        gun.Fire(camera.forward);
	}
}
