using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Spine.Runtime.MonoGame.Graphics;

namespace Spine.Runtime.MonoGame.Json
{
	public class TextureMapJsonReader : BaseJsonReader
	{
		/*
		 * Json Atlas file expected in the following format.  Rotation is ignored - we expect all images to be correctly rotated

		{"frames": [	
			{
				"filename": "BackLowerLegLeft.png",
			    "frame": {"x":185,"y":106,"w":36,"h":69},
				"rotated": false,
			},
			{
				"filename": "BackLowerLegRight.png",
				"frame": {"x":147,"y":106,"w":36,"h":69},
				"rotated": false,
			},

	    */
#if WINDOWS
		public TextureAtlas ReadTextureJsonFile(string jsonFile, Texture2D texture)
#elif ANDROID
		public TextureAtlas ReadTextureJsonFile (AndroidGameActivity activity, string jsonFile, Texture2D texture)
#endif
		{
			var regions = new Dictionary<string, TextureRegion>();

			string jsonText = null;
#if WINDOWS
			jsonText = File.ReadAllText(jsonFile);
#elif ANDROID

			using (var inputStram = activity.Assets.Open(jsonFile))
			{
				jsonText = new StreamReader(inputStram).ReadToEnd();
			}
#endif
			JObject data = JObject.Parse(jsonText);

			foreach (JToken frame in data["frames"])
			{
				Rectangle textureAtlasArea;

				string filename = Path.GetFileNameWithoutExtension((string) frame["filename"]);

				bool rotated = Read(frame, "rotated", false);

				JToken details = frame["frame"];
				int x = Read(details, "x", 0);
				int y = Read(details, "y", 0);
				int width = Read(details, "w", 0);
				int height = Read(details, "h", 0);

				if (rotated)
				{
					// The image inside our texture map is rotated so swap width and height
					textureAtlasArea = new Rectangle(x, y, height, width);
				}
				else
				{
					textureAtlasArea = new Rectangle(x, y, width, height);
				}

				regions.Add(filename, new TextureRegion(texture, rotated, textureAtlasArea));
			}

			return new TextureAtlas(texture, regions);
		}
	}
}