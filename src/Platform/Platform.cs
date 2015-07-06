using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fna {
    internal abstract class FnaPlatform {
        public static readonly FnaPlatform Platform;

        static FnaPlatform () {
            if (JSIL.Builtins.IsJavascript)
                Platform = new Platform.JSILPlatform();
            else
                Platform = new Platform.NativePlatform();
        }

        public void AssertSupported (string featureName) {
            if (!IsFeatureSupported(featureName))
                throw new NotSupportedException("Platform '" + this.ToString() + "' does not support feature '" + featureName + "'");
        }

        public abstract string GetDefaultWindowTitle ();
        public abstract bool   IsFeatureSupported (string featureName);
        public abstract int    MapJoystickIndex (int index);

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

        public abstract MonoGameJoystickConfig CreateJoystickConfig (string osConfigFile);

        public abstract void   InnerLoop (Func<bool> innerLoopTick, Action exit);
        public abstract string GetTitleContainerLocation ();
        public abstract string GetResolvedLocalPath (string filePath, string relativeFile);
        public abstract bool   TrySleep (int durationMs);
    }
}
