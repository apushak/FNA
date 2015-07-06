using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

#if false

namespace JSIL {
    // these are currently implemented in SampleFNA proxies
    public delegate void SongDecodeCompleteHandler (object audioBuffer);

    public static class FNAHelpers {
        internal static void BufferSubData (string p, int elementSizeInBytes, int offsetInBytes, Array data, int startIndex, int elementCount) {
            throw new NotImplementedException();
        }

        public static object GetALContext() {
            throw new NotImplementedException();
        }

        internal static void BeginDecodeSong (string filename, SongDecodeCompleteHandler onDecodeComplete) {
            throw new NotImplementedException();
        }

        internal static object PlaySong (object audioBuffer, float volume) {
            throw new NotImplementedException();
        }

        internal static void PauseSong (object audioBuffer, object playingSong) {
            throw new NotImplementedException();
        }

        internal static void ResumeSong (object audioBuffer, object playingSong) {
            throw new NotImplementedException();
        }

        internal static void StopSong (object audioBuffer, object playingSong) {
            throw new NotImplementedException();
        }

        internal static float GetSongLength (object audioBuffer) {
            throw new NotImplementedException();
        }

        internal static void SetSongVolume (object audioBuffer, object playingSong, float volume) {
            throw new NotImplementedException();
        }
    }
}

#endif