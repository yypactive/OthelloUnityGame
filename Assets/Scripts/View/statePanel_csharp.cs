using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class statePanel_csharp : MonoBehaviour {

    public GameObject panel;
    public GameObject label;
    public GameObject numLabel;

    int state;
    // Use this for initialization
    void Start () {
        state = 0;
        //Refresh();
    }
	
	// Update is called once per frame
	void Update () {
        if (Global.IsAIRound())
        {
            var text = "";
            if (state == 1)
            {
                text = "黑子";
            }
            else if (state == -1)
            {
                text = "白子";
            }
            var currTs = UI.GetCurrClientTimeStamp();
            text = text + String.Format("<size=30>({0})</size>", currTs - Global.aiEngineStartTs);
            Text t = numLabel.GetComponentInChildren<Text>();
            t.text = text;
        }
	}

    void Refresh()
    {
        Text t = numLabel.GetComponentInChildren<Text>();
        // 1 means black, -1 means white
        if (state == 1)
        {
            t.text = "黑子";
        }
        else if (state == -1)
        {
            t.text = "白子";
        }
        else if (state == 0)
        {
            t.text = "双方";
        }
        else
        {
            t.text = "ERROR";
        }
    }

    public int GetState() {
        return state;
    }

    public void SetState (int _num) {
        state = _num;
        Refresh();
    }

    public void SetWin(bool _win)
    {
        if (_win)
        {
            label.GetComponentInChildren<Text>().text = "胜   利: ";
        }
        else
        {
            label.GetComponentInChildren<Text>().text = "回   合: ";
        }
    }
}
