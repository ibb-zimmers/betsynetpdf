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
    //this class was published on stackoverflow by greenhat http://stackoverflow.com/a/8825884
    class TextPositionExtractionStrategy : LocationTextExtractionStrategy
    {
        private List<LocatedTextChunk> m_locationResult = new List<LocatedTextChunk>();
        private List<TextInfo> m_TextLocationInfo = new List<TextInfo>();
        public List<LocatedTextChunk> LocationResult
        {
            get { return m_locationResult; }
        }
        public List<TextInfo> TextLocationInfo
        {
            get { return m_TextLocationInfo; }
        }

        /// <summary>
        /// Creates a new LocationTextExtracationStrategyEx
        /// </summary>
        public TextPositionExtractionStrategy()
        {
        }

        /// <summary>
        /// Returns the result so far
        /// </summary>
        /// <returns>a String with the resulting text</returns>
        public override String GetResultantText()
        {
            m_locationResult.Sort();

            StringBuilder sb = new StringBuilder();
            LocatedTextChunk lastChunk = null;
            TextInfo lastTextInfo = null;
            foreach (LocatedTextChunk chunk in m_locationResult)
            {
                if (lastChunk == null)
                {
                    sb.Append(chunk.Text);
                    lastTextInfo = new TextInfo(chunk);
                    m_TextLocationInfo.Add(lastTextInfo);
                }
                else
                {
                    if (chunk.sameLine(lastChunk))
                    {
                        float dist = chunk.distanceFromEndOf(lastChunk);

                        if (dist < -chunk.CharSpaceWidth)
                        {
                            sb.Append(' ');
                            lastTextInfo.addSpace();
                        }
                        //append a space if the trailing char of the prev string wasn't a space && the 1st char of the current string isn't a space
                        else if (dist > chunk.CharSpaceWidth / 2.0f && chunk.Text[0] != ' ' && lastChunk.Text[lastChunk.Text.Length - 1] != ' ')
                        {
                            sb.Append(' ');
                            lastTextInfo.addSpace();
                        }
                        sb.Append(chunk.Text);
                        lastTextInfo.appendText(chunk);
                    }
                    else
                    {
                        sb.Append('\n');
                        sb.Append(chunk.Text);
                        lastTextInfo = new TextInfo(chunk);
                        m_TextLocationInfo.Add(lastTextInfo);
                    }
                }
                lastChunk = chunk;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderInfo"></param>
        public override void RenderText(TextRenderInfo renderInfo)
        {
            LineSegment segment = renderInfo.GetBaseline();
            LocatedTextChunk location = new LocatedTextChunk(renderInfo.GetText(), segment.GetStartPoint(), segment.GetEndPoint(), renderInfo.GetSingleSpaceWidth(), renderInfo.GetAscentLine(), renderInfo.GetDescentLine());
            m_locationResult.Add(location);

        }
    }

    class TextInfo
    {
        public Vector TopLeft;
        public Vector BottomRight;
        private string m_Text;

        public string Text
        {
            get { return m_Text; }
        }

        /// <summary>
        /// Create a TextInfo.
        /// </summary>
        /// <param name="initialTextChunk"></param>
        public TextInfo(LocatedTextChunk initialTextChunk)
        {
            TopLeft = initialTextChunk.AscentLine.GetStartPoint();
            BottomRight = initialTextChunk.DecentLine.GetEndPoint();
            m_Text = initialTextChunk.Text;
        }

        /// <summary>
        /// Add more text to this TextInfo.
        /// </summary>
        /// <param name="additionalTextChunk"></param>
        public void appendText(LocatedTextChunk additionalTextChunk)
        {
            BottomRight = additionalTextChunk.DecentLine.GetEndPoint();
            m_Text += additionalTextChunk.Text;
        }

        /// <summary>
        /// Add a space to the TextInfo.  This will leave the endpoint out of sync with the text.
        /// The assumtion is that you will add more text after the space which will correct the endpoint.
        /// </summary>
        public void addSpace()
        {
            m_Text += ' ';
        }
    }
}
