﻿using System;

namespace common.libs
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ReceiveDataBuffer
    {
        private Memory<byte> items { get; set; }
        private int size = 0;
        /// <summary>
        /// 
        /// </summary>
        public int Size
        {
            get
            {
                return size;
            }
            private set
            {
                if (value == 0)
                {
                    items = Helper.EmptyArray;
                }
                else if (value > items.Length)
                {
                    Memory<byte> newItems = new byte[value].AsMemory();
                    items.CopyTo(newItems);
                    items = newItems;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Memory<byte> Data
        {
            get
            {
                return items;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void AddRange(Memory<byte> data)
        {
            if(data.Length > 0)
            {
                BeResize(data.Length);

                data.CopyTo(items.Slice(size, data.Length));
                size += data.Length;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        public void AddRange(Memory<byte> data, int length)
        {
            if (length > 0)
            {
                BeResize(length);

                data.Slice(0, length).CopyTo(items.Slice(size, length));
                size += length;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void AddRange(byte[] data, int offset, int length)
        {
            if(length > 0)
            {
                BeResize(length);
                data.AsMemory(offset, length).CopyTo(items.Slice(size, length));
                size += length;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public void RemoveRange(int index, int count)
        {
            if (index >= 0 && count > 0 && size - index >= count)
            {
                size -= count;
                if (index < size)
                {
                    items.Slice(index + count, size - index).CopyTo(items.Slice(index, size - index));
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clearData"></param>
        public void Clear(bool clearData = false)
        {
            size = 0;
            if (clearData)
            {
                Size = 0;
            }
        }

        private void BeResize(int length)
        {
            int _size = size + length;
            if (_size > items.Length)
            {
                int newsize = items.Length * 2;
                if (newsize < _size)
                {
                    newsize = _size;
                }
                Size = newsize;
            }
        }
    }
}
