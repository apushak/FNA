using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Fna.Platform {
    internal class NativePlatform : FnaPlatform {
        public override string ToString () {
            return "Native SDL2";
        }

        public override bool IsFeatureSupported (string featureName) {
            switch (featureName.ToLowerInvariant()) {
                case "adpcm":
                    return true;

                default:
                    return false;
            }
        }

        public override string GetDefaultWindowTitle () {
            return MonoGame.Utilities.AssemblyHelper.GetDefaultWindowTitle();
        }

        public override void BufferSubData (
            OpenGLDevice device,
            OpenGLDevice.GLenum buffer, bool discard, 
            OpenGLDevice.IOpenGLBuffer handle, 
            int elementSizeInBytes, int offsetInBytes, 
            Array data, int startIndex, int elementCount
        ) {
            if (discard) {
				device.glBufferData(
					OpenGLDevice.GLenum.GL_ELEMENT_ARRAY_BUFFER,
					(IntPtr)handle.BufferSize,
					IntPtr.Zero,
					handle.Dynamic
				);
			}

			GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);

			device.glBufferSubData(
				OpenGLDevice.GLenum.GL_ELEMENT_ARRAY_BUFFER,
				offsetInBytes,
				elementSizeInBytes * elementCount,
				(IntPtr) (dataHandle.AddrOfPinnedObject().ToInt64() + startIndex * elementSizeInBytes)
			);

			dataHandle.Free();
        }

        /*
#else
            fixed (byte* bytePtr = _buffer)
            {
                // TODO: We need to know the type of buffer float/int/bool
                // and cast this correctly... else it doesn't work as i guess
                // GL is checking the type of the uniform.

#if SDL2
                float[] floatArray = new float[_buffer.Length / 4];
                Buffer.BlockCopy(_buffer, 0, floatArray, 0, _buffer.Length);
                Buffer.BlockCopy(_buffer, 0, floatArray, 0, _buffer.Length);
                device.GLDevice.glUniform4fv(
                    _location,
                    floatArray.Length / 4,
                    floatArray
                );   
#else
                GL.Uniform4(_location, _buffer.Length / 16, (float*)bytePtr);
#endif
            }
         */
    }
}
