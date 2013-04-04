namespace Demo.Spine.WidowsXNA
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (var game = new AnimationDemo())
            {
                game.Run();
            }
        }
    }
#endif
}

