﻿using System;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Engine.Events;
using SiliconStudio.Xenko.Physics;

namespace FirstPersonShooter.Player
{
    public class PlayerController : SyncScript
    {
        [Display("Run Speed")]
        public float MaxRunSpeed { get; set; } = 5;

        // This component is the physics representation of a controllable character
        private CharacterComponent character;

        private readonly EventReceiver<Vector3> moveDirectionEvent = new EventReceiver<Vector3>(PlayerInput.MoveDirectionEventKey);

        public static readonly EventKey<float>  RunSpeedEventKey = new EventKey<float>();

        /// <summary>
        /// Called when the script is first initialized
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Will search for an CharacterComponent within the same entity as this script
            character = Entity.Get<CharacterComponent>();
            if (character == null) throw new ArgumentException("Please add a CharacterComponent to the entity containing PlayerController!");
        }
        
        /// <summary>
        /// Called on every frame update
        /// </summary>
        public override void Update()
        {
            Move(MaxRunSpeed);
        }

        private void Move(float speed)
        {
            // Use the delta time from physics
            var dt = this.GetSimulation().FixedTimeStep;

            // Character speed
            Vector3 moveDirection;
            moveDirectionEvent.TryReceive(out moveDirection);

            character.Move(moveDirection * speed * dt);

            // Broadcast normalized speed
            RunSpeedEventKey.Broadcast(moveDirection.Length());
        }
    }
}
