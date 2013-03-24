/// <summary>
/// Skin.cs
/// 2013-March
/// </summary>
namespace Spine.Runtime.MonoGame
{
	using System;
	using System.Collections.Generic;

	using Spine.Runtime.MonoGame.Attachments;

	/** Stores attachments by slot index and attachment name. */
	public class Skin
	{
		internal readonly String name;
		private readonly Dictionary<Key, Attachment> attachments = new Dictionary<Key, Attachment> ();
		
		public Skin (String name)
		{
			if (name == null)
			{
				throw new ArgumentException ("name cannot be null.");
			}
			this.name = name;
		}
		
		public void addAttachment (int slotIndex, String name, Attachment attachment)
		{
			if (attachment == null)
			{
				throw new ArgumentException ("attachment cannot be null.");
			}
			Key key = new Key (slotIndex, name);
			attachments [key] = attachment;
		}
		
		/** @return May be null. */
		public Attachment getAttachment (int slotIndex, String name)
		{
			Key lookup = new Key (slotIndex, name);

			Attachment matchedAttachment;
			return attachments.TryGetValue (lookup, out matchedAttachment) ? matchedAttachment : null;
		}
		
		public void findNamesForSlot (int slotIndex, List<String> names)
		{
			if (names == null)
			{
				throw new ArgumentException ("names cannot be null.");
			}

			foreach (Key key in attachments.Keys)
			{
				if (key.slotIndex == slotIndex)
				{
					names.Add (key.name);
				}
			}
		}
		
		public void findAttachmentsForSlot (int slotIndex, List<Attachment> attachmentList)
		{
			if (attachments == null)
			{
				throw new ArgumentException ("attachments cannot be null.");
			}

			foreach (var entry in this.attachments)
			{
				if (entry.Key.slotIndex == slotIndex)
				{
					attachmentList.Add (entry.Value);
				}
			}
		}
		
		public void clear ()
		{
			attachments.Clear ();
		}
		
		public String getName ()
		{
			return name;
		}
		
		public String toString ()
		{
			return name;
		}
		
		private class Key : IEquatable<Key>
		{
			internal int slotIndex
			{
				get;
				private set;
			}

			internal String name {
				get;
				private set;
			}

			public Key(int slotIndex, String name)
			{
				if (name == null)
				{
					throw new ArgumentException ("attachmentName cannot be null.");
				}
				
				this.slotIndex = slotIndex;
				this.name = name;
			}

			public override int GetHashCode ()
			{
				return 31 * (31 + name.GetHashCode ()) + slotIndex;
			}

			public bool Equals(Key other)
			{
				return slotIndex == other.slotIndex && name.Equals (other.name);
			}

			public override bool Equals (Object obj)
			{
				if (obj == null)
				{
					return false;
				}

				Key other = (Key)obj;
				if (other == null)
				{
					return false;
				}

				return this.Equals(other);
			}
			
			public override String ToString ()
			{
				return slotIndex + ":" + name;
			}
		}
		
		/** Attach all attachments from this skin if the corresponding attachment from the old skin is currently attached. */
		internal void attachAll (Skeleton skeleton, Skin oldSkin)
		{
			foreach (var entry in attachments)
			{
				int slotIndex = entry.Key.slotIndex;
				Slot slot = skeleton.slots [slotIndex];
				if (slot.attachment == entry.Value)
				{
					Attachment attachment = getAttachment (slotIndex, entry.Key.name);
					if (attachment != null)
					{
						slot.setAttachment (attachment);
					}
				}
			}
		}
	}
}