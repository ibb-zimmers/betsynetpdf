/*
Copyright 2012-2015 IBB Ehlert&Wolf GbR
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
using System.Drawing;

namespace BetsyNetPDF.Parser
{
    public class LocatedObject
    {
        private double x, y, angle;
        private string text;
        private long id = 0;

        public long ID { get { return CalcID(); } }
        public double X { get { return this.x; } }
        public double Y { get { return this.y; } }
        public double Angle { get { return this.angle; } }
        public string Text { get { return this.text; } }

        public LocatedObject(PointF origin, double angle, string text)
        {
            this.x = origin.X;
            this.y = origin.Y;
            this.angle = angle;
            this.text = text;
        }

        public override string ToString()
        {
            return string.Format("{0}; {1}; {2}; {3}; {4}", this.ID, this.Text, this.X, this.Y, this.Angle);
        }

        private long CalcID()
        {
            if (id != 0)
                return id;

            string sx = ((int)this.X).ToString();
            string sy = ((int)this.Y).ToString();
            string sid = sx + sy;
            if (sid.Length < 18)
                sid = sx.PadRight(sx.Length + (18 - sid.Length), '0') + sy;

            long.TryParse(sid, out this.id);

            while (id == 0)
            {
                sx = sx.Substring(0, sx.Length - 1);
                sy = sy.Substring(0, sy.Length - 1);

                long.TryParse(sx + sy, out this.id);
            }

            return id;
        }
    }
}
