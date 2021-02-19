using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimMath {
    public static float Lerp(float min, float max, float p, bool allowExtrapolation = true) {
        if (!allowExtrapolation) {
            if (p < 0) return min;
            if (p > 1) return max;
        }
        return (max - min) * p + min;
    }
    public static Vector3 Lerp(Vector3 min, Vector3 max, float p, bool allowExtrapolation = true) {
        if (!allowExtrapolation) {
            if (p < 0) return min;
            if (p > 1) return max;
        }
        return (max - min) * p + min;
    }

    public static Quaternion Lerp(Quaternion min, Quaternion max, float p, bool allowExtrapolation = true) {
        if (!allowExtrapolation) {
            if (p < 0) return min;
            if (p > 1) return max;
        }
        return Quaternion.Lerp(min, max, p);
    }

    public static float Slide(float current, float target, float percentLeftAfter1Second)
    {
        float p = 1 - Mathf.Pow(percentLeftAfter1Second, Time.deltaTime);
        return AnimMath.Lerp(current, target, p);
    }
    public static Vector3 Slide(Vector3 current, Vector3 target, float percentLeftAfter1Second)
    {
        float p = 1 - Mathf.Pow(percentLeftAfter1Second, Time.deltaTime);
        return AnimMath.Lerp(current, target, p);
    }

    public static Quaternion Slide(Quaternion current, Quaternion target, float percentLeftAfter1Second = .05f) {
        float p = 1 - Mathf.Pow(percentLeftAfter1Second, Time.deltaTime);
        return AnimMath.Lerp(current, target, p);
    }

}
