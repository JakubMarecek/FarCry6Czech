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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Gibbed.IO;

namespace Gibbed.Dunia2.FileFormats
{
    public class XmlResourceFile
    {
        public void FromRML(Stream stream, Stream output)
        {
            Deserialize(stream);

            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true
            };

            using var writer = XmlWriter.Create(output, settings);
            writer.WriteStartDocument();
            WriteNode(writer, Root);
            writer.WriteEndDocument();
        }

        public void FromXML(Stream stream, Stream output)
        {
            var doc = new XPathDocument(stream);
            var nav = doc.CreateNavigator();

            if (nav.MoveToFirstChild() == false)
            {
                throw new FormatException();
            }

            Root = ReadNode(nav);

            Serialize(output);
        }

        public static Node ReadNode(XPathNavigator nav)
        {
            bool mustStartWithNumber = nav.Name[0] == '_';

            var node = new Node
            {
                Name = mustStartWithNumber ? nav.Name.Remove(0, 1) : nav.Name
            };

            if (nav.MoveToFirstAttribute() == true)
            {
                node.Attributes = new List<Attribute>();

                do
                {
                    node.Attributes.Add(new Attribute()
                    {
                        Name = nav.Name,
                        Value = nav.Value,
                    });
                }
                while (nav.MoveToNextAttribute() == true);
                nav.MoveToParent();
            }

            var children = nav.SelectChildren(XPathNodeType.Element);
            if (children.Count > 0)
            {
                node.Value = "";
                node.Children = new List<Node>();
                while (children.MoveNext() == true)
                {
                    if (children.Current == null)
                    {
                        throw new InvalidOperationException();
                    }

                    node.Children.Add(ReadNode(children.Current.CreateNavigator()));
                }
            }
            else
            {
                node.Value = nav.Value;
            }

            return node;
        }

        public static void WriteNode(XmlWriter writer, Node node)
        {
            bool startWithNumber = char.IsNumber(node.Name[0]);
            writer.WriteStartElement((startWithNumber ? "_" : "") + node.Name);

            foreach (var attribute in node.Attributes)
            {
                writer.WriteAttributeString(attribute.Name, attribute.Value);
            }

            foreach (var child in node.Children)
            {
                WriteNode(writer, child);
            }

            if (string.IsNullOrEmpty(node.Value) == false)
            {
                writer.WriteValue(node.Value);
            }

            writer.WriteEndElement();
        }

        public byte Unknown1;
        public Node Root;

        public void Deserialize(Stream input)
        {
            if (input.ReadValueU8() != 0)
            {
                throw new FormatException("not an xml resource file");
            }
            var endian = Endian.Little;

            this.Unknown1 = input.ReadValueU8();
            var stringTableSize = input.ReadValuePackedU32(endian);
            var totalNodeCount = input.ReadValuePackedU32(endian);
            var totalAttributeCount = input.ReadValuePackedU32(endian);

            uint actualNodeCount = 1, actualAttributeCount = 0;

            this.Root = new Node();
            this.Root.Deserialize(
                input, ref actualNodeCount, ref actualAttributeCount, endian);

            if (actualNodeCount != totalNodeCount ||
                actualAttributeCount != totalAttributeCount)
            {
                throw new FormatException();
            }

            var stringTableData = new byte[stringTableSize];
            input.Read(stringTableData, 0, stringTableData.Length);
            var stringTable = new StringTable();
            stringTable.Deserialize(stringTableData);

            this.Root.ReadStringTable(stringTable);
        }

        public void Serialize(Stream output)
        {
            var endian = Endian.Little;

            var stringTable = new StringTable();
            this.Root.WriteStringTable(stringTable);
            var stringTableData = stringTable.Serialize();

            output.WriteValueU8(0);
            output.WriteValueU8(0);

            using (var data = new MemoryStream())
            {
                uint totalNodeCount = 1, totalAttributeCount = 0;
                this.Root.Serialize(
                    data,
                    ref totalNodeCount,
                    ref totalAttributeCount,
                    endian);

                output.WriteValuePackedU32((uint)stringTableData.Length, endian);
                output.WriteValuePackedU32(totalNodeCount, endian);
                output.WriteValuePackedU32(totalAttributeCount, endian);

                data.Position = 0;
                output.WriteFromStream(data, data.Length);

                output.Write(stringTableData, 0, stringTableData.Length);
            }
        }

        public class Node
        {
            public string Name;
            public string Value;

            internal uint NameIndex;
            internal uint ValueIndex;

            public List<Attribute> Attributes = new List<Attribute>();
            public List<Node> Children = new List<Node>();

