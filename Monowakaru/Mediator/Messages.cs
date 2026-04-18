using System;
using System.Collections.Generic;
using System.Numerics;
using DalaMock.Host.Mediator;
using Monowakaru.Services;

namespace Monowakaru.Mediator;

/// <summary>
/// Request that a window is toggled.
/// </summary>
/// <param name="WindowType">The type of the window.</param>
public record ToggleWindowMessage(Type WindowType) : MessageBase;

/// <summary>
/// Request that a window is closed.
/// </summary>
/// <param name="WindowType">The type of the window.</param>
public record OpenWindowMessage(Type WindowType) : MessageBase;

/// <summary>
/// Request that a window is closed.
/// </summary>
/// <param name="WindowType">The type of the window.</param>
public record CloseWindowMessage(Type WindowType) : MessageBase;

/// <summary>
///     Published when a text capture is completed.
/// </summary>
public record TextCaptureResultMessage(IReadOnlyList<TextCapture> Captures, Vector2 MousePosition) : MessageBase;
