/**
 * SmithNgine Game Framework
 * 
 * Copyright (C) 2013 by Erno Pakarinen / Codesmith (www.codesmith.fi)
 * All Rights Reserved
 */
namespace Codesmith.SmithNgine.General
{
    using System;

    interface IRotatableObject
    {
        float Rotation
        {
            get;
            set;
        }

        event EventHandler<RotationEventArgs> RotationChanged;
    }
}
