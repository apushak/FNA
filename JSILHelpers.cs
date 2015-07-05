using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace JSIL {
    // these are currently implemented in SampleFNA proxies
    internal static class FNAHelpers {
        internal static void BufferSubData (string p, int elementSizeInBytes, int offsetInBytes, Array data, int startIndex, int elementCount) {
            throw new NotImplementedException();
        }

        internal static SoundEffect DecodeSong (string filename) {
            throw new NotImplementedException();
        }
    }
}