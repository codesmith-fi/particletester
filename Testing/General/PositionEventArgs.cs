﻿/**
 * SmithNgine Game Framework
 * 
 * Copyright (C) 2013 by Erno Pakarinen / Codesmith (www.codesmith.fi)
 * All Rights Reserved
 */

namespace Codesmith.SmithNgine.General
{
    using System;
    using DeltaEngine.Datatypes;

    public class PositionEventArgs : EventArgs
    {
        public Point oldPosition = Point.Zero;
        public Point newPosition = Point.Zero;
        public PositionEventArgs(Point oldPosition, Point newPosition)
        {
            this.oldPosition = oldPosition;
            this.newPosition = newPosition;
        }
    }
}
