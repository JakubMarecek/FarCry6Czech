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

using Gibbed.IO;
using ModInstallerData;
using System;
using System.Collections.Generic;
using System.IO;

namespace Gibbed.Dunia2.FileFormats
{
    public class BinaryObject
    {
        public long Position { get; set; }

        public uint NameHash { get; set; }

        public Dictionary<uint, byte[]> Fields { get; } = new Dictionary<uint, byte[]>();

        public List<BinaryObject> Children { get; } = new List<BinaryObject>();

        public BinaryObject Parent { set; get; }

        public void Serialize(Stream output,
                              ref uint totalObjectCount,
                              ref uint totalValueCount,
                              Endian endian)
        {
            totalObjectCount += (uint)this.Children.Count;
            totalValueCount += (uint)this.Fields.Count;

            output.WriteCount(this.Children.Count, false, endian);

            output.WriteValueU32(this.NameHash, endian);

            output.WriteCount(this.Fields.Count, false, endian);
            foreach (var kv in this.Fields)
            {
                output.WriteValueU32(kv.Key, endian);
                output.WriteCount(kv.Value.Length, false, endian);
                output.WriteBytes(kv.Value);
            }

            foreach (var child in this.Children)
            {
                child.Serialize(output,
                                ref totalObjectCount,
                                ref totalValueCount,
                                endian);
            }
        }

        public static BinaryObject MakeNewInstance(BinaryObject parent, BinaryObject pointer, int pos = 0)
        {
            BinaryObject pointerChild = new BinaryObject();
            pointerChild.NameHash = pointer.NameHash;
            pointerChild.Position = pos;
            pointerChild.Parent = parent;

            foreach (BinaryObject child in pointer.Children)
                pointerChild.Children.Add(MakeNewInstance(pointerChild, child, pos));

            pointerChild.Fields.AddRange(pointer.Fields);

            return pointerChild;
        }

        public static BinaryObject Deserialize(BinaryObject parent,
                                               Stream input,
                                               List<BinaryObject> pointers,
                                               Endian endian)
        {
            long position = input.Position;

            bool isOffset;
            var childCount = input.ReadCount(out isOffset, endian);

            if (isOffset == true)
            {
                return MakeNewInstance(parent, pointers[(int)childCount]);
                //return pointers[(int)childCount];
            }

            var child = new BinaryObject();
            child.Parent = parent;
            child.Position = position;
            pointers.Add(child);

            child.Deserialize(input, childCount, pointers, endian);
            return child;
        }

        private void Deserialize(Stream input,
                                 uint childCount,
                                 List<BinaryObject> pointers,
                                 Endian endian)
        {
            bool isOffset;

            this.NameHash = input.ReadValueU32(endian);

            var valueCount = input.ReadCount(out isOffset, endian);
            if (isOffset == true)
            {
                throw new NotImplementedException();
            }

            this.Fields.Clear();
            for (var i = 0; i < valueCount; i++)
            {
                var nameHash = input.ReadValueU32(endian);
                byte[] value;

                var position = input.Position;
                var size = input.ReadCount(out isOffset, endian);
                if (isOffset == true)
                {
                    input.Seek(position - size, SeekOrigin.Begin);

                    size = input.ReadCount(out isOffset, endian);
                    if (isOffset == true)
                    {
                        throw new FormatException();
                    }

                    value = input.ReadBytes((int)size);

                    input.Seek(position, SeekOrigin.Begin);
                    input.ReadCount(out isOffset, endian);
                }
                else
                {
                    value = input.ReadBytes((int)size);
                }

                this.Fields.Add(nameHash, value);
            }

            this.Children.Clear();
            for (var i = 0; i < childCount; i++)
            {
                this.Children.Add(Deserialize(this, input, pointers, endian));
            }
        }

        public IEnumerable<BinaryObject> GetDescendantChild(uint hashValue)
        {
            foreach (BinaryObject binaryObject in Children)
            {
                if (binaryObject.NameHash == hashValue && binaryObject.Position >= 0)
                    yield return binaryObject;

                foreach (BinaryObject child in binaryObject.GetDescendantChild(hashValue))
                {
                    yield return child;
                }
            }
        }

        /*public IEnumerable<byte[]> GetDescendantFields(uint hashValue, uint inParent)
        {
            foreach (BinaryObject binaryObject in Children)
            {
                foreach (KeyValuePair<uint, byte[]> binaryObjectField in binaryObject.Fields)
                    if (binaryObjectField.Key == hashValue && ((inParent != 0 && binaryObject.NameHash == inParent) || inParent == 0))
                        yield return binaryObjectField.Value;

                foreach (byte[] child in binaryObject.GetDescendantFields(hashValue, inParent))
                {
                    yield return child;
                }
            }
        }

        public IEnumerable<byte[]> GetDescendantFields(Dictionary<uint, uint> search)
        {
            foreach (BinaryObject binaryObject in Children)
            {
                foreach (KeyValuePair<uint, byte[]> binaryObjectField in binaryObject.Fields)
                    if (search.ContainsKey(binaryObject.NameHash) && search.ContainsValue(binaryObjectField.Key))
                        yield return binaryObjectField.Value;

                foreach (byte[] child in binaryObject.GetDescendantFields(search))
                {
                    yield return child;
                }
            }
        }*/

        /*public IEnumerable<byte[]> GetDescendantFields(Dictionary<uint, uint> search)
        {
            foreach (BinaryObject binaryObject in Children)
            {
                foreach (KeyValuePair<uint, byte[]> binaryObjectField in binaryObject.Fields)
                    foreach (KeyValuePair<uint, uint> srch in search)
                        if (((srch.Key != 0 && binaryObject.NameHash == srch.Key) || srch.Key == 0) && binaryObjectField.Key == srch.Value)
                            yield return binaryObjectField.Value;

                foreach (byte[] child in binaryObject.GetDescendantFields(search))
                {
                    yield return child;
                }
            }
        }*/

        public IEnumerable<string> GetDescendantFields(Dictionary<uint, uint> search)
        {
            foreach (BinaryObject binaryObject in Children)
            {
                foreach (KeyValuePair<uint, byte[]> binaryObjectField in binaryObject.Fields)
                    foreach (KeyValuePair<uint, uint> srch in search)
                        if (((srch.Key != 0 && binaryObject.NameHash == srch.Key) || srch.Key == 0) && binaryObjectField.Key == srch.Value)
                            yield return binaryObjectField.Value.ByteArrayToHexViaLookup32();

                foreach (string child in binaryObject.GetDescendantFields(search))
                {
                    yield return child;
                }
            }
        }

        /*public int GetDescendantFieldCount(uint hashValue, int count = 0)
        {
            foreach (BinaryObject binaryObject in Children)
            {
                foreach (KeyValuePair<uint, byte[]> binaryObjectField in binaryObject.Fields)
                    if (binaryObjectField.Key == hashValue)
                        count++;

                foreach (BinaryObject childs in binaryObject.Children)
                {
                    count += GetDescendantFieldCount(hashValue, count);
                }
            }

            return count;
        }*/
    }
}
