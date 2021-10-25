/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

/* 
 * Mod Installer Dynamic Link Library
 * Copyright (C) 2020  Jakub Mareček (info@jakubmarecek.cz)
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Mod Installer.  If not, see <https://www.gnu.org/licenses/>.
 */

using Gibbed.Dunia2.BinaryObjectInfo;
using Gibbed.Dunia2.FileFormats;
using System.IO;
using System.Text;
using System.Xml;

namespace Gibbed.Dunia2.ConvertBinaryObject
{
    internal static class Exporting
    {
        public static void Export(Stream outputPath, BinaryObjectFile bof)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                CheckCharacters = false,
                OmitXmlDeclaration = false
            };

            using var writer = XmlWriter.Create(outputPath, settings);
            writer.WriteStartDocument();
            WriteNode(writer, bof.Root);
            writer.WriteEndDocument();
        }
        
        private static void WriteNode(XmlWriter writer, BinaryObject node)
        {
            writer.WriteStartElement("object");

            writer.WriteAttributeString("hash", node.NameHash.ToString("X8"));

            if (node.Fields != null)
            {
                foreach (var kv in node.Fields)
                {
                    writer.WriteStartElement("field");

                    writer.WriteAttributeString("hash", kv.Key.ToString("X8"));

                    writer.WriteAttributeString("type", FieldType.BinHex.GetString());
                    writer.WriteBinHex(kv.Value, 0, kv.Value.Length);

                    writer.WriteEndElement();
                }
            }

            foreach (var childNode in node.Children)
            {
                WriteNode(writer, childNode);
            }

            writer.WriteEndElement();
        }
    }
}
