using DeltaEngine.Platforms;

namespace ParticleApp
{
    /// <summary>
    /// Just starts the Game class. For more complex examples see the other sample games.
    /// </summary>
    internal static class Program
    {
        public static void Main()
        {
            var app = new App();
            app.Start<ParticleTester>();
        }
    }
}