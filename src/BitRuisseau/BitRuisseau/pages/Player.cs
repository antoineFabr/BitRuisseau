using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using BitRuisseau.data;
using NAudio.Wave;

namespace BitRuisseau
{
    public partial class Player : Form
    {
        private Song currentSong;
        private bool isPlaying = false;
        private AudioFileReader audioFile;
        private WaveOutEvent outputDevice;

        public Player(Song song)
        {
            InitializeComponent();
            currentSong = song;
            this.Text = $"Player - {currentSong.Title}";
            lblTitle.Text = $"{currentSong.Artist} - {currentSong.Title}";
            PlayMusic(currentSong.Path);
            isPlaying = true;
            btnPlayPause.Text = "Pause";
            

        }

        private void PlayMusic(string path)
        {
            try
            {
                outputDevice = new WaveOutEvent();
                audioFile = new AudioFileReader(path);
                outputDevice.Init(audioFile);
                outputDevice.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lecture : {ex.Message}");
            }
        }

        private void StopMusic()
        {
            outputDevice.Stop();
            isPlaying = false;
            btnPlayPause.Text = "Play";
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                outputDevice.Pause();
                btnPlayPause.Text = "Play";
                isPlaying = false;
            }
            else
            {
                outputDevice.Play();
                btnPlayPause.Text = "Pause";
                isPlaying = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
             StopMusic();
             this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
             StopMusic();
             base.OnFormClosing(e);
        }
    }
}
