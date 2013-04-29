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
    using System;
using Codesmith.SmithNgine.MathUtil;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering;

    /// <summary>
    /// Implements a particle
    /// </summary>
    public class Particle : Sprite
    {
        private float scale;

        #region Properties
        public float InitialScale { get; set; }
        public float InitialSpeed { get; set; }
        public float InitialRotation { get; set; }
        public float InitialOpacity { get; set; }
        public float InitialAngularVelocity { get; set; }
        public float InitialDepth { get; set; }
        public Point LinearVelocity { get; set; }
        public float AngularVelocity { get; set; }

        public Point Origin
        {
            get { return RotationCenter; }
            set { RotationCenter = value; }
        }

        public new float Rotation
        {
            get { return base.Rotation * MathConstants.DegreesToRadiansRatio; }
            set { base.Rotation = value * MathConstants.RadiansToDegreesRatio; }
        }

        public Point Position 
        {
            get { return DrawArea.Center; }
            set { DrawArea.Center = value; }
        }
        public float Opacity  
        {
            get { return Color.AlphaValue; }
            set { Color.AlphaValue = value; }
        }

        public float Scale 
        {
            get { return scale; }
            set
            {
                scale = value;
                DrawArea = Rectangle.FromCenter(DrawArea.Center, InitialDrawArea.Size * scale);
            }
        }
        public float Depth { get; set; }
        public float TTL { get; set; }
        public float TTLPercent { get; set; }

        public Rectangle InitialDrawArea
        {
            get;
            internal set;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs the particle from given parameters
        /// 
        /// TODO: This will change a lot in the future. I intend
        /// to make PropertyGenerators to make it easier for extending
        /// this functionality instead of using this huge monolithic
        /// ParticleGenerationParams which contains everything!
        /// 
        /// </summary>
        /// <param name="p"></param>
        public Particle(Image image, Rectangle drawArea) : base(image, drawArea)
        {
            InitialDrawArea = drawArea;
            InitialSpeed = 0.0f;
            InitialScale = 1.0f;
            InitialOpacity = 1.0f;
            InitialRotation = 0.0f;
            InitialAngularVelocity = 0.0f;
            InitialDepth = 0.0f;
            Depth = 0.0f;
            Color = Color.White;

            Opacity = InitialOpacity;
            Rotation = InitialRotation;
            Scale = InitialScale;
            AngularVelocity = InitialAngularVelocity;
            TTL = 1.0f;
            TTLPercent = 0.0f;
        }
        #endregion
    }
}
