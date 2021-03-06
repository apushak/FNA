#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2014 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */
#endregion

#if JSIL

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Microsoft.Xna.Framework.Audio;
using JSIL;
#endregion

namespace Microsoft.Xna.Framework.Media
{
	public sealed class Song : IEquatable<Song>, IDisposable
	{
        private static class WebAudio {
            internal static object GetALContext() {
                return Verbatim.Expression("JSIL.PInvoke.GetModule('soft_oal.dll', true).OpenAL.currentContext.ctx");
            }

            internal static void BeginDecodeSong (string filename, Action<object> onDecodeComplete) {
                try {
                    dynamic stream = File.OpenRead(filename);
                    // HACK: JSIL stores the underlying byte array for the file as a property on the stream
                    dynamic fileByteArray = stream._buffer;
                    object fileArrayBuffer = fileByteArray.buffer;

                    dynamic al = GetALContext();

                    al.decodeAudioData(fileArrayBuffer, Verbatim.Expression(
                        "function (buffer) { System.Console.WriteLine('Song {0} decoding complete', $1); $0(buffer); }",
                        onDecodeComplete, filename
                    ));

                    Console.WriteLine("Song {0} decoding started", filename);
                } catch {
                    Console.WriteLine("Song {0} decoding failed", filename);
                    throw;
                }
            }

            internal static object PlaySong (object audioBuffer, float volume) {
                if (audioBuffer == null)
                    return null;

                dynamic al = GetALContext();

                var gain = al.createGain();
                gain.gain.value = volume;
                gain.connect(al.destination);

                var source = al.createBufferSource();
                source.buffer = audioBuffer;
                // FIXME
                source.loop = true;
                source.connect(gain);
                source.start();

                return JSIL.Verbatim.Expression(
                    "{ source: $0, gain: $1 }",
                    source,
                    gain
                );
            }

            internal static void PauseSong (object audioBuffer, object playingSong) {
                if ((audioBuffer == null) || (playingSong == null))
                    return;

                throw new NotImplementedException("Web Audio API cannot pause/resume");
            }

            internal static void ResumeSong (object audioBuffer, object playingSong) {
                if ((audioBuffer == null) || (playingSong == null))
                    return;

                throw new NotImplementedException("Web Audio API cannot pause/resume");
            }

            internal static void StopSong (object audioBuffer, object playingSong) {
                if ((audioBuffer == null) || (playingSong == null))
                    return;

                dynamic ps = playingSong;
                ps.source.stop();
            }

            internal static float GetSongLength (object audioBuffer) {
                if (audioBuffer == null)
                    return 0.0f;

                dynamic ab = audioBuffer;
                return (float)(ab.length);
            }

            internal static void SetSongVolume (object audioBuffer, object playingSong, float volume) {
                if ((audioBuffer == null) || (playingSong == null))
                    return;

                dynamic ps = playingSong;
                ps.gain.gain.value = volume;
            }
        }

		#region Public Metadata Properties

		// TODO: vorbis_comment TITLE
		public string Name
		{
			get;
			private set;
		}

		// TODO: vorbis_comment TRACKNUMBER
		public int TrackNumber
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the Album on which the Song appears.
		/// </summary>
		// TODO: vorbis_comment ALBUM
		public Album Album
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the Artist of the Song.
		/// </summary>
		// TODO: vorbis_comment ARTIST
		public Artist Artist
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the Genre of the Song.
		/// </summary>
		// TODO: vorbis_comment GENRE
		public Genre Genre
		{
			get
			{
				return null;
			}
		}

		#endregion

		#region Public Stream Properties

		public TimeSpan Duration
		{
			get;
			private set;
		}

		#endregion

		#region Public MediaPlayer Properties

		public bool IsProtected
		{
			get
			{
				return false;
			}
		}

		public bool IsRated
		{
			get
			{
				return false;
			}
		}

		public int PlayCount
		{
			get;
			private set;
		}

		public int Rating
		{
			get
			{
				return 0;
			}
		}

