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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.XPath;

namespace Gibbed.Dunia2.ConvertBinaryObject
{
    internal class Importing
    {
        public Importing()
        {
        }

        public BinaryObject Import(XPathNavigator nav)
        {
            var root = new BinaryObject();
            ReadNode(null, root, nav);
            return root;
        }

        private void ReadNode(BinaryObject parent, BinaryObject node, XPathNavigator nav)
        {
            uint classNameHash;

            LoadNameAndHash(nav, out classNameHash);

            node.NameHash = classNameHash;
            node.Parent = parent;

            var fields = nav.Select("field");
            while (fields.MoveNext() == true)
            {
                if (fields.Current == null)
                {
                    throw new InvalidOperationException();
                }

                uint fieldNameHash;

                LoadNameAndHash(fields.Current, out fieldNameHash);

                if (fieldNameHash == CRC32.Hash("ArchetypeResDepList"))
                {
                    WriteListFiles(fields, node, fieldNameHash, "Resource");
                }
                else if (fieldNameHash == CRC32.Hash("hidShapePoints"))
                {
                    List<byte[]> resIdsBytes = new List<byte[]>();

                    var resIds = fields.Current.Select("Point");
                    while (resIds.MoveNext() == true)
                    {
                        var data = FieldTypeSerializers.Serialize(null, FieldType.Vector3, FieldType.Invalid, resIds.Current);
                        resIdsBytes.Add(data);
                    }

                    resIdsBytes.Insert(0, BitConverter.GetBytes(resIdsBytes.Count));

                    node.Fields.Add(fieldNameHash, resIdsBytes.SelectMany(byteArr => byteArr).ToArray());
                }
                else
                {
                    FieldType fieldType;
                    var fieldTypeName = fields.Current.GetAttribute("type", "");
                    if (Enum.TryParse(fieldTypeName, true, out fieldType) == false)
                    {
                        throw new InvalidOperationException();
                    }

                    var arrayFieldType = FieldType.Invalid;
                    var arrayFieldTypeName = fields.Current.GetAttribute("array_type", "");
                    if (string.IsNullOrEmpty(arrayFieldTypeName) == false)
                    {
                        if (Enum.TryParse(arrayFieldTypeName, true, out arrayFieldType) == false)
                        {
                            throw new InvalidOperationException();
                        }
                    }

                    var data = FieldTypeSerializers.Serialize(null, fieldType, arrayFieldType, fields.Current);
                    node.Fields.Add(fieldNameHash, data);
                }
            }

            var children = nav.Select("object");
            while (children.MoveNext() == true)
            {
                var child = new BinaryObject();
                ReadNode(node, child, children.Current);
                node.Children.Add(child);
            }
        }

        private static void LoadNameAndHash(XPathNavigator nav, out uint hash)
        {
            var nameAttribute = nav.GetAttribute("name", "");
            var hashAttribute = nav.GetAttribute("hash", "");

            if (string.IsNullOrWhiteSpace(nameAttribute) == true &&
                string.IsNullOrWhiteSpace(hashAttribute) == true)
            {
                throw new FormatException();
            }

            string name = string.IsNullOrWhiteSpace(nameAttribute) == false ? nameAttribute : null;
            hash = name != null ? CRC32.Hash(name) : uint.Parse(hashAttribute, NumberStyles.AllowHexSpecifier);
        }

        private static void WriteListFiles(XPathNodeIterator fields, BinaryObject node, uint fieldNameHash, string nodeName)
        {
            List<byte[]> resIdsBytes = new List<byte[]>();

            var resIds = fields.Current.Select(nodeName);
            while (resIds.MoveNext() == true)
            {
                string fileName = resIds.Current.GetAttribute("ID", "");
                ulong fileHash = 0;

                if (fileName.ToLowerInvariant().Contains("__unknown"))
                {
                    var partName = Path.GetFileNameWithoutExtension(fileName);

                    if (partName.Length > 16)
                    {
                        partName = partName.Substring(0, 16);
                    }

                    fileHash = ulong.Parse(partName, NumberStyles.AllowHexSpecifier);
                }
                else
                {
                    fileHash = CRC64.Hash(fileName);
                }

                resIdsBytes.Add(BitConverter.GetBytes(fileHash));
            }

            resIdsBytes.Insert(0, BitConverter.GetBytes(resIdsBytes.Count));

            node.Fields.Add(fieldNameHash, resIdsBytes.SelectMany(byteArr => byteArr).ToArray());
        }
    }
}
