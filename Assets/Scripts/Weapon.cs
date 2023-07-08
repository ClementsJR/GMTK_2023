using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    [SerializeField]
    Transform muzzle;
    [SerializeField][Range(0f, 500f)]
    float bulletForce = 250f;

	public void Fire(Vector3 direction) {
        Color lineColor = Color.red;
        if (Physics.Raycast(muzzle.position, direction, out RaycastHit hit)) {
            if (hit.rigidbody != null) {
                hit.rigidbody.AddForceAtPosition(direction * bulletForce, hit.point);
                lineColor = Color.green;
            }
            Debug.DrawLine(muzzle.position, hit.point, lineColor, 2f);
        } else {
            Debug.DrawRay(muzzle.position, direction * 100f, Color.white, 2f);
        }
    }
}
