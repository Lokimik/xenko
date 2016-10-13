﻿// Copyright (C) 2014-2016 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed as part of the Xenko Game Studio Samples
// Detailed license can be found at: http://xenko.com/legal/eula/

using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Input;

namespace FirstPersonShooter.Core
{
    public static class InputManagerExtensions
    {
        public static bool IsGamePadButtonDown(this InputManager input, GamePadButton button, int index)
        {
            if (input.GamePadCount < index)
                return false;

            return (input.GetGamePad(index).Buttons & button) == button;
        }

        public static Vector2 GetLeftThumb(this InputManager input, int index)
        {
            return input.GamePadCount >= index ? input.GetGamePad(index).LeftThumb : Vector2.Zero;
        }

        public static Vector2 GetRightThumb(this InputManager input, int index)
        {
            return input.GamePadCount >= index ? input.GetGamePad(index).RightThumb : Vector2.Zero;
        }

        public static float GetLeftTrigger(this InputManager input, int index)
        {
            return input.GamePadCount >= index ? input.GetGamePad(index).LeftTrigger : 0.0f;
        }

        public static float GetRightTrigger(this InputManager input, int index)
        {
            return input.GamePadCount >= index ? input.GetGamePad(index).RightTrigger : 0.0f;
        }
    }
}
