#pragma strict

var espaco=10;

function Start () {

}

function Update () {

}


function OnGUI () {

//var luz1= gameObject.GetComponentsInChildren("Luz");



GUI.Box(Rect(0,Screen.height-Screen.height/5,Screen.width,Screen.height), "Painel de Controle");

if (GUI.Button(Rect(10,Screen.height-70,70,30), "Luz 100%"))
{


Debug.Log("Luz 100%");

}
if (GUI.Button(Rect(100,Screen.height-70,70,30), "Luz 70%"))
{
//luz1=0.7;
Debug.Log("Luz 70%");

}

if (GUI.Button(Rect(190,Screen.height-70,70,30), "Luz 30%"))
{
Debug.Log("Luz 30%");
//luz1=0.3;
}





}