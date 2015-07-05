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
#endregion

namespace Microsoft.Xna.Framework.Media
{
	public sealed class Song : IEquatable<Song>, IDisposable
	{
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

		internal float Volume
		{
			get
			{
				return soundInstance.Volume;
			}
			set
			{
				soundInstance.Volume = value;
			}
		}

		#endregion

		#region Private Variables

        private SoundEffect         sound;
        private SoundEffectInstance soundInstance;

		#endregion

		#region Constructors, Deconstructor, Dispose()

		internal Song(string fileName)
		{
			Name = Path.GetFileNameWithoutExtension(fileName);
			TrackNumber = 0;
			Position = TimeSpan.Zero;
			IsDisposed = false;

            sound = JSIL.FNAHelpers.DecodeSong(fileName);
            Duration = sound.Duration;
            soundInstance = sound.CreateInstance();
		}

		internal Song(string fileName, int durationMS) : this(fileName)
		{
			// HACK: We should check the duration here for a match but
            //  browser audio decoding is gonna be ~a nightmaaaaaaaaare~
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
				soundInstance.Dispose();
                sound.Dispose();
                soundInstance = null;
                sound = null;
			}
			IsDisposed = true;
		}

		#endregion

		#region Internal Playback Methods

		internal void Play()
		{
			soundInstance.Play();

			PlayCount += 1;
		}

		internal void Resume()
		{
			soundInstance.Resume();
		}

		internal void Pause()
		{
			soundInstance.Pause();
		}

		internal void Stop()
		{
			soundInstance.Stop();

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