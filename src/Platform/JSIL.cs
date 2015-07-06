using JSIL;
using JSIL.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Utilities;
using System.IO;

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

        public override int MapJoystickIndex (int index) {
            // HACK: To compensate for bug in emscripten SDL2
            if (index == -1)
                return 0;
            else
                return index;
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

        public override MonoGameJoystickConfig CreateJoystickConfig (string osConfigFile) {
            // http://www.w3.org/TR/gamepad/#remapping
            return new MonoGameJoystickConfig {
                BUTTON_A = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 0
                },
                BUTTON_B = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 1
                },
                BUTTON_X = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 2
                },
                BUTTON_Y = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 3
                },

                SHOULDER_LB = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 4
                },
                SHOULDER_RB = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 5
                },

                // FIXME: How can we make the analog work here?
                TRIGGER_LT = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 6
                },
                TRIGGER_RT = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 7
                },

                BUTTON_BACK = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 8
                },
                BUTTON_START = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 9
                },

                BUTTON_LSTICK = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 10
                },
                BUTTON_RSTICK = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 11
                },

                DPAD_UP = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 12
                },
                DPAD_DOWN = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 13
                },
                DPAD_LEFT = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 14
                },
                DPAD_RIGHT = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Button,
                    INPUT_ID = 15
                },

                AXIS_LX = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Axis,
                    INPUT_ID = 0
                },
                AXIS_LY = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Axis,
                    INPUT_ID = 1
                    // FIXME: Invert Y?
                },

                AXIS_RX = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Axis,
                    INPUT_ID = 2
                },
                AXIS_RY = new MonoGameJoystickValue {
                    INPUT_TYPE = InputType.Axis,
                    INPUT_ID = 3
                    // FIXME: Invert Y?
                },
            };
        }

        private class InnerLoopContext {
            public bool IsRunning;
            public Action<Action> RequestAnimationFrame;
            public Func<bool>     InnerLoopTick;
            public Action         Exit;
            private Action        _RunNextTick;

            public InnerLoopContext (
                Func<bool> innerLoopTick, Action exit
            ) {
                _RunNextTick = RunNextTick;
                RequestAnimationFrame = Verbatim.Expression<Action<Action>>("window.requestAnimationFrame");
                InnerLoopTick = innerLoopTick;
                Exit = exit;
            }

            public void RunNextTick () {
                IsRunning = InnerLoopTick();
                if (IsRunning)
                    RequestAnimationFrame(_RunNextTick);
                else
                    Exit();
            }
        }

        public override void InnerLoop (Func<bool> innerLoopTick, Action exit) {
            var context = new InnerLoopContext(
                innerLoopTick, exit
            );

            context.RunNextTick();
        }

        public override string GetTitleContainerLocation () {
            // FIXME: Pull the appropriate location out of jsilConfig?
            return "/bin/JSIL";
        }

        public override string GetResolvedLocalPath (string filePath, string relativeFile) {
            // FIXME: JSIL does not implement Uri. Do what we can.
            var parentDirectory = Path.GetDirectoryName(filePath);

            relativeFile = relativeFile.Replace(FileHelpers.BackwardSlash, FileHelpers.ForwardSlash);
            while (relativeFile.StartsWith("../")) {
                parentDirectory = Path.GetDirectoryName(parentDirectory) ?? "";
                relativeFile = relativeFile.Substring(3);
            }

            return Path.Combine(parentDirectory, relativeFile);
        }

        public override bool TrySleep (int durationMs) {
            return false;
        }
    }
}
