using JSIL;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Fna.Platform {
    internal class JSILPlatform : FnaPlatform {
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

/*
            //dynamic Document = Builtins.Global["document"];
            //dynamic Canvas = Document.getElementById("canvas");
            //dynamic gl = Canvas.getContext("webgl");
            //gl.uniform4fv(_location, _buffer.Length / 16, (float*)_buffer);
            float[] floatArray = new float[_buffer.Length / 4];
            Buffer.BlockCopy(_buffer, 0, floatArray, 0, _buffer.Length);
            device.GLDevice.glUniform4fv(
                _location,
                floatArray.Length / 4,
                floatArray
            );              
 */
    }
}
