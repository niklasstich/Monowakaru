using System.ComponentModel;

// We include this to make records work in .NETStandard 2.1

// ReSharper disable once CheckNamespace - must be this namespace
namespace System.Runtime.CompilerServices;

/// <summary>
///     Reserved to be used by the compiler for tracking metadata.
///     This class should not be used by developers in source code.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class IsExternalInit
{
}