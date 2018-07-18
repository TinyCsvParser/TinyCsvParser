using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
    /// <summary>
    ///     This is just the ArrayMemoryPool from .NET Core,
    ///     except it slices the returned Memory to the exact size
    ///     requested, when ArrayPool returns something larger.
    ///     <para>NOTE: use the static "Instance" property instead of the static "Shared" property.
    ///     The Shared property always returns the base class and not SizedMemoryPool.</para>
    /// </summary>
    public sealed class SizedMemoryPool<T> : MemoryPool<T>
    {
        /// <summary>
        ///     Use this instead of the "Shared" static property, which returns an instance
        ///     of the base class and not SizedMemoryPool.
        /// </summary>
        public static MemoryPool<T> Instance { get; } = new SizedMemoryPool<T>();

        private const int MaximumBufferSize = int.MaxValue;

        public sealed override int MaxBufferSize => MaximumBufferSize;

        public sealed override IMemoryOwner<T> Rent(int minimumBufferSize = -1)
        {
            if (minimumBufferSize == -1)
                minimumBufferSize = 1 + (4095 / Unsafe.SizeOf<T>());
            else if (((uint)minimumBufferSize) > MaximumBufferSize)
                throw new ArgumentOutOfRangeException($"Minimum buffer size too large: {minimumBufferSize} > {MaximumBufferSize}");

            return new ArrayMemoryPoolBuffer(minimumBufferSize);
        }

        protected sealed override void Dispose(bool disposing) { }  // ArrayMemoryPool is a shared pool so Dispose() would be a nop even if there were native resources to dispose.

        private sealed class ArrayMemoryPoolBuffer : IMemoryOwner<T>
        {
            private T[] _array;
            private readonly int _size;

            public ArrayMemoryPoolBuffer(int size)
            {
                _size = size;
                _array = ArrayPool<T>.Shared.Rent(size);
            }

            public Memory<T> Memory
            {
                get
                {
                    T[] array = _array;
                    if (array == null)
                    {
                        throw new ObjectDisposedException("SizedMemoryPool");
                    }

                    var mem = new Memory<T>(array, 0, _size);
                    Debug.Assert(mem.Length == _size, "Memory length did not match size.");
                    return mem;
                }
            }

            public void Dispose()
            {
                T[] array = _array;
                if (array != null)
                {
                    _array = null;
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] is IDisposable disp)
                        {
                            disp.Dispose();
                        }
                    }
                    ArrayPool<T>.Shared.Return(array);
                }
            }
        }
    }
}
