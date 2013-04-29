using DeltaEngine.Core;
using DeltaEngine.Input;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;
using Codesmith.SmithNgine.Particles;
using Codesmith.SmithNgine.Particles.Generators;
using Codesmith.SmithNgine.Particles.Modifiers;
using Codesmith.SmithNgine.MathUtil;

namespace ParticleApp
{
    public class ParticleTester : Runner<Time, Window>
    {
        private Window window;
        private Point mousePosition;
        private PixelScreenSpace pss;
        private ParticleSystem particleSystem;
        private ParticleEffect effect;
        private ParticleEmitter emitter;
        private Renderer renderer;
        private Content content;
        public ParticleTester(Window window, InputCommands inputCommands, Content content, Renderer renderer)
        {
            this.renderer = renderer;
            this.content = content;
            this.window = window;
            SetupParticleSystem();
            particleSystem.DefaultParticleSize = Size.Half * window.ViewportPixelSize.Width / 1920f;

            window.Title = "Particle System Tester";
            window.BackgroundColor = Color.Black;
            window.ViewportSizeChanged += window_ViewportSizeChanged;
            inputCommands.AddMouseMovement(mouse => this.mousePosition = mouse.Position);
        }

        private void window_ViewportSizeChanged(Size obj)
        {
            particleSystem.DefaultParticleSize = Size.Half * obj.Width/1920f;
        }

        public void Run(Time time, Window window)
        {
            effect.Position = mousePosition;

            // Update the particle system
            particleSystem.Update(time);

            window.Title = "Particle System Tester, Particles: " + particleSystem.ParticleCount;
        }

        private void SetupParticleSystem()
        {
            particleSystem = new ParticleSystem(renderer);
            effect = new ParticleEffect();
            effect.Rotation = 0f;
            effect.Position = Point.Half;
            particleSystem.DefaultParticleSize = new Size(0.1f, 0.1f);
            // Set up one emitter (point emitter) for the effect 
            // using 3 images for particles...
//            emitter = new PointEmitter(Point.Zero);
            // Try this too
            emitter = new LineEmitter(new Point(-0.1f, 0), new Point(0.1f, 0));
            // Generate 100 particles per second
            emitter.Quantity = 100;
            emitter.AddPropertyGenerator(
                new RandomSpeedGenerator(0.1f, 0.4f, 1.0f));
            emitter.AddPropertyGenerator(
                new RandomScaleGenerator(0.0f, 0.1f, 1.0f));
            emitter.AddPropertyGenerator(
                new RandomOpacityGenerator(0.5f, 0.0f, 1.0f));
            emitter.AddPropertyGenerator(
                new RandomRotationGenerator(0.0f, MathConstants.TwoPI, 1.0f));
            emitter.AddPropertyGenerator(
                new RandomAngularVelocityGenerator(-1.0f, 1.0f, 1.0f));
            emitter.AddPropertyGenerator(
                new RandomTTLGenerator(1.0f, 2.0f, 1.0f));
            emitter.AddPropertyGenerator(
                new RandomColorGenerator(Color.White, Color.DarkGray));
            emitter.AddParticleModifier(
                new OpacityModifier1(0.0f));
            emitter.AddParticleModifier(
                new ScaleModifier1(1.0f));
            emitter.AddParticleModifier(
                new DampingLinearVelocityModifier(1.001f));
            // Automatically generate particles, quatity per second
            emitter.Flags |= EmitterModes.AutoGenerate;
            emitter.AddTexture(content.Load<Image>("smoke1"));
            emitter.AddTexture(content.Load<Image>("smoke2"));
            emitter.AddTexture(content.Load<Image>("smoke3"));
            effect.AddEmitter(emitter);
            particleSystem.AddEffect(effect);
        }

    }
}