namespace Max.ISolator.SeriaPortKit
{
    public interface AbsSerialPortCodec
    {
        byte[] Coding(byte[] data);
        byte[] Decoding(byte[] data);
    }
}
