using System;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Spine.Runtime.MonoGame.Attachments;
using Spine.Runtime.MonoGame.Graphics;

namespace Spine.Runtime.MonoGame.Json
{
	public class SkeletonJsonReader : BaseJsonReader
	{
		private readonly IAttachmentLoader attachmentLoader;

		public SkeletonJsonReader(TextureAtlas atlas)
		{
			attachmentLoader = new TextureAtlasAttachmentLoader(atlas);
		}

		public SkeletonJsonReader(IAttachmentLoader attachmentLoader)
		{
			this.attachmentLoader = attachmentLoader;
		}

		public Skeleton ReadSkeletonJsonFile(string jsonFile, float scale = 1)
		{
			string jsonText = File.ReadAllText(jsonFile);
			SkeletonData skeletonData = CreateSkeletonData(jsonFile, scale, jsonText);

			return new Skeleton(skeletonData);
		}

#if ANDROID
		public Skeleton ReadSkeletonJsonFile(AndroidGameActivity activity, string jsonFile, float scale = 1)
		{
			string jsonText = null;
					
			using (var inputStram = activity.Assets.Open(jsonFile))
			{
				jsonText = new StreamReader(inputStram).ReadToEnd();
			}

			
			var skeletonData = CreateSkeletonData(jsonFile, scale, jsonText);

			return new Skeleton (skeletonData);
		}

#endif

		private SkeletonData CreateSkeletonData(string jsonFile, float scale, string jsonText)
		{
			JObject data = JObject.Parse(jsonText);
			var skeletonData = new SkeletonData(Path.GetFileNameWithoutExtension(jsonFile));

			ReadSkeletonBones(skeletonData, data, scale);
			ReadSkeletonSlots(skeletonData, data);
			ReadSkeletonSkins(skeletonData, data, scale);

			skeletonData.Bones.TrimExcess();
			skeletonData.Slots.TrimExcess();
			skeletonData.Skins.TrimExcess();
			return skeletonData;
		}

		private void ReadSkeletonSkins(SkeletonData skeletonData, JObject data, float scale)
		{
			foreach (JProperty skin in data["skins"])
			{
				var skeletonSkin = new Skin(skin.Name);

				foreach (JProperty slot in skin.Value)
				{
					int slotIndex = skeletonData.FindSlotIndex(slot.Name);

					foreach (JProperty attachment in slot.Value)
					{
						Attachment skeletonAttachment = ReadSkeletonAttachment(attachment.Name, attachment.Value, scale);
						if (attachment != null)
						{
							skeletonSkin.AddAttachment(slotIndex, attachment.Name, skeletonAttachment);
						}
					}
				}

				skeletonData.AddSkin(skeletonSkin);
				if (skeletonSkin.Name.Equals("default"))
				{
					skeletonData.DefaultSkin = skeletonSkin;
				}
			}
		}

		private Attachment ReadSkeletonAttachment(String name, JToken map, float scale)
		{
			string attachmentName = Read(map, "name", name);

			var attachmentType = AttachmentType.Region;

			var attachmentRegionType = (String) map["type"];
			if (attachmentRegionType != null && String.Compare(attachmentRegionType, "regionSequence", true) == 0)
			{
				attachmentType = AttachmentType.RegionSequence;
			}

			Attachment attachment = attachmentLoader.NewAttachment(attachmentType, attachmentName);

			if (attachment is RegionSequenceAttachment)
			{
				var regionSequenceAttachment = (RegionSequenceAttachment) attachment;

				JToken fps = map["fps"];
				if (fps == null)
				{
					throw new SerializationException("Region sequence attachment missing fps: " + attachmentName);
				}

				regionSequenceAttachment.SetFrameTime((float) fps);

				regionSequenceAttachment.SetMode(RegionSequenceModeConvert.FromString((String) map["mode"]));
			}

			if (attachment is RegionAttachment)
			{
				var regionAttachment = (RegionAttachment) attachment;
				regionAttachment.X = Read<float>(map, "x", 0)*scale;
				regionAttachment.Y = Read<float>(map, "y", 0)*scale;
				regionAttachment.ScaleX = Read<float>(map, "scaleX", 1);
				regionAttachment.ScaleY = Read<float>(map, "scaleY", 1);
				regionAttachment.Rotation = Read<float>(map, "rotation", 0);
				regionAttachment.Width = Read<float>(map, "width", 32)*scale;
				regionAttachment.Height = Read<float>(map, "height", 32)*scale;
			}

			return attachment;
		}

		private void ReadSkeletonSlots(SkeletonData skeletonData, JObject data)
		{
			foreach (JToken slot in data["slots"].Children())
			{
				var slotName = (String) slot["name"];
				var boneName = (String) slot["bone"];
				BoneData boneData = skeletonData.FindBone(boneName);
				if (boneData == null)
				{
					throw new SerializationException("Slot bone not found: " + boneName);
				}

				var slotData = new SlotData(slotName, boneData);

				var color = (String) slot["color"];

				if (color != null)
				{
					slotData.Color = ReadColor(color);
				}

				slotData.AttachmentName = (String) slot["attachment"];

				skeletonData.AddSlot(slotData);
			}
		}

		private void ReadSkeletonBones(SkeletonData skeletonData, JObject data, float scale)
		{
			foreach (JToken bone in data["bones"].Children())
			{
				BoneData parent = null;
				var parentName = (string) bone["parent"];

				if (parentName != null)
				{
					parent = skeletonData.FindBone(parentName);
					if (parent == null)
					{
						throw new SerializationException("Parent bone not found: " + parentName);
					}
				}

				var boneData = new BoneData((String) bone["name"], parent);
				boneData.Length = Read<float>(bone, "length", 0)*scale;
				boneData.X = Read<float>(bone, "x", 0)*scale;
				boneData.Y = Read<float>(bone, "y", 0)*scale;
				boneData.Rotation = Read<float>(bone, "rotation", 0);
				boneData.ScaleX = Read<float>(bone, "scaleX", 1);
				boneData.ScaleY = Read<float>(bone, "scaleY", 1);
				skeletonData.AddBone(boneData);
			}
		}
	}
}