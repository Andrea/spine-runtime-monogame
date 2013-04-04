using Microsoft.Xna.Framework;
using Spine.Runtime.MonoGame;
using Spine.Runtime.MonoGame.Utils;

namespace Spine.Runtime.Monogame.Animation
{
	public class ColorTimeline : CurveTimeline, ITimeline
	{
		private const int LAST_FRAME_TIME = -5;
		private const int FRAME_R = 1;
		private const int FRAME_G = 2;
		private const int FRAME_B = 3;
		private const int FRAME_A = 4;
		private readonly float[] frames; // time, r, g, b, a, ...
		private int slotIndex = -1;

		public ColorTimeline(int keyframeCount) : base(keyframeCount)
		{
			frames = new float[keyframeCount*5];
		}

		#region ITimeline Members

		public float GetDuration()
		{
			return frames[frames.Length - 5];
		}

		public int GetKeyframeCount()
		{
			return frames.Length/5;
		}

		public void Apply(Skeleton skeleton, float time, float alpha)
		{
			if (time < frames[0])
			{
				return;
			} // Time is before first frame.

			Color color = skeleton.Slots[slotIndex].Color;

			if (time >= frames[frames.Length - 5])
			{
				// Time is after last frame.
				int i = frames.Length - 1;
				float colorRed = frames[i - 3];
				float colorGreen = frames[i - 2];
				float colorBlue = frames[i - 1];
				float colorAlpha = frames[i];
				color = new Color(colorRed, colorGreen, colorBlue, colorAlpha);
				return;
			}

			// Interpolate between the last frame and the current frame.
			int frameIndex = SearchUtils.BinarySearch(frames, time, 5);
			float lastFrameR = frames[frameIndex - 4];
			float lastFrameG = frames[frameIndex - 3];
			float lastFrameB = frames[frameIndex - 2];
			float lastFrameA = frames[frameIndex - 1];
			float frameTime = frames[frameIndex];
			float percent = MathUtils.Clamp(1 - (time - frameTime)/(frames[frameIndex + LAST_FRAME_TIME] - frameTime), 0, 1);
			percent = GetCurvePercent(frameIndex/5 - 1, percent);

			float r = lastFrameR + (frames[frameIndex + FRAME_R] - lastFrameR)*percent;
			float g = lastFrameG + (frames[frameIndex + FRAME_G] - lastFrameG)*percent;
			float b = lastFrameB + (frames[frameIndex + FRAME_B] - lastFrameB)*percent;
			float a = lastFrameA + (frames[frameIndex + FRAME_A] - lastFrameA)*percent;

			if (alpha < 1)
			{
				color = new Color(
					MathUtils.Clamp(color.R + ((r - color.R)*alpha), 0, 1),
					MathUtils.Clamp(color.G + ((g - color.G)*alpha), 0, 1),
					MathUtils.Clamp(color.B + ((b - color.B)*alpha), 0, 1),
					MathUtils.Clamp(color.A + ((a - color.A)*alpha), 0, 1));
			}
			else
			{
				color = new Color(r, g, b, a);
			}
		}

		#endregion

		public void SetSlotIndex(int index)
		{
			slotIndex = index;
		}

		/** Sets the time and value of the specified keyframe. */

		public void SetKeyframe(int keyframeIndex, float time, float r, float g, float b, float a)
		{
			keyframeIndex *= 5;
			frames[keyframeIndex] = time;
			frames[keyframeIndex + 1] = r;
			frames[keyframeIndex + 2] = g;
			frames[keyframeIndex + 3] = b;
			frames[keyframeIndex + 4] = a;
		}
	}
}