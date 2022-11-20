namespace System.Reflection
{
	/// <summary>
	/// Opciones de la copia de propiedades
	/// </summary>
	public class CopyPropertiesOptions
	{
		/// <summary>
		/// Nombres completos de las propiedades/campos que serán excluidos de la copia
		/// </summary>
		public List<string> ExcludePropertiesNames { get; set; } = new List<string>();
		/// <summary>
		/// Profundidad máxima a la que se copiarán los objetos
		/// </summary>
		public int MaxCopyDepth { get; set; } = 128;
		/// <summary>
		/// Nombres parciales que empiecen por, de las propiedades/campos que serán excluidos de la copia si son clases.
		/// Ejemplo: Quiero excluir todas las clases que sean propiedades que empiecen por FK
		/// </summary>
		public List<string> ExcludeClassPropertiesNamePrefix { get; set; } = new List<string>();
		/// <summary>
		/// Nombres parciales que terminen por, de las propiedades/campos que serán excluidos de la copia si son clases.
		/// Ejemplo: Quiero excluir todas las clases que sean propiedades que terminen por Navigation
		/// </summary>
		public List<string> ExcludeClassPropertiesNameSuffix { get; set; } = new List<string>();
		/// <summary>
		/// Si debe seguir copiando en base a la profundidad
		/// </summary>
		internal Func<bool> CopyByDepth { get; set; }
	}
}