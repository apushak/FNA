using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace Fna {
    internal abstract class FnaPlatform {
        public static readonly FnaPlatform Platform;

        static FnaPlatform () {
            if (JSIL.Builtins.IsJavascript)
                Platform = new Platform.JSILPlatform();
            else
                Platform = new Platform.NativePlatform();
        }

        public abstract string GetDefaultWindowTitle ();

        public abstract bool IsFeatureSupported (string featureName);

        public void AssertSupported (string featureName) {
            if (!IsFeatureSupported(featureName))
                throw new NotSupportedException("Platform '" + this.ToString() + "' does not support feature '" + featureName + "'");
        }

        public abstract void BufferSubData (
            OpenGLDevice device,
            OpenGLDevice.GLenum buffer, bool discard, 
            OpenGLDevice.IOpenGLBuffer bufferHandle, 
            int elementSizeInBytes, int offsetInBytes, 
            Array data, int startIndex, int elementCount
        );

        public abstract void glUniform4fv (
            OpenGLDevice openGLDevice, int _location, byte[] _buffer
        );
    }
}
