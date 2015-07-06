using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDL2;

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

        public override int MapJoystickIndex (int index) {
            return index;
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

        public override void glUniform4fv (
            OpenGLDevice openGLDevice, int _location, float[] _buffer
        ) {
            fixed (float* pBuffer = _buffer) {
                openGLDevice.glUniform4fv(
                    _location, _buffer.Length / 4,
                    pBuffer
                );
            }
        }

        public override MonoGameJoystickConfig CreateJoystickConfig (string osConfigFile) {
            var result = new MonoGameJoystickConfig();
            SaveJoystickConfig(result, osConfigFile);
            return result;
        }

        private static void SaveJoystickConfig (MonoGameJoystickConfig config, string osConfigFile) {
            // ... but is our directory even there?
			string osConfigDir = osConfigFile.Substring(0, osConfigFile.IndexOf("MonoGameJoystick.cfg"));
			if (!String.IsNullOrEmpty(osConfigDir) && !Directory.Exists(osConfigDir))
			{
				// Okay, jeez, we're really starting fresh.
				Directory.CreateDirectory(osConfigDir);
			}

			// Now, create the file.
			using (FileStream fileOut = File.Open(osConfigFile, FileMode.OpenOrCreate))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(MonoGameJoystickConfig));
				serializer.Serialize(fileOut, config);
			}
        }

        public override void InnerLoop (Func<bool> innerLoopTick, Action exit) {
            bool running = true;

            while (running = innerLoopTick())
                ;

            exit();
        }

        public override string GetTitleContainerLocation () {
			return AppDomain.CurrentDomain.BaseDirectory;
        }

        public override string GetResolvedLocalPath (string filePath, string relativeFile) {
            // Get a uri for filePath using the file:// schema and no host.
            var src = new Uri("file://" + filePath);

            var dst = new Uri(src, relativeFile);

            // The uri now contains the path to the relativeFile with 
            // relative addresses resolved... get the local path.
            return dst.LocalPath;
        }

        public override bool TrySleep (int durationMs) {
            // FIXME: I'm pretty sure durationMs can be 0 here, in which case 
            //  we should return false to stop spinning
            Thread.Sleep(durationMs);
            return true;
        }

        public override string GetSDLPlatform () {
            return SDL.SDL_GetPlatform();
        }
    }
}
