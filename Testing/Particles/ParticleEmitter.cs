/**
 * SmithNgine Game Framework
 * 
 * Copyright (C) 2013 by Erno Pakarinen / Codesmith (www.codesmith.fi)
 * All Rights Reserved
 * 
 * For licensing terms, see License.txt which reflects to the current license
 * of this framework.
 */
namespace Codesmith.SmithNgine.Particles
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Codesmith.SmithNgine.General;
    using Codesmith.SmithNgine.MathUtil;
    using Codesmith.SmithNgine.Particles.Generators;
    using Codesmith.SmithNgine.Particles.Modifiers;
    using System.Diagnostics;
    using DeltaEngine.Graphics;
    using DeltaEngine.Datatypes;
    using DeltaEngine.Core;
    using DeltaEngine.Rendering;
    #endregion

    #region Enumerations
    /// <summary>
    /// Enumeration flags for Emitter modes
    /// </summary>
    [Flags]
    public enum EmitterModes : int
    {
        None = 0,
        // Emitter spawns particles from random position, e.g. CircleEmitter
        RandomPosition = 1,
        // Movement direction of new particles is random
        RandomDirection = 1 << 1,
        // Emitter position is absolute instead of relative to the effect position
        PositionAbsolute = 1 << 2,
        // Emitter position is relative to the effect position
        PositionRelative = 1 << 3,
        // Emitter rotation is absolute instead of relative to the effect rotation
        RotationAbsolute = 1 << 4,
        // Emitter rotation is relative to the effect rotation
        RotationRelative = 1 << 5,
        // Emitter uses the given budget only and stops generating stuff
        UseBudgetOnly = 1 << 6,
        // Emitter generates stuff automatically when ParticlSystem is updated
        AutoGenerate = 1 << 7
    }
    #endregion

    /// <summary>
    /// Base class for a particle emitter class, for extension only
    /// 
    /// Emitter manages particles during their lifetime. 
    /// AngularVelocity, Opacity and Scale are interpolated during lifetime.
    /// LinearVelocity/speed of the particle is affected by the damping
    /// Rotation and Position is modified by angular velocity and linear velocity
    /// </summary>
    public abstract class ParticleEmitter : MovableGameObject, IRotatableObject
    {
        #region Fields
        // ParticleEffect which owns this emitter
        private ParticleEffect hostEffect;
        // Random generator
        protected Codesmith.SmithNgine.MathUtil.PseudoRandom random;
        // Managed particles
        protected List<Particle> particles;
        // List of generators for new particles
        private List<PropertyGenerator> generators;
        // List of modifiers for existing particles
        private List<ParticleModifier> modifiers;
        // List of texture to use
        private List<Image> textures;
        // Current rotation of the emitter
        private float rotation;
        // Name of the emitter
        private string name;
        // How many particles this emitter can still generate
        private int budget;
        // 
        private float fraction;
        #endregion

        #region Events
        /// <summary>
        /// Event which is triggered when Emitter rotation changes
        /// </summary>
        public event EventHandler<RotationEventArgs> RotationChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Get or set the name of this ParticleEmitter
        /// </summary>
        public string Name
        {
            get { return name; }
            internal set { name = value; }
        }

        /// <summary>
        /// Flags for different modes for the emitter
        /// </summary>
        public EmitterModes Flags
        {
            get;
            set;
        }
      
        /// <summary>
        /// Get the position of this emitter
        /// </summary>
        public override Point Position
        {
            get
            {
                return (Flags.HasFlag(EmitterModes.PositionRelative) && hostEffect != null) ?
                    base.Position + hostEffect.Position : base.Position;
            }
        }

        public float Rotation
        {
            get
            {
                return (Flags.HasFlag(EmitterModes.RotationRelative) && hostEffect != null) ? 
                    rotation + hostEffect.Rotation : rotation;
            }
            set
            {
                if (value != rotation)
                {
                    OnRotationChanged(rotation, value);
                }
                rotation = value;
            }
        }

        /// <summary>
        /// Get the count of particles active in this emitter
        /// </summary>
        public int ParticleCount
        {
            get { return particles.Count; }
        }

        /// <summary>
        /// The host ParticleEffect
        /// </summary>
        [Browsable(false)]
        public ParticleEffect Effect
        {
            get { return hostEffect; }
            internal set { hostEffect = value; }
        }

        /// <summary>
        /// How many particles per second to generate
        /// </summary>
        public float Quantity
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Intializes new instance to default values
        /// </summary>
        /// <param name="position"></param>
        public ParticleEmitter(Point position)
        {
            particles = new List<Particle>();
            generators = new List<PropertyGenerator>();
            modifiers = new List<ParticleModifier>();
            textures = new List<Image>();
            random = new Codesmith.SmithNgine.MathUtil.PseudoRandom();
            // by default, emitter is relative to the effect position
            Position = position;
            Quantity = 1;
            fraction = 0.0f;
            Flags = EmitterModes.PositionRelative | EmitterModes.RotationRelative;
            name = "AbstractEmitter";            
        }
        #endregion

        #region New methods
        /// <summary>
        /// Add a new property generator to this emitter
        /// </summary>
        /// <remarks>
        /// Generator can be added only once
        /// </remarks>
        /// <param name="generator">The PropertyGenerator to be added</param>
        public void AddPropertyGenerator(PropertyGenerator generator)
        {
            Debug.Assert(!generators.Contains(generator), 
                "Tried adding " + generator.ToString() + " twice");
            generators.Add(generator);
        }

        /// <summary>
        /// Add a new modifier to this emitter
        /// </summary>
        /// <remarks>
        /// Modifier can be added only once
        /// </remarks>
        /// <param name="modifier">The ParticleModifier to be added</param>
        public void AddParticleModifier(ParticleModifier modifier)
        {
            Debug.Assert(!modifiers.Contains(modifier), 
                "Tried adding " + modifier.ToString() + " twice");
            modifiers.Add(modifier);
        }

        /// <summary>
        /// Add a texture to this emitter
        /// </summary>
        /// <param name="tex"></param>
        public void AddTexture(Image tex)
        {
            Debug.Assert(!textures.Contains(tex),
                "Tried adding a texture twice");
            textures.Add(tex);
        }

        public void Generate(float elapsedSeconds)
        {
            // How many particles per frame to generate
            float ppf = Quantity * elapsedSeconds;
            fraction += ppf;
            if (fraction >= 1.0f)
            {
                Generate((int)fraction);
                fraction = 0f;
            }
        }

        /// <summary>
        /// Immediately generates one or more particles, particle is created in the 
        /// concrete emitter class by the method GenerateParticle()
        /// </summary>
        /// <param name="count">How many particles to generate at once</param>
        /// <returns>List of particles generated, these will be added to the ParticleSys</returns>
        public void Generate( int count )
        {
            Debug.Assert(textures.Count > 0, "No textures have been added to the emitter");
            Debug.Assert(hostEffect != null, "This emitter is not added to any ParticleEffect");
            ParticlePool pool = hostEffect.ParticleSystem.Pool;
            for (int i = 0; i < count; i++)
            {
                if (Flags.HasFlag(EmitterModes.UseBudgetOnly))
                {
                    budget--;
                    if (budget < 0) return;
                }

                Particle p = null;
                if (pool != null)
                {
                    p = pool.Get();
                    p.InitialDrawArea = Rectangle.FromCenter(Position, hostEffect.ParticleSize);
                    p.SetImage(textures[random.Next(textures.Count)]);
                }
                else
                {
                    p = new Particle(textures[random.Next(textures.Count)],
                        Rectangle.FromCenter(Position, hostEffect.ParticleSize));
                }
                Debug.Assert(p != null, "Particle was not instantiated or fetched from cache!");

                // Apply all generators to this particle
                foreach (PropertyGenerator g in generators)
                {
                    g.Apply(p);
                }
                // Call the concrete emitter for last modifications
                GenerateParticle(p);
                particles.Add(p);
//                hostEffect.Renderer.Add(p);
            }
        }
        #endregion

        #region From base class
        /// <summary>
        /// Called by the host ParticleEffects
        /// 
        /// Updates particles during their lifetime.
        /// 
        /// TODO! This will be refactored in future to use ParticleModifiers 
        /// instead of directly modifying the particle here.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(Time gameTime)
        {
            Debug.Assert(hostEffect != null, "This emitter is not added to any ParticleEffect");
            ParticlePool pool = hostEffect.ParticleSystem.Pool;
            for (int i = 0; i < particles.Count; i++)
            {
                Particle p = particles[i];

                p.TTLPercent += gameTime.CurrentDelta / p.TTL;
                foreach (ParticleModifier mod in modifiers)
                {
                    mod.Apply(p, gameTime.CurrentDelta);
                }

                p.Position += p.LinearVelocity * gameTime.CurrentDelta;

                // Finally apply the gravity from the effect
                p.LinearVelocity += hostEffect.GravityVector;

                if (p.TTLPercent >= 1.0f)
                {
                    if (pool != null)
                    {
                        pool.Insert(p);
                    }
//                    hostEffect.Renderer.Remove(p);
//                    particles[i].Dispose();
                    particles.RemoveAt(i);
                    i--;
                }
            }
        }
        #endregion

        #region Abstract methods
        /// <summary>
        /// Generates one new particle, must be implemented by concrete classes
        /// </summary>
        /// <returns>new Particle</returns>
        protected abstract void GenerateParticle(Particle p);
        #endregion

        #region Private methods
        private void OnRotationChanged(float oldRotation, float newRotation)
        {
            if (RotationChanged != null)
            {
                RotationEventArgs args = new RotationEventArgs(oldRotation, newRotation);
                RotationChanged(this, args);
            }
        }

        public virtual void Draw(Time gameTime)
        {
            // Draw the particles
            int pc = particles.Count;
            var vertices = new VertexPositionColorTextured[4];
            ScreenSpace screen = hostEffect.ParticleSystem.Screen;
            Particle p;
/*
 				GetVertex(DrawArea.TopLeft, Point.Zero), 
				GetVertex(DrawArea.TopRight, Point.UnitX),
				GetVertex(DrawArea.BottomRight, Point.One),
				GetVertex(DrawArea.BottomLeft, Point.UnitY
 */
            for (int i = 0; i < particles.Count; ++i)
            {
                p=particles[i];
                vertices[0] = new VertexPositionColorTextured(
                    screen.ToPixelSpaceRounded(Rotate(p.DrawArea.TopLeft, p)), p.Color, Point.Zero);
                vertices[1] = new VertexPositionColorTextured(
                    screen.ToPixelSpaceRounded(Rotate(p.DrawArea.TopRight, p)), p.Color, Point.UnitX);
                vertices[2] = new VertexPositionColorTextured(
                    screen.ToPixelSpaceRounded(Rotate(p.DrawArea.BottomRight, p)), p.Color, Point.One);
                vertices[3] = new VertexPositionColorTextured(
                    screen.ToPixelSpaceRounded(Rotate(p.DrawArea.BottomLeft, p)), p.Color, Point.UnitY);
                p.Image.Draw(vertices);
            }
        }

		protected Point Rotate(Point point, Particle particle)
		{
			var rotationCenter = particle.DrawArea.Center;
			point -= rotationCenter;
			float sin = MathFunctions.Sin(particle.Rotation);
			float cos = MathFunctions.Cos(particle.Rotation);
			point = new Point(point.X * cos - point.Y * sin, point.X * sin + point.Y * cos);
			return rotationCenter + point;
		}
        #endregion
    }
}
