using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Spine.Runtime.MonoGame;
using Spine.Runtime.MonoGame.Attachments;
using Spine.Runtime.MonoGame.Graphics;
using Spine.Runtime.MonoGame.Json;
using Spine.Runtime.Monogame.Animation;
#if iOS
	using Microsoft.Xna.Framework.Input.Touch;
#endif
namespace Demo
{
	public class AnimationDemo : Game
	{
		private readonly GraphicsDeviceManager _graphicsDeviceManager;
		private int _animation;
		private Animation _animationJump;
		private Animation _animationWalk;
		private Texture2D _lineTexture;
		private Skeleton _skeleton;
		private SpriteBatch _spriteBatch;
		private List<Texture2D> _textureMaps;
		private float _timer = 1;

		public AnimationDemo()
		{
			_graphicsDeviceManager = new GraphicsDeviceManager(this);
#if !WINDOWS
			graphicsDeviceManager.IsFullScreen = true;
			graphicsDeviceManager.SupportedOrientations = 
				DisplayOrientation.LandscapeLeft |
				DisplayOrientation.LandscapeRight |
				DisplayOrientation.Portrait;
#endif
			_textureMaps = new List<Texture2D>();

			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
#if iOS
			TouchPanel.EnabledGestures = GestureType.Tap;
#endif
			base.Initialize();
		}
		
		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(_graphicsDeviceManager.GraphicsDevice);
#if WINDOWS
			var crabTextureMap = Content.Load<Texture2D>("crab");
#else
			var crabTextureMap = Content.Load<Texture2D> ("crab.png");
#endif
			_textureMaps.Add(crabTextureMap);

			var texturePackerReader = new TextureMapJsonReader();
#if WINDOWS
			TextureAtlas textureAtlas = texturePackerReader.ReadTextureJsonFile("Content/crab.json", crabTextureMap);
#elif ANDROID
			var textureAtlas = texturePackerReader.ReadTextureJsonFile (Activity,"Content/crab.json", crabTextureMap);
#endif
			var skeletonReader = new SkeletonJsonReader(new TextureAtlasAttachmentLoader(textureAtlas));
#if WINDOWS
			_skeleton = skeletonReader.ReadSkeletonJsonFile("Content/crab-skeleton.json");
#elif ANDROID

			this.skeleton = skeletonReader.ReadSkeletonJsonFile (Activity,"Content/crab-skeleton.json");
#endif
			_skeleton.FlipY = true;

			var animationReader = new AnimationJsonReader();
#if WINDOWS
			_animationWalk = animationReader.ReadAnimationJsonFile("Content/crab-WalkLeft.json", _skeleton.Data);
			_animationJump = animationReader.ReadAnimationJsonFile("Content/crab-Jump.json", _skeleton.Data);
#elif ANDROID

			this.animationWalk = animationReader.ReadAnimationJsonFile ( Activity,"Content/crab-WalkLeft.json", skeleton.Data);
			this.animationJump = animationReader.ReadAnimationJsonFile (Activity,"Content/crab-Jump.json", skeleton.Data);
#endif
			_animation = 0;
			SetSkeletonStartPosition();

			// used for debugging - draw the bones
			_lineTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			_lineTexture.SetData(new[] {Color.White});
			_textureMaps.Add(_lineTexture);

			base.LoadContent();
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			
			_spriteBatch.Begin();
			_skeleton.Draw(_spriteBatch);
			_spriteBatch.End();

			base.Draw(gameTime);
		}

		protected override void Update(GameTime gameTime)
		{
			if (++_animation > 1)
			{
				_animation = 0;
			}

			SetSkeletonStartPosition();

			Animate(_animationJump);
			base.Update(gameTime);
		}

		protected override void Dispose(bool disposing)
		{
			if (_textureMaps != null)
			{
				foreach (var textureMap in _textureMaps)
				{
					if (textureMap != null && !textureMap.IsDisposed)
					{
						textureMap.Dispose();
					}
				}
				_textureMaps = null;
			}

			base.Dispose(disposing);
		}

		private void SetSkeletonStartPosition()
		{
			_skeleton.SetToBindPose();
			Bone root = _skeleton.GetRootBone();
			root.X = 200;
			root.Y = 350;

			_skeleton.UpdateWorldTransform();
		}

		private void Animate(Animation animation)
		{
			// In reality you'd use the gameTime to calculate the animation but this is for demonstration purposes.
			// Also you'd probably do the calculations in Update and not Draw
			animation.Apply(_skeleton, _timer++/100, true);
			_skeleton.UpdateWorldTransform();
		}
	}
}