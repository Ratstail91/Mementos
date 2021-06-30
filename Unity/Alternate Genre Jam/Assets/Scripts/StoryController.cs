using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class StoryController : MonoBehaviour {
	class StorySegment {
		public TextAsset asset;
		public Story story;
		public Canvas canvas; //hold the clickable images
	}

	Stack<StorySegment> assetStack = new Stack<StorySegment>();

	Dictionary<string, string> variables = new Dictionary<string, string>();

	string textFull;
	bool autoFinishText = false;
	bool resetLineCoroutineRunning = false;
	bool gameEnd = false;

	[SerializeField]
	TMP_Text textBox;

	[SerializeField]
	Canvas choiceCanvas;

	[SerializeField]
	Canvas clickableParentCanvas;

	[SerializeField]
	Canvas backgroundCanvas;

	[SerializeField]
	Canvas zoomedCanvas;

	[SerializeField]
	Image imagePrefab;

	[SerializeField]
	Button buttonPrefab;

	[SerializeField]
	GameObject clickablePrefab;

	[SerializeField]
	GameObject zoomedImagePrefab;

	[Header("Start Point")]
	[SerializeField]
	TextAsset startPoint = null;

	void Awake() {
		//
	}

	void Start() {
		//load the resources
		AudioController audio = GameObject.FindObjectsOfType<AudioController>()[0];
		audio.Load("letters", "Audio/letters");

		//kick off
		PushInkAsset(startPoint);
	}

	void Update() {
		//TODO: scrolling text
	}

	void OnMouseDown() {
		NextLine();
	}

	//controllers
	public void PushInkAsset(TextAsset asset) {
		StorySegment segment = new StorySegment();

		segment.asset = asset;
		segment.story = new Story(asset.text);

		GameObject go = new GameObject();
		go.name = "Layer Canvas";
		go.transform.SetParent(clickableParentCanvas.gameObject.transform);
		segment.canvas = go.AddComponent<Canvas>();
		((RectTransform)(go.transform)).anchorMin = Vector2.zero;
		((RectTransform)(go.transform)).anchorMax = new Vector2(1, 1);
		((RectTransform)(go.transform)).offsetMin = Vector2.zero;
		((RectTransform)(go.transform)).offsetMax = Vector2.zero;

		//callbacks
		segment.story.BindExternalFunction("SetVariable", (string key, string value) => {
			variables[key] = value;
		});

		segment.story.BindExternalFunction("GetVariable", (string key) => {
			if (variables.ContainsKey(key)) {
				return variables[key];
			} else {
				return "";
			}
		});

		segment.story.BindExternalFunction("SetBackground", (string fname, float seconds) => {
			//destroy existing background after
			if (backgroundCanvas.transform.childCount > 0) {
				GameObject.Destroy(backgroundCanvas.transform.GetChild(0).gameObject, seconds);
			}

			Texture2D texture = Resources.Load<Texture2D>("Visuals/" + fname);

			if (texture == null) {
				return;
			}

			GameObject go = Instantiate(imagePrefab, backgroundCanvas.gameObject.transform).gameObject;

			go.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f,0.5f), 100.0f);

			go.GetComponent<Image>().color = new Color(1, 1, 1, seconds == 0 ? 1 : 0);

			if (seconds > 0) {
				StartCoroutine(IncreaseAlphaColor(go.GetComponent<Image>(), seconds));
			}
		});

		segment.story.BindExternalFunction("SetMusic", (string fname, float fadeOut, float fadeIn) => {
			StartCoroutine(SetMusicCoroutine(fname, fadeOut, fadeIn));
		});

		segment.story.BindExternalFunction("PlaySound", (string fname) => {
			AudioController audio = GameObject.FindObjectsOfType<AudioController>()[0];

			if (!audio.GetLoaded(fname)) {
				audio.Load(fname, "Audio/" + fname);
			}

			audio.Play(fname);
		});

		segment.story.BindExternalFunction("AddClickable", (string fname, string ink, float x, float y) => {
			AddClickable(fname, ink, new Vector2(x, y));
		});

		segment.story.BindExternalFunction("RemoveClickable", (string name) => {
			RemoveClickable(name);
		});

		segment.story.BindExternalFunction("AddZoomedImage", (string fname, float x, float y) => {
			AddZoomedImage(fname, new Vector2(x, y));
		});

		segment.story.BindExternalFunction("RemoveZoomedImage", (string name) => {
			RemoveZoomedImage(name);
		});

		if (assetStack.Count > 0) { //disable the lower canvas
			assetStack.Peek().canvas.gameObject.SetActive(false);
		}
		assetStack.Push(segment);

		RemoveCanvasChildren(choiceCanvas);
		textBox.text = null;
		textFull = "";

		NextLine();
	}

	IEnumerator IncreaseAlphaColor(Image image, float seconds) {
		while(image.color.a < 1) {
			image.color = new Color(1, 1, 1, image.color.a + 0.1f / seconds);
			yield return new WaitForSeconds(0.1f / seconds);
		}
	}

	IEnumerator SetMusicCoroutine(string fname, float fadeOut, float fadeIn) {
		AudioController audio = GameObject.FindObjectsOfType<AudioController>()[0];

		if (audio.GetLoaded("music") && audio.GetPlaying("music")) {
			audio.StopFadeOut("music", fadeOut);
			yield return new WaitForSeconds(fadeOut);
			audio.Unload("music");
		}

		audio.Load("music", "Audio/" + fname);
		audio.PlayFadeIn("music", fadeIn, AudioController.Mode.LOOP);
	}

	public void PopInkAsset() {
		if (assetStack.Count == 0) {
			return;
		}

		GameObject.Destroy(assetStack.Peek().canvas.gameObject);
		assetStack.Pop();
		if (assetStack.Count > 0) {
			assetStack.Peek().canvas.gameObject.SetActive(true); //re-enable the canvas
		}
		RemoveCanvasChildren(choiceCanvas);
		textBox.text = null;
		ResetLine(false);
	}

	void NextLine() {
		if (resetLineCoroutineRunning || gameEnd) {
			autoFinishText = true;
			return;
		}

		if (assetStack.Count == 0) {
			textFull = "Programming: Ratstail91\nArt: Crystal\nAudio: Silas / Silence";
			StartCoroutine(ResetLineCallback(true));
			gameEnd = true;
			return;
		}

		if (!assetStack.Peek().story.canContinue) {
			if (assetStack.Peek().story.currentChoices.Count == 0) {
				PopInkAsset();
			}
			return;
		}

		ResetLine(true);
	}

	void ResetLine(bool cont) {
		RemoveCanvasChildren(choiceCanvas);

		if (assetStack.Count == 0) {
			return;
		}

		//actually show the next part
		if (cont) {
			textFull = assetStack.Peek().story.Continue();
		} else {
			textFull = assetStack.Peek().story.currentText;
		}

		StartCoroutine(ResetLineCallback(cont));
	}

	IEnumerator ResetLineCallback(bool cont) {
		resetLineCoroutineRunning = true;
		textBox.text = "";

		AudioController audio = GameObject.FindObjectsOfType<AudioController>()[0];

		audio.Unpause("letters", AudioController.Mode.LOOP);

		//scroll along the text
		for (int i = 0; i < textFull.Length && !autoFinishText && cont; i++) {
			textBox.text += textFull[i];
			yield return new WaitForSeconds(0.05f);

			//pause on a period
			if (i < textFull.Length && textFull[i] == '.') {
				yield return new WaitForSeconds(0.1f);
			}
		}

		audio.Pause("letters");

		textBox.text = textFull;

		autoFinishText = false;

		//if we're at a choice
		if (assetStack.Count > 0 && assetStack.Peek().story.currentChoices.Count > 0) {
			for (int i = 0; i < assetStack.Peek().story.currentChoices.Count; i++) {
				Choice choice = assetStack.Peek().story.currentChoices[i];
				Button button = DisplayButton(choice.text.Trim());
				button.onClick.AddListener(() => { assetStack.Peek().story.ChooseChoiceIndex(choice.index); NextLine(); });
			}
		}
		resetLineCoroutineRunning = false;

		if (textFull == "") {
			NextLine();
		}
	}

	//remove children from a canvas
	void RemoveCanvasChildren(Canvas canv) {
		int childCount = canv.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i) {
			GameObject.Destroy (canv.transform.GetChild(i).gameObject);
		}
	}

	//create and display a button on choiceCanvas
	Button DisplayButton(string text) {
		Button button = Instantiate(buttonPrefab) as Button;
		button.transform.SetParent(choiceCanvas.transform, false);

		TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
		buttonText.text = text;

		return button;
	}

	void AddClickable(string image, string ink, Vector2 position) {
		if (assetStack.Count == 0) {
			Debug.LogError("Can't add a clickable to an empty stack");
			return;
		}

		Texture2D texture = Resources.Load<Texture2D>("Visuals/" + image);

		if (texture == null) {
			Debug.LogError("Clickable visual is null");
			return;
		}

		GameObject go = Instantiate(clickablePrefab, position, Quaternion.identity, assetStack.Peek().canvas.gameObject.transform);
		go.name = image;

		go.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f,0.5f), 100.0f);
		go.GetComponent<Clickable>().inkFilename = ink;

		if (go.GetComponent<Image>().sprite == null) {
			Debug.LogError("Clickable sprite is null");
		}

		//BUGFIX: ink, wtf?
		clickableParentCanvas.gameObject.SetActive(false);
		clickableParentCanvas.gameObject.SetActive(true);
	}

	void RemoveClickable(string image) {
		GameObject go = assetStack.Peek().canvas.gameObject.transform.Find(image)?.gameObject;

		if (go != null) {
			GameObject.Destroy(go);
		} else {
			Debug.LogError("Failed to find the GameObject " + image);
		}
	}

	void AddZoomedImage(string image, Vector2 position) {
		if (assetStack.Count == 0) {
			Debug.LogError("Can't add a zoomed image to an empty stack");
			return;
		}

		Texture2D texture = Resources.Load<Texture2D>("Visuals/" + image);

		if (texture == null) {
			Debug.LogError("zoomed image visual is null");
			return;
		}

		GameObject go = Instantiate(zoomedImagePrefab, position, Quaternion.identity, zoomedCanvas.gameObject.transform);
		go.name = image;

		go.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f,0.5f), 100.0f);

		if (go.GetComponent<Image>().sprite == null) {
			Debug.LogError("Zoomed image sprite is null");
		}

		//BUGFIX: ink, wtf?
		zoomedCanvas.gameObject.SetActive(false);
		zoomedCanvas.gameObject.SetActive(true);
	}

	void RemoveZoomedImage(string image) {
		GameObject go = zoomedCanvas.gameObject.transform.Find(image)?.gameObject;

		if (go != null) {
			GameObject.Destroy(go);
		} else {
			Debug.LogError("Failed to find the GameObject " + image);
		}
	}
}