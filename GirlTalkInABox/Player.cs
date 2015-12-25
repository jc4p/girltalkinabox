using NAudio.FileFormats.Mp3;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirlTalkInABox
{
    class Player
    {
        private Mp3FileReader _reader;
        private List<EndableBufferedWaveProvider> _buffers;
        private IWavePlayer _waveOut;

        public Player(Stream fileStream, IEnumerable<EchoNestBeat> beats, PlaybackStopped callback)
        {
            _buffers = new List<EndableBufferedWaveProvider>();
            _waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
            _waveOut.PlaybackStopped += (o, e) => {
                callback.OnPlaybackStopped();
            };

            _reader = new Mp3FileReader(fileStream);
            _reader.CurrentTime = TimeSpan.FromSeconds(beats.First().start);

            var decompressor = new DmoMp3FrameDecompressor(_reader.Mp3WaveFormat);
            
            var buffer = new byte[16384 * 8];
            var frame = _reader.ReadNextFrame();
            for (var beatIndex = 0; beatIndex < beats.Count(); beatIndex++) 
            {
                var beat = beats.ElementAt(beatIndex);
                var start = beat.start;
                var end = Math.Round(beat.start + beat.duration, 5);

                var waveProvider = new EndableBufferedWaveProvider(_reader.WaveFormat);
                waveProvider.BufferDuration = TimeSpan.FromSeconds(end - start);
                waveProvider.DiscardOnBufferOverflow = true;
                waveProvider.ReadFully = false;
                
                while (_reader.CurrentTime.TotalSeconds < end)
                {
                    int decompressed = decompressor.DecompressFrame(frame, buffer, 0);
                    waveProvider.AddSamples(buffer, 0, decompressed);
                    frame = _reader.ReadNextFrame();
                }

                _buffers.Add(waveProvider);
            }
        }

        public void play(int beatIndex)
        {
            _waveOut.Init(_buffers.ElementAt(beatIndex));
            _waveOut.Play();
        }
    }

    public interface PlaybackStopped
    {
        void OnPlaybackStopped();
    }
}
