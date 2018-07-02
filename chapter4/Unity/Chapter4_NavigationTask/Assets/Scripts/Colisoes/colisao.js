#pragma strict

function Start () {

}

function Update () {

  var fwd = transform.TransformDirection (Vector3.forward);
    if (Physics.Raycast (transform.position, fwd, 10)) {
        print ("There is something in front of the object!");
    }

}