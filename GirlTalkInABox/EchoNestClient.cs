using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GirlTalkInABox
{
    class EchoNestClient
    {
        private readonly WebClient _webClient;

        public EchoNestClient()
        {
            _webClient = new WebClient();
        }

        public async Task<EchoNestAudioSummary> getSongSummary(Stream fileStream, String fileName)
        {
            var url = "https://developer.echonest.com/api/v4/track/upload";
            var postData = new Dictionary<string, string>();
            postData.Add("api_key", Secrets.ECHO_NEST_API_KEY);
            postData.Add("filetype", "mp3");

            var jsonResponse = await WebHelpers.UploadFile(url, postData, fileStream, "track");
            var response = _parse<EchoNestJSONWrapper>(jsonResponse);

            if (response.response.track.status == "complete")
            {
                return response.response.track.audio_summary;
            }
            
            if (response.response.track.status == "error")
            {
                // Bail, we can't handle this song :(
                return null;
            }

            if (response.response.track.status == "pending")
            {
                // Gotta keep trying
                var audio_summary = await _checkForAudioSummary(response.response.track.id);
                return audio_summary;
            }
            
            return null;
        }

        public async Task<EchoNestAudioSummary> getSongSummary(string trackId)
        {
            return await _checkForAudioSummary(trackId);
        }

        private async Task<EchoNestAudioSummary> _checkForAudioSummary(string trackId)
        {
            var request = await Task.Run(async () =>
            {
                var url = String.Format("https://developer.echonest.com/api/v4/track/profile?api_key={0}&id={1}&bucket=audio_summary", Secrets.ECHO_NEST_API_KEY, trackId);
                var maxTries = 12; // 5s wait * 12 = 60s total try
                var numTries = 1;
                while (numTries < maxTries) {
                    var response = await _getAndParse<EchoNestJSONWrapper>(url);
                    if (response.response.track.status == "complete")
                        {
                            return response.response.track.audio_summary;
                        }
                        else if (response.response.track.status == "error")
                        {
                            return null;
                        }
                        else
                        {
                            await Task.Delay(5000);
                            numTries += 1;
                        }
                }
                return null;
            });
            return request;
        }
        
        /// <summary>
        /// Gets the beats, bars, and tatums of a song.
        /// Use this to parse song analysis, unless you only want a specific subset.
        /// </summary>
        /// <param name="analysisUrl">audio_summary.analysis_url retrieved from getSongSummary()</param>
        /// <returns>EchoNestAnalysisResponse object containing all available analysis</returns>
        public async Task<EchoNestAnalysisResponse> getAnalysis(string analysisUrl)
        {
            var analysis = await _getAndParse<EchoNestAnalysisResponse>(analysisUrl);

            foreach(var bar in analysis.bars)
            {
                bar.beats = _getSubsetIn(analysis.beats, bar.start, Math.Round(bar.start + bar.duration, 5));
            }

            foreach(var beat in analysis.beats)
            {
                beat.tatums = _getSubsetIn(analysis.tatums, beat.start, Math.Round(beat.start + beat.duration, 5));
            }

            return analysis;
        }

        private IEnumerable<T> _getSubsetIn<T>(IEnumerable<T> baseSet, double start, double end) where T : EchoNestCommon
        {
            return baseSet.Where(x => x.start >= start && x.start < end);
        }

        public async Task<IEnumerable<EchoNestBeat>> getBeats(string analysisUrl)
        {
            var response = await _getAndParse<EchoNestAnalysisResponse>(analysisUrl);
            return response.beats;
        }

        public async Task<IEnumerable<EchoNestBar>> getBars(string analysisUrl)
        {
            var response = await _getAndParse<EchoNestAnalysisResponse>(analysisUrl);
            return response.bars;
        }

        public async Task<IEnumerable<EchoNestTatum>> getTatums(string analysisUrl)
        {
            var response = await _getAndParse<EchoNestAnalysisResponse>(analysisUrl);
            return response.tatums;
        }

        private async Task<T> _getAndParse<T>(string url)
        {
            var jsonString = await _webClient.DownloadStringTaskAsync(url);

            return _parse<T>(jsonString);
        }

        private T _parse<T>(string json)
        {
            using (var sr = new StringReader(json))
            using (var jr = new JsonTextReader(sr))
            {
                var js = new JsonSerializer();
                var response = js.Deserialize<T>(jr);

                return response;
            }
        }
    }

    class EchoNestJSONWrapper
    {
        public EchoNestResponse response { get; set; }
    }

    class EchoNestResponse
    {
        public EchoNestTrack track { get; set; }
    }

    class EchoNestTrack
    {
        public string id { get; set; }
        public string status { get; set; }
        public string artist { get; set; }
        public string title { get; set; }
        public EchoNestAudioSummary audio_summary { get; set; }
    }

    class EchoNestAudioSummary
    {
        public double energy { get; set; }
        public double valence { get; set; }
        public double danceability { get; set; }
        public string analysis_url { get; set; }

        private int time_signature { get; set; }
        private int key { get; set; }
        private double tempo { get; set; }
        private double duration { get; set; }
    }

    class EchoNestAnalysisResponse
    {
        public IEnumerable<EchoNestBeat> beats { get; set; }
        public IEnumerable<EchoNestBar> bars { get; set; }
        public IEnumerable<EchoNestTatum> tatums { get; set; }
    }

    class EchoNestCommon
    {
        public double start { get; set; }
        public double duration { get; set; }
        public double confidence { get; set; }
    }

    class EchoNestTatum : EchoNestCommon
    { }

    class EchoNestBeat : EchoNestCommon
    {
        public IEnumerable<EchoNestTatum> tatums { get; set; }
    }

    class EchoNestBar : EchoNestCommon
    {
        public IEnumerable<EchoNestBeat> beats { get; set; }
    }

}
