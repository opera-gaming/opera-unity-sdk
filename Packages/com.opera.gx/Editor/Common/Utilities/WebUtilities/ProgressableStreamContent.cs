using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Opera
{
    internal class ProgressableStreamContent : HttpContent
    {
        public Action OnContentReadingInterrupted { get; set; }

        /// <summary>
        /// Lets keep buffer of 20kb
        /// </summary>
        private const int defaultBufferSize = 5 * 4096;

        private HttpContent content;

        private int bufferSize;

        private Action<long, long> progress;

        public ProgressableStreamContent(HttpContent content, Action<long, long> progress) : this(content,
            defaultBufferSize, progress)
        {
        }

        public ProgressableStreamContent(HttpContent content, int bufferSize, Action<long, long> progress)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }

            this.content = content;
            this.bufferSize = bufferSize;
            this.progress = progress;

            foreach (var h in content.Headers)
            {
                this.Headers.Add(h.Key, h.Value);
            }
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var buffer = new Byte[this.bufferSize];
                    long size;
                    TryComputeLength(out size);
                    var uploaded = 0;


                    using (var sinput = await content.ReadAsStreamAsync())
                    {
                        while (true)
                        {
                            var length = sinput.Read(buffer, 0, buffer.Length);
                            if (length <= 0) break;

                            uploaded += length;
                            progress?.Invoke(uploaded, size);

                            stream.Write(buffer, 0, length);
                            stream.Flush();
                        }
                    }

                    stream.Flush();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log($"Exception on uploading the game: {e}");
                    OnContentReadingInterrupted?.Invoke();
                }
            });
        }

        protected override bool TryComputeLength(out long length)
        {
            length = content.Headers.ContentLength.GetValueOrDefault();
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                content.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
