using JSIL;
using JSIL.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Fna.Platform {
    internal unsafe class JSILPlatform : FnaPlatform {
        public override string ToString () {
            return "JSIL";
        }

        public override bool IsFeatureSupported (string featureName) {
            // FIXME
            return false;
        }

        public override string GetDefaultWindowTitle () {
            return Verbatim.Expression<string>("document.title");
        }

        private dynamic GetWebGLContext () {
            return Builtins.Global["document"].getElementById("canvas").getContext("webgl");
        }

        public override void BufferSubData (
            OpenGLDevice device,
            OpenGLDevice.GLenum buffer, bool discard, 
            OpenGLDevice.IOpenGLBuffer handle, 
            int elementSizeInBytes, int offsetInBytes, 
            Array data, int startIndex, int elementCount
        ) {
            // FIXME
            throw new NotImplementedException();
        }

        public override void glUniform4fv (
            OpenGLDevice openGLDevice, int _location, byte[] _buffer
        ) {
            // FIXME: Blech, copy into the emscripten heap
            using (var packed = new NativePackedArray<float>("sdl2.dll", _buffer.Length / 4))
            fixed (float * pPacked = packed.Array) {
                Buffer.BlockCopy(_buffer, 0, packed.Array, 0, _buffer.Length);
                openGLDevice.glUniform4fv(_location, packed.Length / 4, pPacked);
            }
        }
    }
}
