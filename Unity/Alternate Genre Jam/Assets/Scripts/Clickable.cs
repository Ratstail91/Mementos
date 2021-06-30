using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour {
	public string inkFilename;

	void OnMouseDown() {
		TextAsset asset;
		asset = Resources.Load<TextAsset>("Story/" + inkFilename);

		if (asset == null) {
			Debug.LogError("Asset file is null");
			return;
		}

		StoryController controller = FindObjectsOfType<StoryController>()[0];
		controller.PushInkAsset(asset);
	}
}
