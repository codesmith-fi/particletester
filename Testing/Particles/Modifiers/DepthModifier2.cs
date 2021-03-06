﻿/**
 * SmithNgine Game Framework
 * 
 * Copyright (C) 2013 by Erno Pakarinen / Codesmith (www.codesmith.fi)
 * All Rights Reserved
 * 
 * For licensing terms, see License.txt which reflects to the current license
 * of this framework.
 */
namespace Codesmith.SmithNgine.Particles.Modifiers
{
    using System;
    using Codesmith.SmithNgine.MathUtil;

    [Serializable]
    public class DepthModifier2 : ParticleModifier
    {
        public float Initial { get; set; }
        public float Final { get; set; }

        public DepthModifier2(float initial, float final)
        {
            Initial = initial;
            Final = final;
        }

        public override void Apply(Particle p, float elapsedSeconds)
        {
            p.RenderLayer= (int)Interpolations.LinearInterpolate(
                Initial, Final, p.TTLPercent);
        }
    }
}
