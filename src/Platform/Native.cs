using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Fna.Platform {
    internal unsafe class NativePlatform : FnaPlatform {
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

        public override void glUniform4fv (
            OpenGLDevice openGLDevice, int _location, byte[] _buffer
        ) {
            fixed (byte* pBuffer = _buffer) {
                openGLDevice.glUniform4fv(
                    _location, _buffer.Length / 16,
                    (float*)pBuffer
                );
            }
        }
    }
}
