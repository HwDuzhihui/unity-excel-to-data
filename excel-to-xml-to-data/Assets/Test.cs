using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        foreach(var a in ShopConfig.singleton.m_ShopConfigInfo)
        {
            Debug.Log(a.Value.attack);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
