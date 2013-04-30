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
        private ParticleSystem particleSystem;
        private ParticleEffect fireEffect;
        private ParticleEffect waterfall;
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
            fireEffect.Position = new Point(0.5f, 0.8f);
            waterfall.Position = mousePosition;
            // Update the particle system
            particleSystem.Update(time);
            particleSystem.Draw(time);

            window.Title = "Particle System Tester, Particles: " + particleSystem.ParticleCount + 
                " : Renderables: " + renderer.NumberOfActiveRenderableObjects + 
                " Fps: " + time.Fps;
        }

        private void SetupParticleSystem()
        {
            particleSystem = new ParticleSystem(renderer);
            particleSystem.Screen = renderer.Screen;
//            particleSystem.EnableCache(5000);
            fireEffect = new ParticleEffect();
            fireEffect.Rotation = 0f;
            fireEffect.Position = Point.Half;
            particleSystem.DefaultParticleSize = new Size(0.1f, 0.1f);
            // Set up one emitter (point emitter) for the effect 
            // using 3 images for particles...
//            emitter = new PointEmitter(Point.Zero);
            // Try this too
            ParticleEmitter emitter1 = new LineEmitter(new Point(-0.1f, 0), new Point(0.1f, 0));
            emitter1.Quantity = 100;
            emitter1.AddPropertyGenerator(new RandomDepthGenerator(0f, 0.2f, 1.0f));
            emitter1.AddPropertyGenerator(new RandomSpeedGenerator(0.2f, 0.4f, 1.0f));
            emitter1.AddPropertyGenerator(new ConstantScaleGenerator(0.0f));
            emitter1.AddPropertyGenerator(new RandomOpacityGenerator(0.5f, 0.0f, 1.0f));
            emitter1.AddPropertyGenerator(new RandomRotationGenerator(0.0f, MathConstants.TwoPI, 1.0f));
            emitter1.AddPropertyGenerator(new RandomAngularVelocityGenerator(-1.0f, 1.0f, 1.0f));
            emitter1.AddPropertyGenerator(new RandomTTLGenerator(1.0f, 2.0f, 1.0f));
            emitter1.AddPropertyGenerator(new RandomColorGenerator(Color.White, Color.DarkGray));
            emitter1.AddParticleModifier(new OpacityModifier1(0.0f));
            emitter1.AddParticleModifier(new ScaleModifier1(2.0f));
            emitter1.AddParticleModifier(new DampingLinearVelocityModifier(1.001f));

            // Automatically generate particles, quatity per second
            emitter1.Flags |= EmitterModes.AutoGenerate;
            emitter1.AddTexture(content.Load<Image>("smoke1"));
//            emitter1.AddTexture(content.Load<Image>("smoke2"));
//            emitter1.AddTexture(content.Load<Image>("smoke3"));
            fireEffect.AddEmitter(emitter1);

            ParticleEmitter emitter2 = new LineEmitter(new Point(-0.1f, 0), new Point(0.1f, 0));
            emitter2.Quantity = 200;
//            emitter2.AddPropertyGenerator(new ConstantDepthGenerator(0.1f));
            emitter2.AddPropertyGenerator(new RandomDepthGenerator(0f, 0.2f, 1.0f));
            emitter2.AddPropertyGenerator(new RandomSpeedGenerator(0.05f, 0.05f, 1.0f));
            emitter2.AddPropertyGenerator(new RandomScaleGenerator(0.0f, 0.1f, 1.0f));
            emitter2.AddPropertyGenerator(new RandomOpacityGenerator(0.5f, 0.0f, 1.0f));
            emitter2.AddPropertyGenerator(new RandomRotationGenerator(0.0f, MathConstants.TwoPI, 1.0f));
            emitter2.AddPropertyGenerator(new RandomAngularVelocityGenerator(-1.0f, 1.0f, 1.0f));
            emitter2.AddPropertyGenerator(new RandomTTLGenerator(1.0f, 2.0f, 1.0f));
            emitter2.AddPropertyGenerator(new RandomColorGenerator(Color.Red, Color.Yellow));
            emitter2.AddParticleModifier(new OpacityModifier1(0.0f));
            emitter2.AddParticleModifier(new ScaleModifier1(1.0f));
            // Automatically generate particles, quatity per second
            emitter2.Flags |= EmitterModes.AutoGenerate;
            emitter2.AddTexture(content.Load<Image>("smoke1"));
            emitter1.AddTexture(content.Load<Image>("smoke2"));
            emitter2.AddTexture(content.Load<Image>("smoke3"));
            fireEffect.AddEmitter(emitter2);
//            particleSystem.AddEffect(fireEffect);

            SetupWaterFall(particleSystem);
        }

        private void SetupWaterFall(ParticleSystem particleSystem)
        {
            waterfall = new ParticleEffect();
            waterfall.Position = new Point(0.7f, 0.5f);
            waterfall.Rotation = 0f;

            ParticleEmitter emitter1 = new LineEmitter(new Point(-0.1f, 0), new Point(0.1f, 0));
            emitter1.Quantity = 2000;
            emitter1.AddPropertyGenerator(new RandomDepthGenerator(0f, 0.2f, 1.0f));
            emitter1.AddPropertyGenerator(new RandomSpeedGenerator(0.2f, 0.4f, 1.0f));
            emitter1.AddPropertyGenerator(new ConstantScaleGenerator(0.0f));
            emitter1.AddPropertyGenerator(new RandomOpacityGenerator(0.5f, 0.0f, 1.0f));
            emitter1.AddPropertyGenerator(new RandomRotationGenerator(0.0f, MathConstants.TwoPI, 1.0f));
            emitter1.AddPropertyGenerator(new RandomAngularVelocityGenerator(-1.0f, 1.0f, 1.0f));
            emitter1.AddPropertyGenerator(new RandomTTLGenerator(1.0f, 2.0f, 1.0f));
            emitter1.AddPropertyGenerator(new RandomColorGenerator(Color.Blue, Color.LightBlue));
            emitter1.AddParticleModifier(new OpacityModifier1(0.0f));
            emitter1.AddParticleModifier(new ScaleModifier1(1.0f));
            emitter1.AddParticleModifier(new LinearVelocityModifier(new Point(0f,0.98f)));

            // Automatically generate particles, quatity per second
            emitter1.Flags |= EmitterModes.AutoGenerate;
            emitter1.AddTexture(content.Load<Image>("smoke1"));
            emitter1.AddTexture(content.Load<Image>("smoke3"));
            waterfall.AddEmitter(emitter1);
            particleSystem.AddEffect(waterfall);
        }
    }
}