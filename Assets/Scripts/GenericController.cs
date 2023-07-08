using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericController : MonoBehaviour {
    [Header("Movement")]
    [SerializeField][Range(0f, 5f)]
    protected float lookSpeed = 3f;
    [SerializeField][Range(0f, 50f)]
    protected float moveSpeed = 10f;
    [SerializeField][Range(0f, 5000f)]
    protected float jumpForce = 1000f;
    [SerializeField]
    protected Transform feet;
    [SerializeField][Range(0f, 1f)]
    protected float groundedMaxDistance = 0.25f;
    [SerializeField]
    protected Rigidbody physicsBody;

    [Header("Gun")]
    [SerializeField]
    protected Weapon gun;

    [Header("Health")]
    [SerializeField]
    protected HealthSystem healthSystem;

    protected bool CheckIfGrounded() {
        return (Physics.Raycast(feet.position, Vector3.down, out RaycastHit hit, groundedMaxDistance));
    }
}
