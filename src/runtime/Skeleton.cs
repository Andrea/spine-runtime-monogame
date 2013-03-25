/// <summary>
/// Skeleton.cs
/// 2013-March
/// </summary>
namespace Spine.Runtime.MonoGame
{
	using System;
	using System.Collections.Generic;

	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Graphics;

	using Spine.Runtime.MonoGame.Attachments;

	public class Skeleton
	{
		internal readonly List<Bone> bones;
		internal readonly List<Slot> slots;
		internal readonly List<Slot> drawOrder;
		internal Skin skin;
		internal readonly Color color;
		internal float time;
		internal bool flipX, flipY;
		
		public Skeleton (SkeletonData data)
		{
			if (data == null)
			{
				throw new ArgumentException ("data cannot be null.");
			}
			this.Data = data;

			bones = new List<Bone> (data.Bones.Count);
			foreach (BoneData boneData in data.Bones)
			{
				Bone parent = boneData.Parent == null ? null : bones [data.Bones.IndexOf (boneData.Parent)];
				bones.Add (new Bone (boneData, parent));
			}
			
			slots = new List<Slot> (data.Slots.Count);
			drawOrder = new List<Slot> (data.Slots.Count);
			foreach (SlotData slotData in data.Slots)
			{
				Bone bone = bones [data.Bones.IndexOf (slotData.BoneData)];
				Slot slot = new Slot (slotData, this, bone);
				slots.Add (slot);
				drawOrder.Add (slot);
			}
			
			color = new Color (1, 1, 1, 1);
		}
		
		/** Copy constructor. */
		public Skeleton (Skeleton skeleton)
		{
			if (skeleton == null)
			{
				throw new ArgumentException ("skeleton cannot be null.");
			}
			Data = skeleton.Data;
			
			bones = new List<Bone> (skeleton.bones.Count);
			foreach (Bone bone in skeleton.bones)
			{
				Bone parent = bones [skeleton.bones.IndexOf (bone.Parent)];
				bones.Add (new Bone (bone, parent));
			}
			
			slots = new List<Slot> (skeleton.slots.Count);
			foreach (Slot slot in skeleton.slots)
			{
				Bone bone = bones [skeleton.bones.IndexOf (slot.Bone)];
				Slot newSlot = new Slot (slot, this, bone);
				slots.Add (newSlot);
			}
			
			drawOrder = new List<Slot> (slots.Count);
			foreach (Slot slot in skeleton.drawOrder)
				drawOrder.Add (slots [skeleton.slots.IndexOf (slot)]);
			
			skin = skeleton.skin;
			color = skeleton.color;
			time = skeleton.time;
		}

		public SkeletonData Data
		{
			get;
			private set;
		}

		
		/** Updates the world transform for each bone. */
		public void UpdateWorldTransform ()
		{
			foreach (var bone in this.bones)
			{
				bone.UpdateWorldTransform (this.flipX, this.flipY);
			}
		}
		
		/** Sets the bones and slots to their bind pose values. */
		public void SetToBindPose ()
		{
			this.SetBonesToBindPose ();
			this.SetSlotsToBindPose ();
		}
		
		public void SetBonesToBindPose ()
		{
			foreach (var bone in this.bones)
			{
				bone.SetToBindPose ();
			}
		}
		
		public void SetSlotsToBindPose ()
		{
			foreach (var slot in this.slots)
			{
				slot.SetToBindPose ();
			}
		}
		
		public void Draw (SpriteBatch batch)
		{
			foreach (var drawOrderItem in this.drawOrder)
			{
				if (drawOrderItem.Attachment != null)
				{
					drawOrderItem.Attachment.Draw (batch, drawOrderItem);
				}
			}
		}

		/*
		public void drawDebug (ShapeRenderer renderer) {
			renderer.setColor(Color.RED);
			renderer.begin(ShapeType.Line);
			for (int i = 0, n = bones.Count; i < n; i++) {
				Bone bone = bones.get(i);
				if (bone.parent == null) continue;
				float x = bone.data.length * bone.m00 + bone.worldX;
				float y = bone.data.length * bone.m10 + bone.worldY;
				renderer.line(bone.worldX, bone.worldY, x, y);
			}
			renderer.end();
			
			renderer.setColor(Color.GREEN);
			renderer.begin(ShapeType.Filled);
			for (int i = 0, n = bones.Count; i < n; i++) {
				Bone bone = bones.get(i);
				renderer.setColor(Color.GREEN);
				renderer.circle(bone.worldX, bone.worldY, 3);
			}
			renderer.end();
		}*/
		
		/** @return May return null. */
		public Bone GetRootBone ()
		{
			if (bones.Count == 0)
			{
				return null;
			}

			return bones [0];
		}
				
		public Slot FindSlot (string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentException ("slotname cannot be null");
			}

			return this.slots.Find(x => x.Data.Name.Equals(slotName));
		}

		/** Sets a skin by name.
	 * @see #setSkin(Skin) */
		public void SetSkin (String skinName)
		{
			Skin skin = Data.FindSkin (skinName);
			if (skin == null)
			{
				throw new ArgumentException ("Skin not found: " + skinName);
			}

			this.SetSkin (skin);
		}
		
		/** Sets the skin used to look up attachments not found in the {@link SkeletonData#getDefaultSkin() default skin}. Attachments
	 * from the new skin are attached if the corresponding attachment from the old skin is currently attached.
	 * @param newSkin May be null. */
		public void SetSkin (Skin newSkin)
		{
			if (this.skin != null && newSkin != null)
			{
				newSkin.AttachAll (this, this.skin);
			}

			this.skin = newSkin;
		}
		
		/** @return May be null. */
		public Attachment GetAttachment (String slotName, String attachmentName)
		{
			return GetAttachment (Data.FindSlotIndex (slotName), attachmentName);
		}
		
		/** @return May be null. */
		public Attachment GetAttachment (int slotIndex, String attachmentName)
		{
			if (attachmentName == null)
			{
				throw new ArgumentException ("attachmentName cannot be null.");
			}
			if (skin != null)
			{
				return skin.GetAttachment (slotIndex, attachmentName);
			}
			if (Data.DefaultSkin != null)
			{
				Attachment attachment = Data.DefaultSkin.GetAttachment (slotIndex, attachmentName);
				if (attachment != null)
				{
					return attachment;
				}
			}
			return null;
		}
		
		/** @param attachmentName May be null. */
		public void SetAttachment (String slotName, String attachmentName)
		{
			if (slotName == null)
			{
				throw new ArgumentException ("slotName cannot be null.");
			}

			int index = 0;
			foreach (var slot in this.slots)
			{
				if (slot.Data.Name.Equals (slotName))
				{
					Attachment attachment = null;
					if (attachmentName != null)
					{
						attachment = GetAttachment (index, attachmentName);
						if (attachment == null)
						{
							throw new ArgumentException ("Attachment not found: " + attachmentName + ", for slot: " + slotName);
						}
					}
					slot.SetAttachment (attachment);
					return;
				}

				index++;
			}
			throw new ArgumentException ("Slot not found: " + slotName);
		}
		
		public void Update (float delta)
		{
			time += delta;
		}
	}
}