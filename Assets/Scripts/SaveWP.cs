using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveWP : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject [] gos = GameObject.FindGameObjectsWithTag("WP");

        StreamWriter sw = new StreamWriter("Assets/abc.txt", false);
        Debug.Log(sw);
        // FileStream fs = new FileStream("Assets/abc.txt", FileMode.Create);
        string s = "";
        for(int i = 0; i < gos.Length; i++) {
            s = "";
           s += gos[i].name;
            s += " ";
            WP wp = gos[i].GetComponent<WP>();
            s += wp.iFloor;
            s += " ";
            s += wp.bLink;
            s += " ";
            s += wp.m_Neibors.Count;
            s += " ";
            for (int j = 0; j < wp.m_Neibors.Count; j++)
            {
                s += wp.m_Neibors[j].name;
                s += " ";
            }

            sw.WriteLine(s);
        }
        sw.Close();
        
    }
	
}
