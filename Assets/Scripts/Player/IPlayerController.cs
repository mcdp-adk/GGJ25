using UnityEngine;
using System;

public interface IPlayerController
{
    Vector2 FrameInput { get; }
    Vector2 Velocity { get; }
    event Action Jumped;
    event Action<bool, float> GroundedChanged;
} 