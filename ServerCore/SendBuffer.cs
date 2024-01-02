namespace ServerCore;

public class SendBufferHelper
{
    public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null;});

    public static int ChunkSize { get; set; } = 4096*100;

    public static ArraySegment<byte> Open(int reserveSize)
    {
        if (CurrentBuffer.Value == null)
        {
            CurrentBuffer.Value = new SendBuffer(ChunkSize);
        }
        if (CurrentBuffer.Value.FreeSize < reserveSize)
        {
            CurrentBuffer.Value=new SendBuffer(ChunkSize);
        }

        return CurrentBuffer.Value.Open(reserveSize);
    }

    public static ArraySegment<byte> Close(int usedSize)
    {
        return CurrentBuffer.Value.Close(usedSize);
    }
}

public class SendBuffer
{
    //[][u][][][][][][][][]

    private byte[] _buffer;
    private int _usedSize = 0; // 해당 인덱스부터 넣을수 있음

    public SendBuffer(int chunkSize)
    {
        _buffer = new byte[chunkSize];
    }

    public int FreeSize { get { return _buffer.Length - _usedSize; } } //남은 공간의 크기

    /// <summary>
    /// 버퍼를 사용할거라고 Open을하고, 얼마만큼의 최대치를 사용할 건지를 넣어줌
    /// </summary>
    /// <param name="reserveSize">예상 버퍼 사용 최대치</param>
    /// <returns>
    ///     요구한 예약 공간보다 남은 공간이 적으면 null을 리턴.
    ///     <br>그렇지 않다면 예약공간 만큼의 바이트배열 세그먼트를 리턴</br>
    /// </returns>
    public ArraySegment<byte> Open(int reserveSize)
    {
        if (reserveSize > FreeSize)
            return null;

        //실제로 reserveSize만큼을 전부 다 안쓸수도있고, 예약만 한 상태니까 아직 _usedSize를 업데이트 시키진않음.
        return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="usedSize">실제로 사용한 바이크크기</param>
    /// <returns>최종적으로 사용한 버퍼 부분을 리턴</returns>
    public ArraySegment<byte> Close(int usedSize)
    {
        ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
        _usedSize += usedSize;
        return segment;
    }
}