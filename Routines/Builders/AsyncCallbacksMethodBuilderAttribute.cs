#if false
     // ----------------------------------------------------------------------------
// Async Reactors framework https://github.com/korchoon/async-reactors
// Copyright (c) 2016-2019 Mikhail Korchun <korchoon@gmail.com>
// ----------------------------------------------------------------------------

//namespace System.Runtime.CompilerServices

namespace System.Runtime.CompilerServices {
	public sealed class AsyncMethodBuilderAttribute : Attribute {
		public AsyncMethodBuilderAttribute(Type builderType) {
			BuilderType = builderType;
		}

		public Type BuilderType { get; }
	}
}
#endif