using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class onclick : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.GetComponent<Button>().onClick.AddListener(delegate()
        {
            // debug
            Debug.Log("click restart btn");
            Global.RestartGame();
           
        });
    }

    // Update is called once per frame
    void Update () {
		
	}
}
