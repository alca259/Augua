namespace System.Threading
{
	/// <summary>
	/// Semáforo con clave
	/// </summary>
	/// https://stackoverflow.com/questions/31138179/asynchronous-locking-based-on-a-key
	public sealed class SemaphoreSlimKey
	{
		/// <summary>
		/// Listado actual de semáforos
		/// </summary>
		private static readonly Dictionary<object, RefCounted<SemaphoreSlim>> _semaphores = new ();

		private int InitialCount { get; }
		private int MaxCount { get; }

		#region Constructor
		/// <summary>
		/// Semáforo con clave, initial count: 1 / max count: 1
		/// </summary>
		public SemaphoreSlimKey()
		{
			InitialCount = 1;
			MaxCount = 1;
		}

		/// <summary>
		/// Semáforo con clave
		/// </summary>
		/// <param name="initialCount">The initial number of requests for the semaphore that can be granted concurrently.</param>
		/// <param name="maxCount">The maximum number of requests for the semaphore that can be granted concurrently.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">initialCount is less than 0, or initialCount is greater than maxCount, or maxCount is equal to or less than 0.
		/// </exception>
		public SemaphoreSlimKey(uint initialCount, uint maxCount)
		{
			InitialCount = (int)initialCount;
			MaxCount = (int)maxCount;
		}
		#endregion

		#region Métodos privados
		/// <summary>
		/// Obtiene o crea un nuevo semáforo basado en clave
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private SemaphoreSlim GetOrCreate(object key)
		{
			RefCounted<SemaphoreSlim> item;
			lock (_semaphores)
			{
				if (_semaphores.TryGetValue(key, out item))
				{
					++item.RefCount;
				}
				else
				{
					item = new RefCounted<SemaphoreSlim>(new SemaphoreSlim(InitialCount, MaxCount));
					_semaphores[key] = item;
				}
			}
			return item.Value;
		}
		#endregion

		#region Métodos públicos
		/// <summary>
		/// Si la clave ya existe bloqueará el nuevo proceso hasta que éste sea liberado
		/// Liberar cuando no se necesite.
		/// </summary>
		/// <param name="key">Clave de bloqueo</param>
		/// <param name="timeout">Tiempo de espera máximo (5min defecto)</param>
		/// <param name="cancellationToken">Token de cancelación</param>
		/// <returns></returns>
		public IDisposable Lock(object key, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
		{
			var process = GetOrCreate(key);
			if (timeout == null) timeout = TimeSpan.FromMinutes(5);

			var releaser = new Releaser { Key = key };
			if (!process.Wait(timeout.Value, cancellationToken))
			{
				releaser.Dispose();
			}
			return releaser;
		}

		/// <summary>
		/// Si la clave ya existe bloqueará el nuevo proceso hasta que éste sea liberado
		/// Liberar cuando no se necesite.
		/// </summary>
		/// <param name="key">Clave de bloqueo</param>
		/// <param name="timeout">Tiempo de espera máximo (5min defecto)</param>
		/// <param name="cancellationToken">Token de cancelación</param>
		/// <returns></returns>
		public async Task<IDisposable> LockAsync(object key, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
		{
			var process = GetOrCreate(key);
			if (timeout == null) timeout = TimeSpan.FromMinutes(5);

			var releaser = new Releaser { Key = key };
			if (!await process.WaitAsync(timeout.Value, cancellationToken).ConfigureAwait(false))
			{
				releaser.Dispose();
			}
			return releaser;
		}

		/// <summary>
		/// Number of semaphores
		/// </summary>
		/// <returns></returns>
		public int Count()
		{
			lock (_semaphores)
			{
				return _semaphores.Count;
			}
		}

		/// <summary>
		/// Number of references of semaphores by key
		/// </summary>
		/// <returns></returns>
		public int Count(object key)
		{
			lock (_semaphores)
			{
				var elements = _semaphores.Where(c => c.Key == key).Select(s => s.Value).ToArray();
				if (elements.Length <= 0) return 0;

				return elements.Sum(c => c.RefCount);
			}
		}

		/// <summary>
		/// Release a semaphore by key
		/// </summary>
		/// <param name="key"></param>
		public void Release(object key)
		{
			lock (_semaphores)
			{
				if (!_semaphores.TryGetValue(key, out RefCounted<SemaphoreSlim> item)) return;

				--item.RefCount;
				if (item.RefCount == 0)
				{
					_semaphores.Remove(key);
				}

				item.Value.Release();
			}
		}

		/// <summary>
		/// Release all semaphores
		/// </summary>
		public void ReleaseAll()
		{
			lock (_semaphores)
			{
				var local = _semaphores.Values.ToArray();
				foreach (RefCounted<SemaphoreSlim> item in local)
				{
					item.Value.Release();
				}
				_semaphores.Clear();
			}
		}
		#endregion

		#region Clases privadas
		private sealed class RefCounted<T>
		{
			public RefCounted(T value)
			{
				RefCount = 1;
				Value = value;
			}

			public int RefCount { get; set; }
			public T Value { get; private set; }
		}

		private sealed class Releaser : IDisposable
		{
			public object Key { get; set; }

			public void Dispose()
			{
				RefCounted<SemaphoreSlim> item;
				lock (_semaphores)
				{
					if (!_semaphores.TryGetValue(Key, out item)) return;

					--item.RefCount;
					if (item.RefCount == 0)
					{
						_semaphores.Remove(Key);
					}
				}

				try
				{
					item.Value.Release();
				}
				catch (SemaphoreFullException)
				{
					// Ignore
				}
			}
		}
		#endregion
	}
}
