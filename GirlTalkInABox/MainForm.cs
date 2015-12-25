using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace GirlTalkInABox
{
    public partial class MainForm : Form, PlaybackStopped
    {
        private readonly EchoNestClient _client;
        private Player _player;

        private int _currentBeat;
        private int _nextBeat;

        public MainForm()
        {
            InitializeComponent();
            _client = new EchoNestClient();
        }

        public void OnPlaybackStopped()
        {
            if (_currentBeat < 20)
            {
                _currentBeat += 1;
                _nextBeat += 1;
                _player.play(_currentBeat);
            }
        }

        private async void newSongButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName.Split('\\').Last();
                var updated = await updateSong(openFileDialog1.OpenFile(), fileName);
                // #TODO: Show an error if updated == false? Could be a file read error a network error or a echonest error

                if(updated)
                {
                    // For now let's just play the first 20 beats of the song whenever we didn't fail
                    _currentBeat = 0;
                    _nextBeat = 1;
                    _player.play(_currentBeat);
                }
            }
        }

        private async Task<bool> updateSong(Stream fileStream, String fileName) {
            songLabel.Text = "";
            newSongButton.Text = "Loading...";
            newSongButton.Enabled = false;


            //var response = await _client.getSongSummary(fileStream, fileName);
            var response = await _client.getSongSummary("TRQHMGQ151DB1474E2");

            if (response != null)
            {
                var analysis = await _client.getAnalysis(response.analysis_url);

                songLabel.Text = fileName;
                newSongButton.Text = "Pick Song";
                newSongButton.Enabled = true;

                _visualizer.setInfo(analysis);
                _player = new Player(fileStream, analysis.beats, this);
                return true;
            }
            
            return false;
        }
    }
}
