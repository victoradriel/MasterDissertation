#pragma strict

function Start () {
//guiText.text="Status: Starting";


}

var box : Transform;
 
function Update () {
 
 var dist : float = Vector3.Distance(box.position, transform.position);
 Debug.Log(dist);
 
 if(dist <= 40.0){
  // guiText.text="Status: Batida"; 
 }else{
//  guiText.text="Status: Nao esta batendo";
 }
}