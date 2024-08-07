using GamePackages.Core;
using UnityEngine;

namespace GamePackages.Core
{
	public class CameraMovePrototype : MonoBehaviour
	{
		[SerializeField] float moveSensitiveFactor = 10; 
		[SerializeField] float scaleSensitiveFactor = 10;
		[SerializeField] RangeMinMax distanceMinMax = new RangeMinMax(2, 100);
		[SerializeField] Camera thisCamera;
		[SerializeField] Transform owner;
		[SerializeField] bool yAxis;
	    

		float distance;
		float minDistance;

		void Start()
		{
			if (thisCamera.orthographic)
			{
				distance = thisCamera.orthographicSize;
			}
			else
			{
				distance = Vector3.Distance(owner.position, thisCamera.transform.position);
			}
		}

		void LateUpdate()
		{
			float horizontalInput = Input.GetAxis("Horizontal");
			float verticalInput = Input.GetAxis("Vertical");
			Vector3 input = yAxis ? new Vector3(horizontalInput, verticalInput, 0) : new Vector3(horizontalInput, 0, verticalInput);
		    
			distance -= distance * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * scaleSensitiveFactor;
			distance = distanceMinMax.Clamp(distance);
		    
			owner.position += input * (Time.deltaTime * moveSensitiveFactor * distance);
		    
			if (thisCamera.orthographic)
			{
				thisCamera.orthographicSize = distance;
			}
			else
			{
				transform.localPosition = -transform.forward * distance;
			} 
		}
	}
}