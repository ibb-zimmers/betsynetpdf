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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
using BetsyNetPDF.Parser;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.awt.geom;
using System.Text;

namespace BetsyNetPDF
{
    public class BetsyNetPDFEditor
    {
        public static void AttachFiles2PDF(string pdfFile, List<string> attachments)
        {

        }

        public static void ExportOverlayObjects2PDF(string input, string output, string sobjects, string layerTitle)
        {
            ExportOverlayObjects2PDF(input, output, OverlayObject.CreateOverlayObjectListFromString(sobjects), layerTitle);
        }

        public static void ExportOverlayObjects2PDF(string input, string output, List<OverlayObject> objects, string layerTitle)
        {
            if (input.StartsWith("\""))
                input = input.Substring(1, input.Length - 1);
            if (input.EndsWith("\""))
                input = input.Substring(0, input.Length - 1);

            using (Stream inputStream = new FileStream(input, FileMode.Open))
            using (FileStream outputStream = new FileStream(output, FileMode.Create))
            using (PdfReader reader = new PdfReader(inputStream))
            using (PdfStamper stamper = CreateStamper(reader, outputStream))
            {
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
                iTextSharp.text.Rectangle pageRect = reader.GetPageSizeWithRotation(1);
                iTextSharp.text.Rectangle cropBox = reader.GetCropBox(1);
                while (cropBox.Rotation != pageRect.Rotation)
                    cropBox = cropBox.Rotate();

                float llx, lly, urx, ury, height, lineHeight, maxWidth, tmpWidth;
                AffineTransform rotation = null;
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

                    float xdpi = (float)oo.GetXdpi() + pageRect.Left + cropBox.Left;
                    float ydpi = (float)oo.GetYdpi() + pageRect.Bottom + cropBox.Bottom;
                    urx = xdpi + maxWidth + 4f;
                    ury = ydpi;
                    llx = xdpi;
                    lly = ury - height;

                    layerContent.SaveState();

                    //rotate
                    rotation = new AffineTransform();
                    if (oo.angle != 0.0)
                    {
                        rotation.SetToRotation(oo.angle * (Math.PI / 180), xdpi, ydpi);
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
                    if (oo.angle != 0.0 && !rotation.IsIdentity())
                        layerContent.Transform(rotation.CreateInverse());

                    layerContent.RestoreState();
                }

                layerContent.EndLayer();
            }
        }

        public static void DebugPdfFile(string sourceFile)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                PdfContentReaderTool.ListContentStream(sourceFile, writer);
            }

