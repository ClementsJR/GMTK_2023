using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    [SerializeField]
    Transform waitArea;
    SphereCollider[] spawnAreas;

	private void Start() {
        spawnAreas = this.GetComponentsInChildren<SphereCollider>();
	}

	public Vector3 GetSpawnLocation() {
        int randIndex = Random.Range(0, spawnAreas.Length);
        SphereCollider area = spawnAreas[randIndex];
        Vector2 pointInArea = Random.insideUnitCircle * area.radius;
        Vector3 spawnPoint = new Vector3(pointInArea.x, 0f, pointInArea.y);
        return area.transform.TransformPoint(spawnPoint);
	}

    public Vector3 GetWaitLocation() {
        return waitArea.position;
	}
}
