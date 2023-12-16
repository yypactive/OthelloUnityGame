using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class statePanel_csharp : MonoBehaviour {

    public GameObject panel;
    public Text label;
    public Text numLabel;

    public Button jumpBtn;

    int state;
    // jump tag
    bool jumped = false;
    // Use this for initialization
    void Start () {
        state = 0;
        //Refresh();
    }
	
	// Update is called once per frame
	void Update () {
        if (Global.IsAIRound())
        {
            var titleStr = GetTitle();
            numLabel.text = titleStr;
        }
	}

    void Refresh()
    {
        var titleStr = GetTitle();
        numLabel.text = titleStr;
        jumpBtn.gameObject.SetActive(jumped);
    }

    public string GetTitle()
    {
        var titleStr = "";
        // 1 means black, -1 means white
        if (state == 1)
        {
            titleStr = "<b>黑子</b>";    
        }
        else if (state == -1)
        {
            titleStr = "白子";  
        }
        else
        {
            titleStr = "ERROR";
        }

        if (Global.IsAIRound())
        {
            var currTs = UI.GetCurrClientTimeStamp();
            titleStr = titleStr + String.Format("<size=30>({0})</size>", currTs - Global.aiEngineStartTs);
        }

        return titleStr;
    }

    public int GetState() {
        return state;
    }

    public void SetState (int _num, bool _jumped = false) {
        state = _num;
        jumped = _jumped;
        Refresh();
    }

    public void SetWin(bool _win)
    {
        if (_win)
        {
            label.text = "<b>胜   利</b>: ";
        }
        else
        {
            label.text = "回   合: ";
        }
    }
}
