using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour {

    [SerializeField]
    SpawnHandler spawnHandler;
	[SerializeField]
	GenericController controller;
    [SerializeField]
    float maxHealth = 5f;

	[Header("UI")]
	[SerializeField]
	Slider healthBar;
	[SerializeField]
	bool displayHealth;

	float currentHealth;

	private void OnEnable() {
		currentHealth = maxHealth;
	}

	public void DealDamage(float damage) {
		currentHealth -= damage;

		if (displayHealth)
			healthBar.value = currentHealth / maxHealth;

		if (currentHealth <= 0) {
			Respawn();
		}
	}

	public void Respawn() {
		spawnHandler.enabled = true;

		controller.enabled = false;
		this.enabled = false;
	}
}
