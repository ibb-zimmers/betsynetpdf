/*
Copyright 2012, 2013 IBB Ehlert&Wolf GbR
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
using System.Globalization;

namespace BetsyNetPDF
{
    public class OverlayObject
    {
        public string id, label, font;
        public double x, y, dx, dy, lx, ly, angle;
        public float fontSize;
        public Color foreGround, backGround;

        public double GetXdpi()
        {
            return (x / 2540) * 72;
        }

        public double GetYdpi()
        {
            return (y / 2540) * 72;
        }

        // {id|label|x|y|dx|dy|lx|ly|angle|font|fontSize|fgR|fgG|fgB|bgR|bgG|bgB}
        public override string ToString()
        {
            StringBuilder obj = new StringBuilder();

            obj.Append("{");
            obj.AppendFormat("{0}|", id);
            obj.AppendFormat("{0}|", label);
            obj.AppendFormat(CultureInfo.InvariantCulture, "{0}|", x);
            obj.AppendFormat(CultureInfo.InvariantCulture, "{0}|", y);
            obj.AppendFormat(CultureInfo.InvariantCulture, "{0}|", dx);
            obj.AppendFormat(CultureInfo.InvariantCulture, "{0}|", dy);
            obj.AppendFormat(CultureInfo.InvariantCulture, "{0}|", lx);
            obj.AppendFormat(CultureInfo.InvariantCulture, "{0}|", ly);
            obj.AppendFormat(CultureInfo.InvariantCulture, "{0}|", angle);
            obj.AppendFormat("{0}|", font);
            obj.AppendFormat(CultureInfo.InvariantCulture, "{0}|", fontSize);
            obj.AppendFormat("{0}|", foreGround.R);
            obj.AppendFormat("{0}|", foreGround.G);
            obj.AppendFormat("{0}|", foreGround.B);
            obj.AppendFormat("{0}|", backGround.R);
            obj.AppendFormat("{0}|", backGround.G);
            obj.AppendFormat("{0}", backGround.B);
            obj.Append("}");

            return obj.ToString();
        }

        public static OverlayObject CreateFromString(string obj)
        {
            //remove {
            obj = obj.Substring(1, obj.Length - 1);
            //remove }
            obj = obj.Substring(0, obj.Length - 1);
            string[] tokens = obj.Split('|');

            int i = 0;
            OverlayObject oo = new OverlayObject();
            int fgR, fgG, fgB, bgR, bgG, bgB;
            fgR = fgG = fgB = bgR = bgG = bgB = 0;
            foreach (string token in tokens)
            {
                switch (i)
                {
                    case 0:
                        oo.id = token;
                        break;

                    case 1:
                        oo.label = token;
                        break;

                    case 2:
                        double.TryParse(token, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out oo.x);
                        break;

                    case 3:
                        double.TryParse(token, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out oo.y);
                        break;

                    case 4:
                        double.TryParse(token, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out oo.dx);
                        break;

                    case 5:
                        double.TryParse(token, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out oo.dy);
                        break;

                    case 6:
                        double.TryParse(token, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out oo.lx);
                        break;

                    case 7:
                        double.TryParse(token, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out oo.ly);
                        break;

                    case 8:
                        double.TryParse(token, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out oo.angle);
                        break;

                    case 9:
                        oo.font = token;
                        break;

                    case 10:
                        float.TryParse(token, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out oo.fontSize);
                        break;

                    case 11:
                        int.TryParse(token, out fgR);
                        break;

                    case 12:
                        int.TryParse(token, out fgG);
                        break;

                    case 13:
                        int.TryParse(token, out fgB);
                        break;

                    case 14:
                        int.TryParse(token, out bgR);
                        break;

                    case 15:
                        int.TryParse(token, out bgG);
                        break;

                    case 16:
                        int.TryParse(token, out bgB);
                        break;
                }

                i++;
            }

            oo.foreGround = Color.FromArgb(fgR, fgG, fgB);
            oo.backGround = Color.FromArgb(bgR, bgG, bgB);

            return oo;
        }

        public static List<OverlayObject> CreateOverlayObjectListFromString(string sobjects)
        {
            List<OverlayObject> objects = new List<OverlayObject>();
            if (string.IsNullOrEmpty(sobjects))
                return objects;

            int pos = sobjects.IndexOf('}');
            while (pos != -1)
            {
                objects.Add(OverlayObject.CreateFromString(sobjects.Substring(0, pos + 1)));
                sobjects = sobjects.Substring(pos + 1);
                pos = sobjects.IndexOf('}');
            }

            return objects;
        }

        public static string CreateStringFromOverlayObjectList(List<OverlayObject> objects)
        {
            string sobjects = "";
            foreach (OverlayObject oo in objects)
                sobjects += oo.ToString();

            return sobjects;
        }
    }
}
