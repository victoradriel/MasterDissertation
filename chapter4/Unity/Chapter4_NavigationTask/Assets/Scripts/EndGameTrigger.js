#pragma strict
#pragma implicit
#pragma downcast

class EndGameTrigger extends MonoBehaviour
{
	
	function OnTriggerEnter(other : Collider)
	{
		if(other.name.ToLower() == "chegada")
		{
			
			
            
			Destroy(this);		
		}
	}
}