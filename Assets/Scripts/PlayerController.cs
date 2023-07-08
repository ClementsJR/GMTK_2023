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

    [SerializeField]
    SoundEffector gunSoundEffects;

    private Vector2 rotation;
    private bool onGround;

	private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        rotation = Vector2.zero;
        onGround = true;
	}

	void Update() {
        UpdateLook();
        UpdateMovement();

        if (Input.GetButtonDown("Jump")) {
            HandleJump();
        }

        if (Input.GetButtonDown("Fire1")) {
            HandleFire();
		}
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
        if (!onGround) return;

        Vector3 fwdMovement = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime * Vector3.forward;
        Vector3 sideMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime * Vector3.right;
        Vector3 movement = fwdMovement + sideMovement;

        transform.Translate(movement,Space.Self);
    }

    void HandleJump() {
        if (!onGround) return;

        Vector3 jump = (Vector3.up + (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"))).normalized * jumpForce;
        physicsBody.AddForce(jump);
        onGround = false;
    }

	private void OnTriggerEnter(Collider other) {
        onGround = true;
	}

	void HandleFire() {
        gunSoundEffects.Play();
	}
}
