using UnityEngine;
using System.Collections;
using System;

public class moveHorizontal : MonoBehaviour {

	//private double test;
	public Vector3 teste;
	public Vector3 offset;
	public Vector3 bratrackOutput;
	public float translationFactor = 0.001f;

	// Use this for initialization
	void Start () {

		//test = BratrackUDPReceiver.TX;
		//teste = new Vector3(0,0,0);
		teste = new Vector3 (0, 0, 0);
		offset = new Vector3 (0, 0, 0);
		bratrackOutput = new Vector3 (0, 0, 0);

	}
	
	// Update is called once per frame
	void Update () {

		bratrackOutput.Set ((float)BratrackUDPReceiver.TX, (float)BratrackUDPReceiver.TY, (float)BratrackUDPReceiver.TZ);

		float tx = (float)BratrackUDPReceiver.TX;
		float ty = (float)BratrackUDPReceiver.TY;
		float tz = (float)BratrackUDPReceiver.TZ;

		if (Input.anyKeyDown)
						offset.Set (tx, ty, tz);

		if(Input.GetKey(KeyCode.PageUp))
		   translationFactor += 0.001f;

		if(Input.GetKey(KeyCode.PageDown))
		   translationFactor -= 0.001f;
	

		float ttx = (offset.x - tx) * translationFactor;
		float tty = (offset.y - ty) * translationFactor;
		float ttz = (offset.z - tz) * translationFactor;

		teste.Set (ttx, (float)1.5, (float)-0.13);

		transform.localPosition = teste;
//
//		if (Input.anyKeyDown) 
//		{
//			offset = bratrackOutput;
//		}
//
//		teste = (bratrackOutput - offset);
//
//		transform.localPosition += teste;
//	
		print (teste);

	}
}
