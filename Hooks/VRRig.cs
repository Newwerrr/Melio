using MelonLoader;
using System.Reflection;
using UnityEngine;

public static class VRRigHooks
{
    public static object GetRigSerializer(object target)
    {
        if (target == null)
        {
            Debug.Log("Target is null.");
            return null;
        }

        FieldInfo field = target.GetType().GetField(
            "rigSerializer",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        if (field == null)
        {
            Debug.Log("Could not find rigSerializer field.");
            return null;
        }

        return field.GetValue(target);
    }

    public static void SetRigSerializer(object target, object newValue)
    {
        if (target == null)
        {
            Debug.Log("Target is null.");
            return;
        }

        FieldInfo field = target.GetType().GetField(
            "rigSerializer",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        if (field == null)
        {
            Debug.Log("Could not find rigSerializer field.");
            return;
        }

        field.SetValue(target, newValue);
    }

    public static object GetNetView(VRRig rig)
    {
        if (rig == null)
        {
            MelonLogger.Error("VRRig is null.");
            return null;
        }

        FieldInfo field = rig.GetType().GetField(
            "netView",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (field == null)
        {
            Debug.Log("Could not find netView field.");
            return null;
        }

        return field.GetValue(rig);
    }
}