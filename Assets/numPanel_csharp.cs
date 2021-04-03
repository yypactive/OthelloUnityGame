using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class numPanel_csharp: MonoBehaviour {

    public GameObject panel;
    public GameObject label;
    public GameObject numLabel;

    // Use this for initialization
    void Start()
    {
        Text num = numLabel.GetComponentInChildren<Text>();
        num.text = "0";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GetNum()
    {
        Text t = numLabel.GetComponentInChildren<Text>();
        return Convert.ToInt32(t.text);
    }
    public void SetNum(int _num)
    {
        Text t = numLabel.GetComponentInChildren<Text>();
        t.text = _num.ToString();
    }
}
