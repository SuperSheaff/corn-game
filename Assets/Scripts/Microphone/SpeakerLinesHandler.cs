using UnityEngine;
using UnityEngine.UI;

public class SpeakerLinesHandler : MonoBehaviour {

	[SerializeField] private Canvas canvas;

	[SerializeField] private Image animImage;
	[SerializeField] private Sprite[] sprites;

	[SerializeField] private float animationSpeed = 0.5f;

	private float animationTimer = 0;
	private int animIndex = 0;

	public void IsVoiceActive(bool active) {
		canvas.enabled = active;
	}

	private void Update() {
		animationTimer += Time.deltaTime;

		if (animationTimer >= animationSpeed) {
			animationTimer = 0;
			if (animIndex + 1 >= sprites.Length) {
				animIndex = 0;
			} else {
				animIndex++;
			}

			animImage.sprite = sprites[animIndex];
		}
	}
}
