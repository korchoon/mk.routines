using System;
using System.Diagnostics;

namespace JetBrains.Annotations
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
	[Conditional("JETBRAINS_ANNOTATIONS")]
	public sealed class NoReorderAttribute : Attribute
	{
		public NoReorderAttribute()
		{
		}
	}
}