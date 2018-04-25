using System.IO;

namespace Novacode
{
    /// <summary>
    /// OpenXML Isolated Storage access is not thread safe.
    /// Use app domain wide lock for writing.
    /// </summary>
    public class PackagePartStream : Stream
    {
        private static readonly object LockObject = new object();

        private readonly Stream _stream;

        public PackagePartStream(Stream stream)
        {
            this._stream = stream;
        }

        public override bool CanRead
        {
            get { return this._stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return this._stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return this._stream.CanWrite; }
        }

        public override long Length
        {
            get { return this._stream.Length; }
        }

        public override long Position
        {
            get { return this._stream.Position; }
            set { this._stream.Position = value; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this._stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this._stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this._stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (LockObject)
            {
                this._stream.Write(buffer, offset, count);
            }
        }

        public override void Flush()
        {
            lock (LockObject)
            {
                this._stream.Flush();
            }
        }

        public override void Close()
        {
            this._stream.Close();
        }

        protected override void Dispose(bool disposing)
        {
            this._stream.Dispose();
        }
    }
}
