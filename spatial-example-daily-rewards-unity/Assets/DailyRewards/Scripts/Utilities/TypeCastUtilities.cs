using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeCastUtilities
{
    public static Dictionary<string, T> GetDictionary<T>(object dictionary)
    {
        if (dictionary == null)
            return null;

        if (dictionary is Dictionary<string, T> castedDictionary)
        {
            return castedDictionary;
        }

        if (dictionary is Dictionary<string, object> objectDictionary)
        {
            Dictionary<string, T> output = new Dictionary<string, T>();
            foreach (var item in objectDictionary)
            {
                output.Add(item.Key, GetValue<T>(item.Value));
            }
            return output;
        }

        return default;
    }

    public static T GetValue<T>(object value)
    {
        if (value == null)
            return default;

        if (value is T castedValue)
        {
            return castedValue;
        }

        try
        {
            object output = System.Convert.ChangeType(value, typeof(T));
            if (output != null)
            {
                return (T)output;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            return default(T);
        }

        Debug.LogWarning($"unable to convert value: {value} to type: {typeof(T)}");
        return default;
    }
}