            public void Deserialize(Stream input,
                                    ref uint totalNodeCount,
                                    ref uint totalAttributeCount,
                                    Endian endian)
            {
                this.NameIndex = input.ReadValuePackedU32(endian);
                this.ValueIndex = input.ReadValuePackedU32(endian);

                var attributeCount = input.ReadValuePackedU32(endian);
                var childCount = input.ReadValuePackedU32(endian);

                totalNodeCount += childCount;
                totalAttributeCount += attributeCount;

                this.Attributes.Clear();
                for (uint i = 0; i < attributeCount; i++)
                {
                    var attribute = new Attribute();
                    attribute.Deserialize(input, endian);
                    this.Attributes.Add(attribute);
                }

                this.Children.Clear();
                for (uint i = 0; i < childCount; i++)
                {
                    var child = new Node();
                    child.Deserialize(input,
                                      ref totalNodeCount,
                                      ref totalAttributeCount,
                                      endian);
                    this.Children.Add(child);
                }
            }

            public void Serialize(Stream output,
                                  ref uint totalNodeCount,
                                  ref uint totalAttributeCount,
                                  Endian endian)
            {
                output.WriteValuePackedU32(this.NameIndex, endian);
                output.WriteValuePackedU32(this.ValueIndex, endian);

                totalAttributeCount += (uint)this.Attributes.Count;
                totalNodeCount += (uint)this.Children.Count;

                output.WriteValuePackedU32((uint)this.Attributes.Count, endian);
                output.WriteValuePackedU32((uint)this.Children.Count, endian);

                foreach (var attribute in this.Attributes)
                {
                    attribute.Serialize(output, endian);
                }

                foreach (var child in this.Children)
                {
                    child.Serialize(output,
                                    ref totalNodeCount,
                                    ref totalAttributeCount,
                                    endian);
                }
            }

            internal void ReadStringTable(StringTable stringTable)
            {
                this.Name = stringTable.Read(this.NameIndex);
                this.Value = stringTable.Read(this.ValueIndex);

                foreach (var attribute in this.Attributes)
                {
                    attribute.ReadStringTable(stringTable);
                }

                foreach (var child in this.Children)
                {
                    child.ReadStringTable(stringTable);
                }
            }

            internal void WriteStringTable(StringTable stringTable)
            {
                this.NameIndex = stringTable.Write(this.Name);
                this.ValueIndex = stringTable.Write(this.Value);

                foreach (var attribute in this.Attributes)
                {
                    attribute.WriteStringTable(stringTable);
                }

                foreach (var child in this.Children)
                {
                    child.WriteStringTable(stringTable);
                }
            }
        }

        public class Attribute
        {
            public uint Unknown;
            public string Name;
            public string Value;

            internal uint NameIndex;
            internal uint ValueIndex;

            public void Deserialize(Stream input, Endian endian)
            {
                this.Unknown = input.ReadValuePackedU32(endian);

                if (this.Unknown != 0)
                {
                    throw new FormatException();
                }

                this.NameIndex = input.ReadValuePackedU32(endian);
                this.ValueIndex = input.ReadValuePackedU32(endian);
            }

            public void Serialize(Stream output, Endian endian)
            {
                output.WriteValuePackedU32(this.Unknown, endian);
                output.WriteValuePackedU32(this.NameIndex, endian);
                output.WriteValuePackedU32(this.ValueIndex, endian);
            }

            internal void ReadStringTable(StringTable stringTable)
            {
                this.Name = stringTable.Read(this.NameIndex);
                this.Value = stringTable.Read(this.ValueIndex);
            }

            internal void WriteStringTable(StringTable stringTable)
            {
                this.NameIndex = stringTable.Write(this.Name);
                this.ValueIndex = stringTable.Write(this.Value);
            }
        }

        internal class StringTable
        {
            private MemoryStream _Data = new MemoryStream();

            // this is dumb :effort:
            private readonly Dictionary<uint, string> _Offsets = new Dictionary<uint, string>();
            private readonly Dictionary<string, uint> _Values = new Dictionary<string, uint>();

            public string Read(uint index)
            {
                if (this._Offsets.ContainsKey(index) == false)
                {
                    throw new KeyNotFoundException();
                }

                return this._Offsets[index];
            }

            public uint Write(string value)
            {
                if (this._Values.ContainsKey(value) == true)
                {
                    return this._Values[value];
                }

                var offset = (uint)this._Data.Position;
                this._Offsets.Add(offset, value);
                this._Values.Add(value, offset);
                this._Data.WriteStringZ(value, Encoding.UTF8);
                return offset;
            }

            public void Deserialize(byte[] buffer)
            {
                this._Offsets.Clear();
                this._Values.Clear();

                this._Data = new MemoryStream(buffer);
                while (this._Data.Position < this._Data.Length)
                {
                    var offset = (uint)this._Data.Position;
                    var value = this._Data.ReadStringZ(Encoding.UTF8);
                    this._Offsets.Add(offset, value);
                    this._Values.Add(value, offset);
                }
            }

            public byte[] Serialize()
            {
                var buffer = new byte[this._Data.Length];
                Array.Copy(this._Data.GetBuffer(), buffer, buffer.Length);
                return buffer;
            }
        }
    }
}
