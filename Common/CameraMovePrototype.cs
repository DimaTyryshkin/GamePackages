using GamePackages.Core.Validation;
using UnityEngine;

namespace GamePackages.Core
{
    [RequireComponent(typeof(Camera))]
    internal class CameraMovePrototype : MonoBehaviour
    {
        [SerializeField] float moveSensitiveFactor = 10;
        [SerializeField] float moveSensitiveFactorFast = 50;
        [SerializeField] float scaleSensitiveFactor = 10;
        [SerializeField] float rotateSensitiveFactor = 10;
        [SerializeField] RangeMinMax distanceMinMax = new RangeMinMax(2, 100);
        [SerializeField, IsntNull] Camera thisCamera;
        [SerializeField, IsntNull] Transform owner;
        [SerializeField, IsntNull] Transform startTarget;
        [SerializeField] CameraMode mode;

        Vector3 oldMousePos;
        float distance;
        float minDistance;

        void Start()
        {
            GetComponent<Camera>();
            if (thisCamera.orthographic)
            {
                distance = thisCamera.orthographicSize;
            }
            else
            {
                distance = Vector3.Distance(transform.position, owner.position);
            }

            Vector3 offset = startTarget.position - transform.position;
            if (mode == CameraMode.XY)
                offset.z = 0;
            else
                offset.y = 0;

            transform.position += offset;
            oldMousePos = Input.mousePosition;
        }

        void LateUpdate()
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            Vector3 input = Vector3.zero;

            if (mode == CameraMode.XY)
                input = new Vector3(horizontalInput, verticalInput, 0);

            if (mode == CameraMode.XZ)
                input = new Vector3(horizontalInput, 0, verticalInput);

            if (mode == CameraMode.Fly3D)
            {
                input = transform.right * horizontalInput + transform.forward * verticalInput;

                if (Input.GetKey(KeyCode.Space))
                    input += Vector3.up;

                if (Input.GetKey(KeyCode.LeftControl))
                    input += Vector3.down;

                if (input.sqrMagnitude > 1)
                    input = input.normalized;

                Vector3 mouseDelta = (Input.mousePosition - oldMousePos) * thisCamera.fieldOfView / 90;
                oldMousePos = Input.mousePosition;

                if (Input.GetMouseButtonDown(1)) // Чтобы не куртилась камера при расфокусе с игрового экрана.
                    mouseDelta = Vector3.zero;

                if (Input.GetMouseButton(1))
                {
                    Vector3 angles = transform.localEulerAngles;

                    angles.x = angles.x + -mouseDelta.y * rotateSensitiveFactor;
                    angles.y = angles.y + mouseDelta.x * rotateSensitiveFactor;

                    transform.localEulerAngles = angles;
                }

                if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt))
                {
                    transform.RotateAround(owner.position, Vector3.up, mouseDelta.x * rotateSensitiveFactor);
                    transform.RotateAround(owner.position, transform.right, -mouseDelta.y * rotateSensitiveFactor);
                }

                float wheelInput = Input.GetAxis("Mouse ScrollWheel") * scaleSensitiveFactor;
                Vector3 toOwner = transform.position - owner.position;
                transform.position = owner.position + toOwner * (1 - wheelInput);

                float moveFactor = Input.GetKey(KeyCode.LeftShift) ?
                    moveSensitiveFactorFast :
                    moveSensitiveFactor;

                transform.position += input * (Time.deltaTime * moveFactor);
            }
            else
            {
                distance -= distance * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * scaleSensitiveFactor;
                distance = distanceMinMax.Clamp(distance);

                if (thisCamera.orthographic)
                {
                    thisCamera.orthographicSize = distance;
                }
                else
                {
                    transform.localPosition = -transform.forward * distance;
                }

                owner.position += input * (Time.deltaTime * moveSensitiveFactor * distance);
            }

        }

        internal enum CameraMode
        {
            XZ = 0,
            XY = 1,
            Fly3D = 2,
        }
    }

}