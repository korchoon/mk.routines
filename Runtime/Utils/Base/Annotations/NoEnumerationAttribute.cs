using System;
using System.Diagnostics;

namespace JetBrains.Annotations
{
	[AttributeUsage(AttributeTargets.Parameter)]
	[Conditional("JETBRAINS_ANNOTATIONS")]
	public sealed class NoEnumerationAttribute : Attribute
	{
		public NoEnumerationAttribute()
		{
		}
	}
}