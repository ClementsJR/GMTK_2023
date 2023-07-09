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
	[SerializeField]
	float healthRegain = 0.1f;
	[SerializeField]
	float invincibilityTime = 1.5f;

	[Header("UI")]
	[SerializeField]
	Slider healthBar;
	[SerializeField]
	bool displayHealth;

	float currentHealth;
	float currentInvincibility;

	private void OnEnable() {
		currentHealth = maxHealth;
		currentInvincibility = invincibilityTime;

		UpdateDisplay();
	}

	private void Update() {
		if (currentHealth < maxHealth) {
			currentHealth += healthRegain * Time.deltaTime;
			UpdateDisplay();
		}

		if (currentInvincibility > 0) {
			currentInvincibility -= Time.deltaTime;
		}
	}

	private void UpdateDisplay() {
		if (displayHealth)
			healthBar.value = HealthPercent();
	}

	public void DealDamage(Weapon weapon) {
		if (currentInvincibility > 0)
			return;

		currentHealth -= weapon.Damage();
		UpdateDisplay();

		if (currentHealth <= 0) {
			Respawn();
		}
	}

	public void Respawn() {
		spawnHandler.enabled = true;

		controller.enabled = false;
		this.enabled = false;
	}

	public float HealthPercent() {
		return currentHealth / maxHealth;
	}
}
