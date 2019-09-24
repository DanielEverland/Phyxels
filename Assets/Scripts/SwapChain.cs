using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapChain
{
    public SwapChain(byte chainSize, int bufferSize)
    {
        if (chainSize < 2)
            throw new System.ArgumentException("Chain size must be 2 or higher");

        buffers = new PixelBuffer[chainSize];

        for (int i = 0; i < chainSize; i++)
        {
            buffers[i] = new PixelBuffer(bufferSize);
        }

        readIndex = 0;
        writeIndex = 1;
    }

    private readonly PixelBuffer[] buffers;
    private int readIndex;
    private int writeIndex;

    public bool ReadBufferContainsDiryPixels => buffers[readIndex].BufferLength > 0;

    public void Swap()
    {
        readIndex = WrapIndex(readIndex + 1);
        writeIndex = WrapIndex(writeIndex + 1);
    }
    public void Write(int pixelPosition)
    {
        buffers[writeIndex].Write(pixelPosition);
    }
    public int Read()
    {
        return buffers[readIndex].Read();
    }
    private int WrapIndex(int index)
    {
        if (index >= buffers.Length)
            return 0;
        else if (index < 0)
            return buffers.Length - 1;

        return index;
    }
}
