﻿/**
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using Codesmith.SmithNgine.General;
    using DeltaEngine.Core;
    using DeltaEngine.Datatypes;
    using DeltaEngine.Graphics;
    using DeltaEngine.Rendering;

    public enum ParticleSystemStatus
    {
        Idle,
        Running,
        Paused
    }

    /// <summary>
    /// Implements a particle system.
    /// 
    /// Particle system manages a set of ParticleEffects.
    /// 
    /// Call Update() in gameloop.
    /// 
    /// 
    /// </summary>
    public class ParticleSystem : MovableGameObject
    {
        #region Fields
        private List<ParticleEffect> effects;
        private ParticleSystemStatus status;
        private Size particleSize;
        #endregion

        /// <summary>
        /// Get the count of all particles in this system
        /// </summary>
        public int ParticleCount
        {
            get
            {
                int c = 0;
                foreach (ParticleEffect eff in effects)
                {
                    c += eff.ParticleCount;
                }
                return c;
            }
        }

        /// <summary>
        /// Get the number of effects managed by this system
        /// </summary>
        public int EffectCount
        {
            get
            {
                return effects.Count;
            }
        }

        /// <summary>
        /// Get the number of particle emitters in the system
        /// </summary>
        public int EmitterCount
        {
            get
            {
                int c = 0;
                foreach (ParticleEffect eff in effects)
                {
                    c += eff.EmitterCount;
                }
                return c;
            }
        }

        /// <summary>
        /// Is this particle system paused
        /// <value>true if paused, false if running</value> 
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return status == ParticleSystemStatus.Paused;
            }
        }

        /// <summary>
        /// Is this particle system running (means: Update() is called for subsystems)
        /// <value>true if running, false if paused</value>
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return status == ParticleSystemStatus.Running;
            }
        }

        /// <summary>
        /// Drawing
        /// </summary>
        public Drawing Drawing
        {
            get;
            set;
        }

        public ScreenSpace Screen
        {
            get;
            set;
        }


        /// <summary>
        /// The particle bool
        /// </summary>
        internal ParticlePool Pool
        {
            set;
            get;
        }

        public Size DefaultParticleSize
        {
            get { return particleSize; }
            set
            {
                particleSize = value;
                foreach (ParticleEffect eff in effects)
                {
                    eff.ParticleSize = value;
                }
            }
        }
        #region Constructor
        /// <summary>
        /// Constructs a new particle system
        /// </summary>
        public ParticleSystem(ScreenSpace screen, Drawing drawing) 
        {
            Drawing = drawing;
            Screen = screen;
            effects = new List<ParticleEffect>();
            DefaultParticleSize = Size.Half;
            this.status = ParticleSystemStatus.Idle;
            Resume();
        }
        #endregion

        #region New methods
        /// <summary>
        /// Add a particle effect to this system
        /// </summary>
        /// <param name="newEffect">ParticleEffect to be added</param>
        public void AddEffect(ParticleEffect newEffect)
        {
            Debug.Assert(!effects.Contains(newEffect), "Can't add same effect twice");
            newEffect.ParticleSystem = this;            
            newEffect.ParticleSize = DefaultParticleSize;
            effects.Add(newEffect);
        }

        /// <summary>
        /// Remove particle effect from the system
        /// </summary>
        /// <param name="effect"></param>
        public void RemoveEffect(ParticleEffect effect)
        {
            effects.Remove(effect);
        }

        /// <summary>
        /// Enable particle pool/cache 
        /// </summary>
        /// <param name="amount"></param>
        public void EnableCache(int amount = 1000)
        {
            Pool = new ParticlePool(amount);
        }

        /// <summary>
        /// Pause the system
        /// </summary>
        public void Pause()
        {
            this.status = ParticleSystemStatus.Paused;
        }

        /// <summary>
        /// Resume the system
        /// </summary>
        public void Resume()
        {
            this.status = ParticleSystemStatus.Running;
        }

        /// <summary>
        /// Clear the system, removes all effects
        /// </summary>
        public void Clear()
        {
            this.status = ParticleSystemStatus.Idle;
            effects.Clear();
        }
        #endregion

        #region From Base class
        public override void Update(Time gameTime)
        {
            // Update particles unless the system is paused
            if (IsRunning)
            {
                foreach (ParticleEffect eff in effects)
                {
                    eff.Update(gameTime);
                }
            }
        }

        public virtual void Draw(Time gameTime)
        {
            foreach (ParticleEffect e in effects)
            {
                e.Draw(gameTime, Drawing);
            }
        }
        #endregion

    }
}
