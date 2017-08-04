using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEasings {

    private delegate float AnimationEasingMethod(float x);

    public static float Linear(float x)
    {
        return x;
    }

    public static float EaseInSine(float x)
    {
        return 1f - Mathf.Cos(x * Mathf.PI / 2f);
    }

    public static float EaseOutSine(float x)
    {
        return Mathf.Sin(x * Mathf.PI / 2f);
    }

    public static float EaseInOutSine(float x)
    {
        return 0.5f * (1f - Mathf.Cos(Mathf.PI * x));
    }

    public static float EaseInQuintic(float x)
    {
        return x * x * x * x * x;
    }

    public static float EaseOutQuintic(float x)
    {
        return 1f + ((x -= 1f) * x * x * x * x);
    }

    public static float EaseInOutQuintic(float x)
    {
        if ((x *= 2f) < 1f) return 0.5f * x * x * x * x * x;
        return 0.5f * ((x -= 2f) * x * x * x * x + 2f);
    }

    public static float EaseInExponential(float x)
    {
        return x == 0f ? 0f : Mathf.Pow(1024f, x - 1f);
    }

    public static float EaseOutExponential(float x)
    {
        return x == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);
    }

    public static float EaseInOutExponential(float x)
    {
        if (x == 0f) return 0f;
        if (x == 1f) return 1f;
        if ((x *= 2f) < 1f) return 0.5f * Mathf.Pow(1024f, x - 1f);
        return 0.5f * (-Mathf.Pow(2f, -10f * (x - 1f)) + 2f);
    }

    public static float EaseInCircular(float x)
    {
        return 1f - Mathf.Sqrt(1f - x * x);
    }

    public static float EaseOutCircular(float x)
    {
        return Mathf.Sqrt(1f - ((x -= 1f) * x));
    }

    public static float EaseInOutCircular(float x)
    {
        if ((x *= 2f) < 1f) return -0.5f * (Mathf.Sqrt(1f - x * x) - 1);
        return 0.5f * (Mathf.Sqrt(1f - (x -= 2f) * x) + 1f);
    }

    public static float EaseInBounce(float x)
    {
        return 1f - EaseOutBounce(1f - x);
    }

    public static float EaseOutBounce(float x)
    {
        if (x < (1f / 2.75f))
        {
            return 7.5625f * x * x;
        }
        else if (x < (2f / 2.75f))
        {
            return 7.5625f * (x -= (1.5f / 2.75f)) * x + 0.75f;
        }
        else if (x < (2.5f / 2.75f))
        {
            return 7.5625f * (x -= (2.25f / 2.75f)) * x + 0.9375f;
        }
        else
        {
            return 7.5625f * (x -= (2.625f / 2.75f)) * x + 0.984375f;
        }
    }

    public static float EaseInOutBounce(float x)
    {
        if (x < 0.5f) return EaseInBounce(x * 2f) * 0.5f;
        return EaseOutBounce(x * 2f - 1f) * 0.5f + 0.5f;
    }

    public static float AnimationEasingByType(AnimationEasing easingType, float x)
    {
        return AnimationEasings._animationEasings[easingType](x);
    }

    private static Dictionary<AnimationEasing, AnimationEasingMethod> _animationEasings = new Dictionary<AnimationEasing, AnimationEasingMethod>()
    {
        {AnimationEasing.Linear, AnimationEasings.Linear},
        {AnimationEasing.EaseInSine, AnimationEasings.EaseInSine},
        {AnimationEasing.EaseOutSine, AnimationEasings.EaseOutSine},
        {AnimationEasing.EaseInOutSine, AnimationEasings.EaseInOutSine},
        {AnimationEasing.EaseInQuintic, AnimationEasings.EaseInQuintic},
        {AnimationEasing.EaseOutQuintic, AnimationEasings.EaseOutQuintic},
        {AnimationEasing.EaseInOutQuintic, AnimationEasings.EaseInOutQuintic},
        {AnimationEasing.EaseInExponential, AnimationEasings.EaseInExponential},
        {AnimationEasing.EaseOutExponential, AnimationEasings.EaseOutExponential},
        {AnimationEasing.EaseInOutExponential, AnimationEasings.EaseInOutExponential},
        {AnimationEasing.EaseInCircular, AnimationEasings.EaseInCircular},
        {AnimationEasing.EaseOutCircular, AnimationEasings.EaseOutCircular},
        {AnimationEasing.EaseInOutCircular, AnimationEasings.EaseInOutCircular},
        {AnimationEasing.EaseInBounce, AnimationEasings.EaseInBounce},
        {AnimationEasing.EaseOutBounce, AnimationEasings.EaseOutBounce},
        {AnimationEasing.EaseInOutBounce, AnimationEasings.EaseInOutBounce}
    };
}

public enum AnimationEasing
{
    Linear,
    EaseInSine,
    EaseOutSine,
    EaseInOutSine,
    EaseInQuintic,
    EaseOutQuintic,
    EaseInOutQuintic,
    EaseInExponential,
    EaseOutExponential,
    EaseInOutExponential,
    EaseInCircular,
    EaseOutCircular,
    EaseInOutCircular,
    EaseInBounce,
    EaseOutBounce,
    EaseInOutBounce
}