/// <summary>
/// RegionSequenceAttachment.cs
/// 2013-March
/// </summary>
namespace Spine.Runtime.MonoGame.Attachments
{
	using System;
	using Microsoft.Xna.Framework.Graphics;

	// TODO
	internal class RegionSequenceAttachment : Attachment
	{
		internal RegionSequenceAttachment (String name) : base(name) {
		}

		public override void draw (SpriteBatch batch, Slot slot)
		{}

		internal void setFrameTime(float fps)
		{
		}

		internal void setMode(RegionSequenceMode mode)
		{
		}
	}
}

