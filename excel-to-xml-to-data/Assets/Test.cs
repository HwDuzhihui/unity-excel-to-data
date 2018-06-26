using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        foreach(var a in ShopConfig.singleton.m_ShopConfigInfo)
        {
            Debug.Log(a.Value.attack);

            for (int i = 0; i < a.Value.attackUp.Length; i++)
                Debug.Log(a.Value.attackUp[i]);

            for (int i = 0; i < a.Value.descript.Length; i++)
                Debug.Log(a.Value.descript[i]);

            for (int i = 0; i < a.Value.levelUp.Length; i++)
                Debug.Log(a.Value.levelUp[i]);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
