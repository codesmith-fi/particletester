/**
 * SmithNgine Game Framework
 * 
 * Copyright (C) 2013 by Erno Pakarinen / Codesmith (www.codesmith.fi)
 * All Rights Reserved
 */
namespace Codesmith.SmithNgine.Primitives
{
    using System;
    using DeltaEngine.Datatypes;

    /// <summary>
    /// Implements a line primitive
    /// Line has two vectors for start and end point
    /// </summary>
    public class Line : IEquatable<Line>
    {
        public Point Start
        {
            set;
            get;
        }
        public Point End
        {
            set;
            get;
        }

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        public virtual Point Lerp(float amount)
        {
            return Point.Lerp(Start, End, amount);
        }

        public bool Equals(Line other)
        {
            return (other.Start == this.Start && other.End == this.End);
        }
    }
}
