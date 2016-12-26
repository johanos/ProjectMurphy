using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class SubtitleController : MonoBehaviour {
    //35 characters is the magic number BTW :)
    public Text TextSource;
	// Use this for initialization
	void Start () {
        TextMesh currentMesh = gameObject.GetComponent<TextMesh>();
        currentMesh.text = TextSource.text;
	}
	
	// Update is called once per frame
	void Update () {
        TextMesh currentMesh = gameObject.GetComponent<TextMesh>();
        currentMesh.text = TextSource.text;
    }
}
