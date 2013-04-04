namespace Spine.Runtime.MonoGame.Attachments
{
	using Microsoft.Xna.Framework.Graphics;
	using System;

	public abstract class Attachment
	{
		protected Attachment (String name)
		{
			if (name == null)
				throw new ArgumentException("name cannot be null.");

			Name = name;
		}
	
		public String Name { get; private set; }

		public abstract void Draw (SpriteBatch batch, Slot slot, bool flipX, bool flipY);
	}
}

