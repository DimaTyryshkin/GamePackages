using GamePackages.Core.Validation;
using UnityEngine;

namespace GamePackages.Core
{
	public class CameraMovePrototype : MonoBehaviour
	{
		[SerializeField] float moveSensitiveFactor = 10; 
		[SerializeField] float scaleSensitiveFactor = 10;
		[SerializeField] RangeMinMax distanceMinMax = new RangeMinMax(2, 100);
		[SerializeField, IsntNull] Camera thisCamera;
		[SerializeField, IsntNull] Transform owner;
		[SerializeField, IsntNull] Transform startTarget;
		[SerializeField] bool xyPlane;
	    

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

			Vector3 offset = startTarget.position - transform.position;
			if (xyPlane)
				offset.z = 0;
			else
				offset.y = 0;

			transform.position += offset;

		}

		void LateUpdate()
		{
			float horizontalInput = Input.GetAxis("Horizontal");
			float verticalInput = Input.GetAxis("Vertical");
			Vector3 input = xyPlane ? new Vector3(horizontalInput, verticalInput, 0) : new Vector3(horizontalInput, 0, verticalInput);
		    
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