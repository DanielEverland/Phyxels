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

        writeCount = new int[bufferSize];

        readIndex = 0;
        writeIndex = 1;
        swapIndex = 1;
    }

    private readonly PixelBuffer[] buffers;
    private readonly int[] writeCount;

    private int readIndex;
    private int writeIndex;
    private int swapIndex;

    public bool ReadBufferContainsDiryPixels => buffers[readIndex].BufferLength > 0;

    public void Swap()
    {
        int oldReadIndex = readIndex;
        int oldWriteIndex = writeIndex;

        readIndex = WrapIndex(readIndex + 1);
        writeIndex = WrapIndex(writeIndex + 1);

        swapIndex++;

        //Debug.Log($"Swapped read index from {oldReadIndex} to {readIndex}, and write index from {oldWriteIndex} to {writeIndex}");

        //Debug.Log($"\tRead Buffer Values\n{buffers[readIndex][0]}\n{buffers[readIndex][1]}\n{buffers[readIndex][2]}\n{buffers[readIndex][3]}\n{buffers[readIndex][4]}");
        //Debug.Log($"\tWrite Buffer Values\n{buffers[writeIndex][0]}\n{buffers[writeIndex][1]}\n{buffers[writeIndex][2]}\n{buffers[writeIndex][3]}\n{buffers[writeIndex][4]}");
    }
    public void Write(int pixelPosition)
    {
        //Debug.Log($"Writing to buffer {writeIndex}, read buffer is {readIndex}");
        if (writeCount[pixelPosition] == swapIndex)
            return;

        writeCount[pixelPosition] = swapIndex;
        buffers[writeIndex].Write(pixelPosition);
    }
    public int Read()
    {
        //Debug.Log($"Reading from buffer {readIndex}, write buffer is {writeIndex}");
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
