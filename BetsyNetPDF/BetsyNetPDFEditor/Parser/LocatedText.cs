/*
Copyright 2012-2016 IBB Ehlert&Wolf GbR
Author: Silvio Zimmer

This file is part of BetsyNetPDF.

BetsyNetPDF is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

BetsyNetPDF is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with BetsyNetPDF.  If not, see <http://www.gnu.org/licenses/>.

Diese Datei ist Teil von BetsyNetPDF.

BetsyNetPDF ist Freie Software: Sie können es unter den Bedingungen
der GNU Lesser General Public License, wie von der Free Software Foundation,
Version 3 der Lizenz oder (nach Ihrer Option) jeder späteren
veröffentlichten Version, weiterverbreiten und/oder modifizieren.

BetsyNetPDF wird in der Hoffnung, dass es nützlich sein wird, aber
OHNE JEDE GEWÄHELEISTUNG, bereitgestellt; sogar ohne die implizite
Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK.
Siehe die GNU Lesser General Public License für weitere Details.

Sie sollten eine Kopie der GNU Lesser General Public License zusammen mit diesem
Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf.parser;

namespace BetsyNetPDF.Parser
{
    public class LocatedTextChunk : IComparable, ICloneable
    {
        string m_text;
        Vector m_startLocation;
        Vector m_endLocation;
        Vector m_orientationVector;
        int m_orientationMagnitude;
        int m_distPerpendicular;
        float m_distParallelStart;
        float m_distParallelEnd;
        float m_charSpaceWidth;

        public LineSegment AscentLine;
        public LineSegment DecentLine;

        public object Clone()
        {
            LocatedTextChunk copy = new LocatedTextChunk(m_text, m_startLocation, m_endLocation, m_charSpaceWidth, AscentLine, DecentLine);
            return copy;
        }

        public string Text
        {
            get { return m_text; }
            set { m_text = value; }
        }
        public float CharSpaceWidth
        {
            get { return m_charSpaceWidth; }
            set { m_charSpaceWidth = value; }
        }
        public Vector StartLocation
        {
            get { return m_startLocation; }
            set { m_startLocation = value; }
        }
        public Vector EndLocation
        {
            get { return m_endLocation; }
            set { m_endLocation = value; }
        }
        public double Angle { get { return CalcAngle(); } }

        /// <summary>
        /// Represents a chunk of text, it's orientation, and location relative to the orientation vector
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="startLoc"></param>
        /// <param name="endLoc"></param>
        /// <param name="charSpaceWidth"></param>
        public LocatedTextChunk(string txt, Vector startLoc, Vector endLoc, float charSpaceWidth, LineSegment ascentLine, LineSegment decentLine)
        {
            m_text = txt;
            m_startLocation = startLoc;
            m_endLocation = endLoc;
            m_charSpaceWidth = charSpaceWidth;
            AscentLine = ascentLine;
            DecentLine = decentLine;

            m_orientationVector = m_endLocation.Subtract(m_startLocation).Normalize();
            m_orientationMagnitude = (int)(Math.Atan2(m_orientationVector[Vector.I2], m_orientationVector[Vector.I1]) * 1000);

            // see http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
            // the two vectors we are crossing are in the same plane, so the result will be purely
            // in the z-axis (out of plane) direction, so we just take the I3 component of the result
            Vector origin = new Vector(0, 0, 1);
            m_distPerpendicular = (int)(m_startLocation.Subtract(origin)).Cross(m_orientationVector)[Vector.I3];

            m_distParallelStart = m_orientationVector.Dot(m_startLocation);
            m_distParallelEnd = m_orientationVector.Dot(m_endLocation);
        }

        /// <summary>
        /// true if this location is on the the same line as the other text chunk
        /// </summary>
        /// <param name="textChunkToCompare">the location to compare to</param>
        /// <returns>true if this location is on the the same line as the other</returns>
        public bool sameLine(LocatedTextChunk textChunkToCompare)
        {
            if (m_orientationMagnitude != textChunkToCompare.m_orientationMagnitude) return false;
            if (m_distPerpendicular != textChunkToCompare.m_distPerpendicular) return false;
            return true;
        }

        /// <summary>
        /// Computes the distance between the end of 'other' and the beginning of this chunk
        /// in the direction of this chunk's orientation vector.  Note that it's a bad idea
        /// to call this for chunks that aren't on the same line and orientation, but we don't
        /// explicitly check for that condition for performance reasons.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>the number of spaces between the end of 'other' and the beginning of this chunk</returns>
        public float distanceFromEndOf(LocatedTextChunk other)
        {
            float distance = m_distParallelStart - other.m_distParallelEnd;
            return distance;
        }

        /// <summary>
        /// Compares based on orientation, perpendicular distance, then parallel distance
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null) throw new ArgumentException("Object is now a TextChunk");

            LocatedTextChunk rhs = obj as LocatedTextChunk;
            if (rhs != null)
            {
                if (this == rhs) return 0;

                int rslt;
                rslt = m_orientationMagnitude - rhs.m_orientationMagnitude;
                if (rslt != 0) return rslt;

                rslt = m_distPerpendicular - rhs.m_distPerpendicular;
                if (rslt != 0) return rslt;

                // note: it's never safe to check floating point numbers for equality, and if two chunks
                // are truly right on top of each other, which one comes first or second just doesn't matter
                // so we arbitrarily choose this way.
                rslt = m_distParallelStart < rhs.m_distParallelStart ? -1 : 1;

                return rslt;
            }
            else
            {
                throw new ArgumentException("Object is now a TextChunk");
            }
        }

        public LocatedObject Convert2LocatedObject(iTextSharp.text.Rectangle pageSize)
        {
            LocatedObject locObj = null;
            double angle = this.Angle - pageSize.Rotation;
                        
            locObj = new LocatedObject(BetsyNetPDFEditor.TranslateCoords(this.AscentLine.GetStartPoint()[Vector.I1], this.AscentLine.GetStartPoint()[Vector.I2], pageSize), angle, this.Text);

            return locObj;
        }

        private double CalcAngle()
        {
            double angle = 0.0;

            float x = this.m_orientationVector[Vector.I1];
            float y = this.m_orientationVector[Vector.I2];

            if (x == 1.0 && y == 0.0)
                return 0.0;
            if (x == -1.0 && y == 0.0)
                return 180.0;
            if (x == 0.0 && y == 1.0)
                return 90.0;
            if (x == 0.0 && y == -1.0)
                return 270.0;

            float sina = 1 / y;
            angle = Math.Sin(sina);
            angle = (180 / Math.PI) * angle;

            return angle;
        }
    }
}