		#endregion

		#region Public IDisposable Properties

		public bool IsDisposed
		{
			get;
			private set;
		}

		#endregion

		#region Internal Properties

        // TODO: Implement this
		internal TimeSpan Position
		{
			get;
			private set;
		}

        internal float _Volume = 1.0f;
		internal float Volume
		{
			get
			{
				return _Volume;
			}
			set
			{
				_Volume = value;
                WebAudio.SetSongVolume(audioBuffer, playingSong, _Volume);
			}
		}

		#endregion

		#region Private Variables

        private object audioBuffer;
        private object playingSong;
        private bool   decodeIsPending;
        private bool   playIsPending;

		#endregion

		#region Constructors, Deconstructor, Dispose()

		internal Song(string fileName)
		{
			Name = Path.GetFileNameWithoutExtension(fileName);
			TrackNumber = 0;
			Position = TimeSpan.Zero;
			IsDisposed = false;

            decodeIsPending = true;
            // FIXME: Blech, asynchrony
            WebAudio.BeginDecodeSong(fileName, OnDecodeComplete);
		}

        private void OnDecodeComplete (object audioBuffer) {
            this.audioBuffer = audioBuffer;
            this.decodeIsPending = false;

            Duration = TimeSpan.FromSeconds(WebAudio.GetSongLength(audioBuffer));

            if (this.playIsPending)
                Play();
        }

		internal Song(string fileName, int durationMS) : this(fileName)
		{
			// HACK: Pre-initialize with the duration from the XNB file, so that we
            //  have a roughly accurate duration while the decode happens
            Duration = TimeSpan.FromMilliseconds(durationMS);
		}

		~Song()
		{
			Dispose(true);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (disposing)
			{
				Stop();
                audioBuffer = null;
                playIsPending = false;
			}
			IsDisposed = true;
		}

		#endregion

		#region Internal Playback Methods

		internal void Play()
		{
            // FIXME
            if (playingSong != null)
                return;

            if (audioBuffer == null) {
                playIsPending = true;
                return;
            }

            playingSong = WebAudio.PlaySong(audioBuffer, _Volume);
            playIsPending = false;
    		PlayCount += 1;
		}

		internal void Resume()
		{
            WebAudio.ResumeSong(audioBuffer, playingSong);
		}

		internal void Pause()
		{
            WebAudio.PauseSong(audioBuffer, playingSong);
		}

		internal void Stop()
		{
            WebAudio.StopSong(audioBuffer, playingSong);

			PlayCount = 0;
		}

		#endregion

		#region Internal Event Handler Methods

		internal void OnFinishedPlaying()
		{
			MediaPlayer.OnSongFinishedPlaying(null, null);
		}

		#endregion

		#region Public Comparison Methods/Operators

		public bool Equals(Song song) 
		{
			return (((object) song) != null) && (Name == song.Name);
		}

		public override bool Equals(Object obj)
		{
			if (obj == null)
			{
				return false;
			}
			return Equals(obj as Song);
		}

		public static bool operator ==(Song song1, Song song2)
		{
			if (((object) song1) == null)
			{
				return ((object) song2) == null;
			}
			return song1.Equals(song2);
		}

		public static bool operator !=(Song song1, Song song2)
		{
			return !(song1 == song2);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Constructs a new Song object based on the specified URI.
		/// </summary>
		/// <remarks>
		/// This method matches the signature of the one in XNA4, however we currently can't play remote songs, so
		/// the URI is required to be a file name and the code uses the LocalPath property only.
		/// </remarks>
		/// <param name="name">Name of the song.</param>
		/// <param name="uri">Uri object that represents the URI.</param>
		/// <returns>Song object that can be used to play the song.</returns>
		public static Song FromUri(string name, Uri uri)
		{
			if (!uri.IsFile)
			{
				throw new InvalidOperationException("Only local file URIs are supported for now");
			}

			return new Song(uri.LocalPath)
			{
				Name = name
			};
		}

		#endregion
	}
}

#endif