            File.WriteAllText(sourceFile + ".debug", sb.ToString());
        }

        public static List<LocatedObject> ExtractTextAndCoordinates(string sourceFile, string regEx)
        {
            return ExtractTextAndCoordinates(new FileStream(sourceFile, FileMode.Open), regEx);
        }

        public static List<LocatedObject> ExtractTextAndCoordinates(Stream inputStream, string regEx)
        {
            List<LocatedObject> locObjects = new List<LocatedObject>();
            if (string.IsNullOrEmpty(regEx))
                regEx = ".*";

            using (PdfReader reader = new PdfReader(inputStream))
            {
                iTextSharp.text.Rectangle pageSizeWR = reader.GetPageSizeWithRotation(1);
                PdfReaderContentParser parser = new PdfReaderContentParser(reader);
                Parser.TextPositionExtractionStrategy strategy;
                Regex oRegex = new Regex(regEx);
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    strategy = parser.ProcessContent<Parser.TextPositionExtractionStrategy>(i, new Parser.TextPositionExtractionStrategy());

                    foreach (Parser.LocatedTextChunk chunk in strategy.LocationResult)
                        if (!string.IsNullOrEmpty(chunk.Text.Trim()) && oRegex.IsMatch(chunk.Text))
                            locObjects.Add(chunk.Convert2LocatedObject(pageSizeWR));
                }
            }

            return locObjects;
        }

        public static List<string> GetLayers(string file)
        {
            List<string> layers = new List<string>();

            using (PdfReader reader = new PdfReader(file))
            using (PdfStamper stamper = new PdfStamper(reader, new MemoryStream()))
            {
                Dictionary<string, PdfLayer> layerDict = stamper.GetPdfLayers();
                foreach (string l in layerDict.Keys)
                    layers.Add(layerDict[l].GetAsString(PdfName.NAME).ToString());
            }

            return layers;
        }

        public static List<LocatedObject> ExtractTextAndCoordsFromLayer(string file, string layer, string regEx)
        {
            List<LocatedObject> texts = new List<LocatedObject>();

            if (string.IsNullOrEmpty(regEx))
                regEx = ".*";

            if (string.IsNullOrEmpty(layer))
                return ExtractTextAndCoordinates(file, regEx);

            string tmpFile = Path.GetTempFileName();

            using (PdfReader reader = new PdfReader(file))
            using (FileStream resPdf = new FileStream(tmpFile, FileMode.Open))
            using (PdfStamper stamper = new PdfStamper(reader, resPdf))
            {
                PdfDictionary ocProps = reader.Catalog.GetAsDict(PdfName.OCPROPERTIES);
                PdfArray ocgs = ocProps.GetAsArray(PdfName.OCGS);

                PRIndirectReference ocgRef = null;
                PdfDictionary ocgDict = null;
                PdfString ocgName = null;
                foreach (PdfObject pdfo in ocgs.ArrayList)
                {
                    ocgRef = pdfo as PRIndirectReference;
                    if (ocgRef == null)
                        continue;

                    ocgDict = ocgRef.Reader.GetPdfObject(ocgRef.Number) as PdfDictionary;
                    if (ocgDict == null)
                        continue;

                    ocgName = ocgDict.GetAsString(PdfName.NAME);
                    if (ocgName == null)
                        continue;

                    if (ocgName.ToString().Equals(layer))
                        break;
                    else
                    {
                        ocgRef = null;
                        ocgDict = null;
                        ocgName = null;
                    }
                }

                if (ocgRef == null || ocgDict == null || ocgName == null)
                    return texts;

                PdfDictionary page = reader.GetPageN(1);
                if (page == null)
                    return texts;

                PdfDictionary resources = page.GetAsDict(PdfName.RESOURCES);
                PdfDictionary properties = resources.GetAsDict(PdfName.PROPERTIES);
                PdfDictionary curDict;
                PdfName ocgID = null;
                foreach (PdfName key in properties.Keys)
                {
                    curDict = properties.GetAsDict(key);
                    if (curDict == null)
                        continue;

                    if (curDict.Equals(ocgDict))
                    {
                        ocgID = key;
                        break;
                    }
                }

                if (ocgID == null)
                    return texts;

                List<PdfObject> contents;
                PdfArray contentArray = page.GetAsArray(PdfName.CONTENTS);
                if (contentArray != null)
                    contents = contentArray.ArrayList;
                else
                {
                    contents = new List<PdfObject>();
                    PdfObject contentStream = page.Get(PdfName.CONTENTS);
                    if (contentStream != null)
                        contents.Add(contentStream);
                }

                PRIndirectReference streamRef;
                PRStream stream;
                byte[] streamBytes;
                string curline, layerStart = string.Format("/OC {0} BDC", ocgID.ToString());
                bool insideLayer, insideTextObj, insideText;
                byte[] resbytes;
                StringReader contentReader;
                StringBuilder resContent;
                int curLineNo, lineNoTextStart;
                foreach (PdfObject pdfo in contents)
                {
                    insideLayer = false;
                    insideText = false;
                    insideTextObj = false;
                    resContent = new StringBuilder();

                    streamRef = pdfo as PRIndirectReference;
                    if (streamRef == null)
                        continue;

                    stream = streamRef.Reader.GetPdfObject(streamRef.Number) as PRStream;
                    if (stream == null)
                        continue;

                    streamBytes = PdfReader.GetStreamBytes(stream);
                    if (streamBytes.Length == 0)
                        continue;

                    using (contentReader = new StringReader(Encoding.UTF8.GetString(streamBytes)))
                    {
                        curLineNo = 0;
                        lineNoTextStart = -1;
                        while ((curline = contentReader.ReadLine()) != null)
                        {
                            curLineNo++;
                            if (curline.StartsWith(layerStart) || curline.Trim().EndsWith(layerStart) || curline.Contains(" " + layerStart + " "))
                                insideLayer = true;

                            if (curline.StartsWith("BT"))
                                insideTextObj = true;

                            if (insideTextObj && curline.StartsWith("ET"))
                                insideTextObj = insideText = false;

                            if (insideTextObj && !insideText && (curline.StartsWith("(") || curline.StartsWith("<") || (curline.Contains(" <") && !curline.Contains("("))))
                            {
                                insideText = true;
                                lineNoTextStart = curLineNo;
                            }

                            if (!insideLayer)
                            {
                                if (insideText && (curline.StartsWith("(") || curline.StartsWith("<")) && curline.EndsWith("Tj") && curLineNo == lineNoTextStart)
                                    curline = "()Tj";

                                if (insideText && (curline.Contains(" <") && !curline.Contains("(")) && curline.EndsWith("Tj") && curLineNo == lineNoTextStart)
                                    curline = curline.Substring(0, curline.IndexOf("<")) + "()Tj";

                                if (insideText && (curline.Contains(" <") && !curline.Contains("(")) && !curline.EndsWith("Tj") && curLineNo == lineNoTextStart)
                                    curline = curline.Substring(0, curline.IndexOf("<")) + "(";

                                if (insideText && (curline.StartsWith("(") || curline.StartsWith("<")) && curLineNo == lineNoTextStart && !curline.EndsWith("Tj"))
                                    curline = "(";

                                if (insideText && curLineNo > lineNoTextStart && curline.EndsWith("Tj"))
                                    curline = ")Tj";

                                if (insideText && curLineNo > lineNoTextStart && !curline.EndsWith("Tj"))
                                    curline = "";
                            }

                            if (insideText && curline.EndsWith("Tj"))
                                insideText = false;

                            resContent.AppendLine(curline);

                            if (insideLayer && (curline.StartsWith("EMC") || curline.Trim().EndsWith(" EMC") || curline.Contains(" EMC ")) && !curline.Contains(layerStart))
                                insideLayer = false;
                        }

                        resbytes = Encoding.ASCII.GetBytes(resContent.ToString());
                        stream.Put(PdfName.LENGTH, new PdfNumber(resbytes.Length));
                        stream.SetData(resbytes);
                    }
                }
            }

            texts = ExtractTextAndCoordinates(tmpFile, regEx);
            if (File.Exists(tmpFile))
                File.Delete(tmpFile);

            return texts;
        }

        // minLength in percentage of the page size 0.0 = 0% -> 1.0 = 100%
        public static List<PointF> ExtractLinesFromPdf(string file, float minLength)
        {
            List<PointF> linePoints = new List<PointF>();

            using (PdfReader reader = new PdfReader(file))
            {
                PdfDictionary page = reader.GetPageN(1);
                iTextSharp.text.Rectangle pageSizeWR = reader.GetPageSizeWithRotation(1);
                float maxPageSize;
                if (pageSizeWR.Height > pageSizeWR.Width)
                    maxPageSize = FromDPI(pageSizeWR.Height);
                else
                    maxPageSize = FromDPI(pageSizeWR.Width);

                if (page == null)
                    return linePoints;

                List<PdfObject> contents;
                PdfArray contentArray = page.GetAsArray(PdfName.CONTENTS);
                if (contentArray != null)
                    contents = contentArray.ArrayList;
                else
                {
                    contents = new List<PdfObject>();
                    PdfObject contentStream = page.Get(PdfName.CONTENTS);
                    if (contentStream != null)
                        contents.Add(contentStream);
                }

                PRIndirectReference streamRef;
                PRStream stream;
                PRTokeniser streamTok;
                PdfContentParser parser;
                byte[] streamBytes;
                List<PointF> currentPoints = null;
                PointF p1, p2;
                float length, tmpFacX = 1, tmpFacY = 1, tmpOffsetX = 0, tmpOffsetY = 0, facX = 1, facY = 1, offsetX = 0, offsetY = 0, x, y, cX = 0, cY = 0;
                bool facSet = false;
                string op;
                foreach (PdfObject pdfo in contents)
                {
                    streamRef = pdfo as PRIndirectReference;
                    if (streamRef == null)
                        continue;

                    stream = streamRef.Reader.GetPdfObject(streamRef.Number) as PRStream;
                    if (stream == null)
                        continue;

                    streamBytes = PdfReader.GetStreamBytes(stream);
                    if (streamBytes.Length == 0)
                        continue;

                    streamTok = new PRTokeniser(new RandomAccessFileOrArray(streamBytes));
                    parser = new PdfContentParser(streamTok);
                    List<PdfObject> operands = new List<PdfObject>();
                    while (parser.Parse(operands).Count > 0)
                    {
                        if (operands.Count == 7 && linePoints.Count == 0 && !facSet)
                        {
                            if (operands[6].ToString().Equals("cm"))
                            {
                                float.TryParse(operands[0].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmpFacX);
                                float.TryParse(operands[3].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmpFacY);
                                float.TryParse(operands[4].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmpOffsetX);
                                float.TryParse(operands[5].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out tmpOffsetY);

                                if (tmpFacX == 1.0f &&
                                    tmpFacY == 1.0f &&
                                    tmpOffsetX == 0.0f &&
                                    tmpOffsetY == 0.0f)
                                    continue;

                                facX = tmpFacX;
                                facY = tmpFacY;
                                offsetX = tmpOffsetX;
                                offsetY = tmpOffsetY;
                                facSet = true;
                            }
                        }
                        if (operands.Count == 3)
                        {
                            float.TryParse(operands[0].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out x);
                            float.TryParse(operands[1].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out y);
                            op = operands[2].ToString().ToLower();

                            if (currentPoints == null && op.Equals("m"))
                            {
                                currentPoints = new List<PointF>();
                                cX = x;
                                cY = y;
                            }
                            if (currentPoints != null && op.Equals("m"))
                            {
                                cX = x;
                                cY = y;
                            }
                            if (currentPoints != null && op.Equals("l"))
                            {
                                p1 = TranslateCoords((cX * facX) + offsetX, (cY * facY) + offsetY, pageSizeWR);
                                p2 = TranslateCoords((x * facX) + offsetX, (y * facY) + offsetY, pageSizeWR);
                                length = CalcLength(p1, p2);
                                if ((length / maxPageSize) > minLength)
                                {
                                    currentPoints.Add(p1);
                                    currentPoints.Add(p2);
                                }

                                cX = x;
                                cY = y;
                            }
                        }
                        if (operands.Count == 1)
                        {
                            op = operands[0].ToString().ToLower();
                            if (currentPoints != null && op.Equals("s"))
                            {
                                linePoints.AddRange(currentPoints);
                                currentPoints = null;
                            }
                            else
                                currentPoints = null;
                        }
                    }
                }
            }

            return linePoints;
        }

        public static PointF TranslateCoords(float x, float y, iTextSharp.text.Rectangle pageSize)
        {
            PointF res = PointF.Empty;
            int pageRotation = 360 - pageSize.Rotation;

            if (pageRotation % 360 == 0)
                res = new PointF(FromDPI(x), FromDPI(y));
            if (pageRotation == 90)
                res = new PointF(FromDPI(pageSize.Width - y), FromDPI(x));
            if (pageRotation == 180)
                res = new PointF(FromDPI(pageSize.Width - x), FromDPI(pageSize.Height - y));
            if (pageRotation == 270)
                res = new PointF(FromDPI(y), FromDPI(pageSize.Height - x));

            return res;
        }

        private static float CalcLength(PointF p1, PointF p2)
        {
            float pdfA = p2.X - p1.X;
            float pdfB = p2.Y - p1.Y;
            float pdfLength = (pdfA * pdfA) + (pdfB * pdfB);

            return (float)Math.Sqrt(pdfLength);
        }

        private static float FromDPI(float dpi)
        {
            return (dpi / 72.0f) * 2540.0f;
        }

        private static PdfStamper CreateStamper(PdfReader reader, FileStream output)
        {
            PdfStamper stamper;
            if (reader.PdfVersion < '5')
                stamper = new PdfStamper(reader, output, '5');
            else
                stamper = new PdfStamper(reader, output);

            return stamper;
        }
    }
}
