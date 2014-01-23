/*
Copyright 2012-2014 IBB Ehlert&Wolf GbR
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

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
using BetsyNetPDF.Parser;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;

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
                float pageHeight = reader.GetPageSize(1).Height;
                float llx, lly, urx, ury, height, lineHeight, maxWidth, tmpWidth;
                System.Drawing.Drawing2D.Matrix rotation = null;
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
                    using (rotation = new System.Drawing.Drawing2D.Matrix())
                    {
                        if (oo.angle != 0.0)
                        {
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
                        if (oo.angle != 0.0 && !rotation.IsIdentity)
                        {
                            rotation.Invert();
                            layerContent.Transform(rotation);
                            rotation.Dispose();
                        }
                    }

                    layerContent.RestoreState();
                }

                layerContent.EndLayer();
            }
        }

        public static List<LocatedObject> ExtractTextAndCoordinates(string sourceFile, string regEx)
        {
            List<LocatedObject> locObjects = new List<LocatedObject>();
            if (string.IsNullOrEmpty(regEx))
                regEx = ".*";

            using (PdfReader reader = new PdfReader(sourceFile))
            {
                iTextSharp.text.Rectangle pageSizeWR = reader.GetPageSizeWithRotation(1);
                PdfReaderContentParser parser = new PdfReaderContentParser(reader);
                Parser.TextPositionExtractionStrategy strategy;
                Regex oRegex = new Regex(regEx);
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    strategy = parser.ProcessContent<Parser.TextPositionExtractionStrategy>(i, new Parser.TextPositionExtractionStrategy());

                    foreach (Parser.LocatedTextChunk chunk in strategy.LocationResult)
                        if (oRegex.IsMatch(chunk.Text))
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
                    layers.Add(l);
            }

            return layers;
        }

        public static List<LocatedObject> ExtractTextAndCoordsFromLayer(string file, string layer, string regEx)
        {
            List<string> texts = new List<string>();
            List<LocatedObject> layerObjs = new List<LocatedObject>();

            if (string.IsNullOrEmpty(regEx))
                regEx = ".*";

            if (string.IsNullOrEmpty(layer))
                return ExtractTextAndCoordinates(file, regEx);

            using (PdfReader reader = new PdfReader(file))
            using (PdfStamper stamper = new PdfStamper(reader, new MemoryStream()))
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
                    return layerObjs;

                PdfDictionary page = reader.GetPageN(1);
                if (page == null)
                    return layerObjs;

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
                    return layerObjs;

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
                    string oc = null, ocID = null, bdc = null, emc = null;
                    while (parser.Parse(operands).Count > 0)
                    {
                        if (operands.Count > 2 && oc == null && ocID == null && bdc == null && emc == null)
                        {
                            oc = operands[0].ToString();
                            ocID = operands[1].ToString();
                            bdc = operands[2].ToString();

                            if (!oc.Equals("/OC") || !ocID.Equals(ocgID.ToString()) || !bdc.Equals("BDC"))
                                oc = ocID = bdc = emc = null;
                        }
                        if (operands.Count == 1 && oc != null && ocID != null && bdc != null && emc == null)
                        {
                            emc = operands[0].ToString();
                            if (emc.Equals("EMC"))
                                oc = ocID = bdc = emc = null;
                            else
                                emc = null;
                        }

                        if (oc != null && ocID != null && bdc != null && emc == null)
                        {
                            foreach (PdfObject pdfop in operands)
                            {
                                if (pdfop.IsString())
                                    texts.Add(pdfop.ToString());
                            }
                        }
                    }
                }
            }

            if (texts.Count <= 0)
                return layerObjs;

            List<LocatedObject> allObjs = ExtractTextAndCoordinates(file, regEx);
            foreach (LocatedObject obj in allObjs)
            {
                Console.WriteLine(obj.ToString());
                if (texts.Contains(obj.Text))
                    layerObjs.Add(obj);
            }

            return layerObjs;
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
