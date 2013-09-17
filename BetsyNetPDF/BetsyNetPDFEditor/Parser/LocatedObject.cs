using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public LocatedObject(double x, double y, double angle, string text)
        {
            this.x = x;
            this.y = y;
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
