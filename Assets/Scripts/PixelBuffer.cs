using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelBuffer
{
    public PixelBuffer(int size)
    {
        buffer = new int[size];
    }

    public int BufferLength => bufferIndex;

    private readonly int[] buffer;

    private int bufferIndex;

    public void Write(int value)
    {
        WriteInternal(value);
    }
    public int Read()
    {
        return ReadInternal();
    }

    private void WriteInternal(int value)
    {
        buffer[bufferIndex] = value;
        bufferIndex++;
    }
    private int ReadInternal()
    {
        //Debug.Log("Reading index " + (bufferIndex - 1));
        return buffer[--bufferIndex];
    }
}
