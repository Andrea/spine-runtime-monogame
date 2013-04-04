/// <summary>
/// Skeleton.cs
/// 2013-March
/// </summary>

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine.Runtime.MonoGame.Attachments;

namespace Spine.Runtime.MonoGame
{
	public class Skeleton
	{
		internal readonly List<Bone> Bones;
		internal readonly Color Color;
		internal readonly List<Slot> DrawOrder;
		internal readonly List<Slot> Slots;
		internal Skin Skin;
		internal float Time;

		public Skeleton(SkeletonData data)
		{
			if (data == null)
			{
				throw new ArgumentException("data cannot be null.");
			}
			Data = data;

			Bones = new List<Bone>(data.Bones.Count);
			foreach (BoneData boneData in data.Bones)
			{
				Bone parent = boneData.Parent == null ? null : Bones[data.Bones.IndexOf(boneData.Parent)];
				Bones.Add(new Bone(boneData, parent));
			}

			Slots = new List<Slot>(data.Slots.Count);
			DrawOrder = new List<Slot>(data.Slots.Count);
			foreach (SlotData slotData in data.Slots)
			{
				Bone bone = Bones[data.Bones.IndexOf(slotData.BoneData)];
				var slot = new Slot(slotData, this, bone);
				Slots.Add(slot);
				DrawOrder.Add(slot);
			}

			Color = new Color(1, 1, 1, 1);
		}

		/** Copy constructor. */

		public Skeleton(Skeleton skeleton)
		{
			if (skeleton == null)
			{
				throw new ArgumentException("skeleton cannot be null.");
			}
			Data = skeleton.Data;

			Bones = new List<Bone>(skeleton.Bones.Count);
			foreach (Bone bone in skeleton.Bones)
			{
				Bone parent = Bones[skeleton.Bones.IndexOf(bone.Parent)];
				Bones.Add(new Bone(bone, parent));
			}

			Slots = new List<Slot>(skeleton.Slots.Count);
			foreach (Slot slot in skeleton.Slots)
			{
				Bone bone = Bones[skeleton.Bones.IndexOf(slot.Bone)];
				var newSlot = new Slot(slot, this, bone);
				Slots.Add(newSlot);
			}

			DrawOrder = new List<Slot>(Slots.Count);
			foreach (Slot slot in skeleton.DrawOrder)
				DrawOrder.Add(Slots[skeleton.Slots.IndexOf(slot)]);

			Skin = skeleton.Skin;
			Color = skeleton.Color;
			Time = skeleton.Time;
		}

		public SkeletonData Data { get; private set; }

		public bool FlipX { get; set; }

		public bool FlipY { get; set; }

		/** Updates the world transform for each bone. */

		public void UpdateWorldTransform()
		{
			foreach (Bone bone in Bones)
			{
				bone.UpdateWorldTransform(FlipX, FlipY);
			}
		}

		/** Sets the bones and slots to their bind pose values. */

		public void SetToBindPose()
		{
			SetBonesToBindPose();
			SetSlotsToBindPose();
		}

		public void SetBonesToBindPose()
		{
			foreach (Bone bone in Bones)
			{
				bone.SetToBindPose();
			}
		}

		public void SetSlotsToBindPose()
		{
			foreach (Slot slot in Slots)
			{
				slot.SetToBindPose();
			}
		}

		public void Draw(SpriteBatch batch)
		{
			foreach (Slot drawOrderItem in DrawOrder)
			{
				if (drawOrderItem.Attachment != null)
				{
					drawOrderItem.Attachment.Draw(batch, drawOrderItem, FlipX, FlipY);
				}
			}
		}


		public void DrawDebug(SpriteBatch batch, Texture2D lineTexture)
		{
			foreach (Bone bone in Bones)
			{
				if (bone.Parent == null)
				{
					continue;
				}

				var point1 = new Vector2(bone.worldX, bone.worldY);
				var point2 = new Vector2(bone.Data.Length*bone.m00 + bone.worldX, bone.Data.Length*bone.m10 + bone.worldY);
				var angle = (float) Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
				float length = Vector2.Distance(point1, point2);

				batch.Draw(lineTexture, point1, null, Color.Red, angle, Vector2.Zero, new Vector2(length, 5), SpriteEffects.None, 0);
			}
		}

		/** @return May return null. */

		public Bone GetRootBone()
		{
			if (Bones.Count == 0)
			{
				return null;
			}

			return Bones[0];
		}

		public Slot FindSlot(string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentException("slotname cannot be null");
			}

			return Slots.Find(x => x.Data.Name.Equals(slotName));
		}

		/** Sets a skin by name.
	 * @see #setSkin(Skin) */

		public void SetSkin(String skinName)
		{
			Skin skin = Data.FindSkin(skinName);
			if (skin == null)
			{
				throw new ArgumentException("Skin not found: " + skinName);
			}

			SetSkin(skin);
		}

		/** Sets the skin used to look up attachments not found in the {@link SkeletonData#getDefaultSkin() default skin}. Attachments
	 * from the new skin are attached if the corresponding attachment from the old skin is currently attached.
	 * @param newSkin May be null. */

		public void SetSkin(Skin newSkin)
		{
			if (Skin != null && newSkin != null)
			{
				newSkin.AttachAll(this, Skin);
			}

			Skin = newSkin;
		}

		/** @return May be null. */

		public Attachment GetAttachment(String slotName, String attachmentName)
		{
			return GetAttachment(Data.FindSlotIndex(slotName), attachmentName);
		}

		/** @return May be null. */

		public Attachment GetAttachment(int slotIndex, String attachmentName)
		{
			if (attachmentName == null)
			{
				throw new ArgumentException("attachmentName cannot be null.");
			}
			if (Skin != null)
			{
				return Skin.GetAttachment(slotIndex, attachmentName);
			}
			if (Data.DefaultSkin != null)
			{
				Attachment attachment = Data.DefaultSkin.GetAttachment(slotIndex, attachmentName);
				if (attachment != null)
				{
					return attachment;
				}
			}
			return null;
		}

		/** @param attachmentName May be null. */

		public void SetAttachment(String slotName, String attachmentName)
		{
			if (slotName == null)
			{
				throw new ArgumentException("slotName cannot be null.");
			}

			int index = 0;
			foreach (Slot slot in Slots)
			{
				if (slot.Data.Name.Equals(slotName))
				{
					Attachment attachment = null;
					if (attachmentName != null)
					{
						attachment = GetAttachment(index, attachmentName);
						if (attachment == null)
						{
							throw new ArgumentException("Attachment not found: " + attachmentName + ", for slot: " + slotName);
						}
					}
					slot.SetAttachment(attachment);
					return;
				}

				index++;
			}
			throw new ArgumentException("Slot not found: " + slotName);
		}

		public void Update(float delta)
		{
			Time += delta;
		}
	}
}