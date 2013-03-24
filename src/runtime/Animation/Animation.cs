/// <summary>
/// Animation.cs
/// 2013-March
/// </summary>
namespace Spine.Runtime.MonoGame
{
	using System;
	using System.Collections.Generic;

	public class Animation
	{
		private String name;
		private readonly List<ITimeline> timelines;
		private float duration;
		
		public Animation (List<ITimeline> timelines, float duration)
		{
			if (timelines == null)
			{
				throw new ArgumentException ("timelines cannot be null.");
			}
			this.timelines = timelines;
			this.duration = duration;
		}
		
		public List<ITimeline> getTimelines ()
		{
			return timelines;
		}
		
		/** Returns the duration of the animation in seconds. Defaults to the max {@link Timeline#getDuration() duration} of the
	 * timelines. */
		public float getDuration ()
		{
			return duration;
		}
		
		public void setDuration (float duration)
		{
			this.duration = duration;
		}
		
		/** Poses the skeleton at the specified time for this animation. */
		public void apply (Skeleton skeleton, float time, bool loop)
		{
			if (skeleton == null)
			{
				throw new ArgumentException ("skeleton cannot be null.");
			}
			
			if (loop && duration != 0)
			{
				time %= duration;
			}

			foreach (var timeline in this.timelines)
			{
				timeline.apply (skeleton, time, 1);
			}
		}
		
		/** Poses the skeleton at the specified time for this animation mixed with the current pose.
	 * @param alpha The amount of this animation that affects the current pose. */
		public void mix (Skeleton skeleton, float time, bool loop, float alpha)
		{
			if (skeleton == null)
			{
				throw new ArgumentException ("skeleton cannot be null.");
			}

			if (loop && duration != 0)
			{
				time %= duration;
			}
			
			foreach (var timeline in this.timelines)
			{
				timeline.apply (skeleton, time, alpha);
			}
		}

		
		/** @return May be null. */
		public String getName () {
			return name;
		}
		
		/** @param name May be null. */
		public void setName (String name) {
			this.name = name;
		}
		
		public String toString () {
			return name != null ? name : base.ToString();
		}
	}
}
