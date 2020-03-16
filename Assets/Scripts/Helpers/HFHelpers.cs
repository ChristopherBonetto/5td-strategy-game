public static class HFHelpers
{
	#region Enums

	/// <summary>
	/// Create a dictionary using all possible enum values of a given type as keys
	/// </summary>
	/// <typeparam name="T">Enum type</typeparam>
	/// <typeparam name="U">Any type</typeparam>
	/// <returns>Initialised dictionary</returns>
	public static System.Collections.Generic.Dictionary<T, U> CreateEnumDictionary<T, U>() where T : System.Enum
	{
		System.Collections.Generic.Dictionary<T, U> dict = new System.Collections.Generic.Dictionary<T, U>();

		InitEnumDictionary(dict);

		return dict;
	}

	/// <summary>
	/// Initialise an existing dictionary using all possible enum values of a given type as keys
	/// </summary>
	/// <typeparam name="T">Enum type</typeparam>
	/// <typeparam name="U">Any type</typeparam>
	/// <param name="dict">Dictionary to initialise</param>
	public static void InitEnumDictionary<T, U>(System.Collections.Generic.Dictionary<T, U> dict) where T : System.Enum
	{
		dict.Clear();

		T[] names = EnumToArray<T>();

		for (int i = 0; i < names.Length; i++)
		{
			dict.Add(names[i], default);
		}
	}

	/// <summary>
	/// Fill an array with all possible enum values of a given type
	/// </summary>
	/// <typeparam name="T">Enum type</typeparam>
	/// <returns>Array with enum values</returns>
	public static T[] EnumToArray<T>() where T : System.Enum
	{
		System.Array values = System.Enum.GetValues(typeof(T));
		T[] names = new T[values.Length];
		values.CopyTo(names, 0);
		return names;
	}

	public static string ReplaceEnumWithValues<T>(string text) where T : System.Enum
	{
		// Prepare substitution dictionary
		T[] names = EnumToArray<T>();
		System.Collections.Generic.Dictionary<string, string> subs = new System.Collections.Generic.Dictionary<string, string>();

		foreach (T name in names)
		{
			string enumString = "\"" + name + "\"";
			if (subs.ContainsKey(enumString))
			{
				continue;
			}

			if (typeof(int).IsAssignableFrom(System.Enum.GetUnderlyingType(typeof(T))))
			{
				subs.Add(enumString, ((int)(object)name).ToString());
			}
		}

		// Perform substitution
		foreach (var sub in subs)
		{
			text = text.Replace(sub.Key, sub.Value);
		}

		return text;
	}

	#endregion

	#region Math

	/// <summary>
	/// Modulus operator
	/// </summary>
	/// <param name="x">Input integer</param>
	/// <param name="m">Modulus length</param>
	/// <returns>Modulus difference</returns>
	public static int Modulus(int x, int m)
	{
		int remainder = x % m;
		return remainder < 0 ? remainder + m : remainder;
	}

	#endregion

	#region Logs

	public static void NullCheck(UnityEngine.GameObject holder, UnityEngine.Object reference, string logName)
	{
		if(!reference)
		{
			UnityEngine.Debug.LogError(holder.name + " has no " + logName + " reference.");
		}
	}

	public static void NullCheck(UnityEngine.GameObject holder, UnityEngine.Object[] references, string logName)
	{
		foreach (UnityEngine.Object reference in references)
		{
			if (!reference)
			{
				UnityEngine.Debug.LogError(holder.name + " has a missing reference in " + logName + " array.");
			}
		}
	}

	#endregion
}

#region Json

public static class JsonHelper
{
	public static T[] FromJson<T>(string json)
	{
		Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
		return wrapper.Items;
	}

	public static string ToJson<T>(T[] array)
	{
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.Items = array;
		return UnityEngine.JsonUtility.ToJson(wrapper);
	}

	[System.Serializable]
	private class Wrapper<T>
	{
		public T[] Items;
	}
}

#endregion

#region Extensions

public static class HFExtensionMethods
{
	public static string ToTitleCase(this string s)
	{
		return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLowerInvariant());
	}
}

#endregion
