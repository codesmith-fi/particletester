﻿/**
 * SmithNgine Game Framework
 * 
 * Copyright (C) 2013 by Erno Pakarinen / Codesmith (www.codesmith.fi)
 * All Rights Reserved
 */

namespace Codesmith.SmithNgine.General
{
    using System;

    public class RotationEventArgs : EventArgs
    {
        public float oldRotation = 0.0f;
        public float rotation = 0.0f;
        public RotationEventArgs(float oldRotation, float newRotation)
        {
            this.oldRotation = oldRotation;
            this.rotation = newRotation;
        }
    }
}
