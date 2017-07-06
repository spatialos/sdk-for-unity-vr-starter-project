using Improbable;
using UnityEngine;

namespace Assets.Gamelogic.Utils
{
    public static class MathUtils {

        private const float Epsilon = 0.001f;

        public static bool ApproximatelyEqual(Vector3 a, Vector3 b)
        {
            return ApproximatelyEqual(a.x, b.x) 
                && ApproximatelyEqual(a.y, b.y)
                && ApproximatelyEqual(a.z, b.z);
        }

        public static bool ApproximatelyEqual(Vector3d a, Vector3d b)
        {
            return ApproximatelyEqual(a.X, b.X)
                && ApproximatelyEqual(a.Y, b.Y)
                && ApproximatelyEqual(a.Z, b.Z);
        }

        public static bool ApproximatelyEqual(Vector3f a, Vector3f b)
        {
            return ApproximatelyEqual(a.X, b.X)
                && ApproximatelyEqual(a.Y, b.Y)
                && ApproximatelyEqual(a.Z, b.Z);
        }

        public static bool ApproximatelyEqual(Improbable.Coordinates a, Improbable.Coordinates b)
        {
            return ApproximatelyEqual(a.X, b.X)
                && ApproximatelyEqual(a.Y, b.Y)
                && ApproximatelyEqual(a.Z, b.Z);
        }

        public static bool ApproximatelyEqual(Quaternion a, Quaternion b)
        {
            return ApproximatelyEqual(a.eulerAngles, b.eulerAngles);
        }

        public static bool ApproximatelyEqual(float a, float b)
        {
            return Mathf.Abs(a - b) < Epsilon;
        }

        public static bool ApproximatelyEqual(double a, double b)
        {
            return ApproximatelyEqual((float) a, (float) b);
        }

        public static Quaternion ToUnityQuaternion(Improbable.Global.Quaternion quaternion)
        {
            return new Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        public static Improbable.Global.Quaternion ToSpatialQuaternion(Quaternion quaternion)
        {
            return new Improbable.Global.Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
    }

    public static class Vector3Extensions
    {
        public static Improbable.Coordinates ToCoordinates(this Vector3 vector3)
        {
            return new Improbable.Coordinates(vector3.x, vector3.y, vector3.z);
        }

        public static Vector3f ToVector3f(this Vector3 vector3)
        {
            return new Vector3f(vector3.x, vector3.y, vector3.z);
        }
    }
}
