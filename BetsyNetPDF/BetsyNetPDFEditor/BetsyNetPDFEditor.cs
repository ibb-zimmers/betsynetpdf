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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace BetsyNetPDF
{
    public class BetsyNetPDFEditor
    {
        public static void ExportOverlayObjects2PDF(string input, string output, List<OverlayObject> objects, string layerTitle)
        {
            if (input.StartsWith("\""))
                input = input.Substring(1, input.Length - 1);
            if (input.EndsWith("\""))
                input = input.Substring(0, input.Length - 1);

            using (Stream inputStream = new FileStream(input, FileMode.Open))
            {
                PdfReader reader = new PdfReader(inputStream);
                PdfStamper stamper;
                if (reader.PdfVersion < '5')
                    stamper = new PdfStamper(reader, new FileStream(output, FileMode.Create), '5');
                else
                    stamper = new PdfStamper(reader, new FileStream(output, FileMode.Create));

                PdfContentByte layerContent = stamper.GetOverContent(1);
                PdfLayer objectLayer = new PdfLayer(layerTitle, stamper.Writer);
                layerContent.BeginLayer(objectLayer);

                iTextSharp.text.Font fontFG, fontBG;
                BaseFont baseFont;
                //BaseFont bfFont;
                List<Chunk> lines;
                Chunk tmp;
                BaseColor fg, bg;
                string[] slines;
                string tmpText;
                ColumnText column;
                float pageHeight = reader.GetPageSize(1).Height;
                float llx, lly, urx, ury, height, lineHeight, maxWidth, tmpWidth;
                foreach (OverlayObject oo in objects)
                {
                    height = maxWidth = tmpWidth = 0;
                    lines = new List<Chunk>();
                    lineHeight = oo.fontSize * 1.25f;

                    fg = new BaseColor(oo.foreGround.ToArgb());
                    bg = new BaseColor(oo.backGround.ToArgb());
                    fontFG = FontFactory.GetFont(oo.font, oo.fontSize, iTextSharp.text.Font.NORMAL, fg);
                    fontBG = FontFactory.GetFont(oo.font, oo.fontSize, iTextSharp.text.Font.NORMAL, bg);
                    baseFont = fontFG.GetCalculatedBaseFont(false);

                    slines = oo.label.Split('\n');
                    foreach (string sline in slines)
                    {
                        tmpText = sline.Replace("\r", "");
                        if (string.IsNullOrEmpty(tmpText))
                        {
                            tmpText = "_";
                            tmp = new Chunk(tmpText, fontBG);
                        }
                        else
                            tmp = new Chunk(tmpText, fontFG);

                        lines.Add(tmp);

                        height += lineHeight + 5;
                        tmpWidth = baseFont.GetWidthPoint(sline, oo.fontSize);
                        if (tmpWidth > maxWidth)
                            maxWidth = tmpWidth;
                    }

                    foreach (Chunk cline in lines)
                    {
                        tmpWidth = baseFont.GetWidthPoint(cline.Content, oo.fontSize);
                        cline.SetBackground(bg, 2, 2, 2 + maxWidth - tmpWidth, 3);
                    }

                    float xdpi = (float)oo.GetXdpi();
                    float ydpi = (float)oo.GetYdpi();
                    urx = xdpi + maxWidth + 4f;
                    ury = ydpi;
                    llx = xdpi;
                    lly = ury - height;

                    layerContent.SaveState();

                    //rotate
                    System.Drawing.Drawing2D.Matrix rotation = null;
                    if (oo.angle != 0.0)
                    {
                        rotation = new System.Drawing.Drawing2D.Matrix();
                        rotation.RotateAt((float)oo.angle, new PointF(xdpi, ydpi), MatrixOrder.Append);
                        layerContent.Transform(rotation);
                    }

                    column = new ColumnText(layerContent);
                    foreach (Chunk cline in lines)
                    {
                        column.AddText(cline);
                        column.AddText(Chunk.NEWLINE);
                    }

                    column.Alignment = Element.ALIGN_LEFT;
                    column.Leading = lineHeight;
                    column.SetSimpleColumn(llx, lly, urx, ury);
                    column.Go();

                    //rotate back
                    if (oo.angle != 0.0 && rotation != null)
                    {
                        rotation.Invert();
                        layerContent.Transform(rotation);
                        rotation.Dispose();
                    }

                    layerContent.RestoreState();
                }

                layerContent.EndLayer();

                stamper.Close();
                reader.Close();
            }
        }

        public static void ExportOverlayObjects2PDF(string input, string output, string sobjects, string layerTitle)
        {
            ExportOverlayObjects2PDF(input, output, OverlayObject.CreateOverlayObjectListFromString(sobjects), layerTitle);
        }

        public static Dictionary<string, PointF> ExtractTextAndCoordinates(string sourceFile, string regEx)
        {
            Dictionary<string, PointF> xyCoords = new Dictionary<string, PointF>();

            PdfReader reader = new PdfReader(sourceFile);
            int rotation = reader.GetPageRotation(1);
            rotation = 360 - rotation;
            iTextSharp.text.Rectangle pageSizeWR = reader.GetPageSizeWithRotation(1);
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);
            Parser.TextPositionExtractionStrategy strategy;
            Regex oRegex = new Regex(regEx);
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                strategy = parser.ProcessContent<Parser.TextPositionExtractionStrategy>(i, new Parser.TextPositionExtractionStrategy());

                foreach (Parser.MyTextChunk chunk in strategy.LocationResult)
                {
                    if (oRegex.IsMatch(chunk.Text))
                    {
                        if (rotation % 360 == 0)
                        {
                            xyCoords.Add(chunk.Text, new PointF(FromDPI(chunk.StartLocation[Vector.I1]), FromDPI(chunk.StartLocation[Vector.I2])));
                            continue;
                        }
                        if (rotation == 90)
                        {
                            xyCoords.Add(chunk.Text, new PointF(FromDPI(pageSizeWR.Width - chunk.StartLocation[Vector.I2]), FromDPI(chunk.StartLocation[Vector.I1])));
                            continue;
                        }
                        if (rotation == 180)
                        {
                            xyCoords.Add(chunk.Text, new PointF(FromDPI(pageSizeWR.Width - chunk.StartLocation[Vector.I1]), FromDPI(pageSizeWR.Height - chunk.StartLocation[Vector.I2])));
                            continue;
                        }
                        if (rotation == 270)
                        {
                            xyCoords.Add(chunk.Text, new PointF(FromDPI(chunk.StartLocation[Vector.I2]), FromDPI(pageSizeWR.Height - chunk.StartLocation[Vector.I1])));
                            continue;
                        }
                    }
                }
            }

            return xyCoords;
        }

        private static float FromDPI(float dpi)
        {
            return (dpi / 72.0f) * 2540.0f;
        }
    }
}
