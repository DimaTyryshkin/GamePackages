using UnityEngine;

namespace GamePackages.Core
{
    public enum Plaine
    {
        YZ = 0,
        XZ = 1,
        XY = 2
    }

    public static class CameraExtension
    {
        public static Vector3 ScreenPointToWorldPointOnPlane(this Camera camera, Vector3 screenPoint, Plaine onPlane)
        {
            var r = camera.ScreenPointToRay(screenPoint);

            Vector3 dir = r.direction.normalized;
            int i = (int)onPlane;

            float t = -r.origin[i] / dir[i];
            return r.GetPoint(t);
        }

        public static void Lerp(this Camera camera, Camera from, Camera to, float t)
        {
            camera.transform.position = Vector3.Lerp(from.transform.position, to.transform.position, t);
            camera.transform.rotation = Quaternion.Lerp(from.transform.rotation, to.transform.rotation, t);

            if (camera.orthographic)
                camera.orthographicSize = Mathf.Lerp(from.orthographicSize, to.orthographicSize, t);
            else
                camera.fieldOfView = Mathf.Lerp(from.fieldOfView, to.fieldOfView, t);
        }
    }
}