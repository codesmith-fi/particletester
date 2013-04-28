﻿/**
 * SmithNgine Game Framework
 * 
 * Copyright (C) 2013 by Erno Pakarinen / Codesmith (www.codesmith.fi)
 * All Rights Reserved
 */

namespace Codesmith.SmithNgine.General
{
    using System;
    using DeltaEngine.Core;

    public interface IActivatableObject
    {
        bool ObjectIsActive
        {
            get;
        }

        void ActivateObject();
        void DeactivateObject();
        void Dismiss();
        void Update(Time time);

        event EventHandler<EventArgs> ObjectActivated;
        event EventHandler<EventArgs> ObjectDeactivated;
    }
}
