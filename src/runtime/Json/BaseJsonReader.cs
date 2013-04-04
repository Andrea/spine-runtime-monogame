/// <summary>
/// BaseJsonReader.cs
/// 2013-March
/// </summary>

using System.Globalization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

namespace Spine.Runtime.MonoGame.Json
{
	public abstract class BaseJsonReader
	{
		protected T Read<T>(JToken jsonObject, string name, T defaultValue)
		{
			JToken token = jsonObject[name];
			if (token == null)
			{
				return defaultValue;
			}

			return token.Value<T>();
		}

		protected Color ReadColor(string input)
		{
			uint colorValue = uint.Parse(input, NumberStyles.HexNumber);
			return new Color(
				(colorValue >> 6) & 0xff,
				(colorValue >> 4) & 0xff,
				(colorValue >> 2) & 0xff,
				colorValue & 0xff
				);
		}
	}
}