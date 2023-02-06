using System.Globalization;
using System.Text.RegularExpressions;

namespace System;

/// <summary>
/// Extensiones de texto
/// </summary>
public static class StringExtensions
{
	/// <summary>
	/// Compara 2 cadenas de string ignorando mayus/minus. Opcionalmente, puede decir que elimine los espacios iniciales y finales de las cadenas a comparar
	/// </summary>
	/// <param name="value">Valor inicial.</param>
	/// <param name="compare">Valor comparable.</param>
	/// <param name="trim">Indica si eliminará espacios al inicio y al final de ambos string al comparar.</param>
	/// <returns>Si son iguales o no.</returns>
	public static bool EqualsIgnoreCase(this string value, string compare, bool trim = false)
	{
		if (trim)
			return string.Equals(value?.Trim(), compare?.Trim(), StringComparison.OrdinalIgnoreCase);

		return string.Equals(value, compare, StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Elimina los espacios entre palabras y los limita a uno
	/// </summary>
	/// <param name="line"></param>
	/// <returns></returns>
	public static string TrimSpacesBetweenString(this string line)
	{
		var regex = new Regex(@"[ ]{2,}", RegexOptions.None);
		line = regex.Replace(line, @" ").Trim();
		return line;
	}

	/// <summary>
	/// Corta una cadena aunque no cumpla con la longitud máxima
	/// </summary>
	/// <param name="value"></param>
	/// <param name="startIndex"></param>
	/// <param name="length"></param>
	/// <returns></returns>
	public static string Truncate(this string value, int startIndex, int length)
	{
		if (string.IsNullOrEmpty(value)) return value;
		if (value.Length > length) return value.Substring(startIndex, length);
		return value.Substring(startIndex);
	}

	/// <summary>
	/// Recorta la cadena si supera el tamaño pasado como parámetro. Usa de marcador los 3 puntos suspensivos
	/// Ejemplo si ejecutamos: "Este es un texto demasiado largo".FitTextToLength(10) nos devolverá: "Este es..."
	/// </summary>
	/// <param name="source"></param>
	/// <param name="maxLength"></param>
	/// <returns></returns>
	public static string FitTextToLength(this string source, int maxLength)
	{
		return source.FitTextToLength(maxLength, "...");
	}

	/// <summary>
	/// Recorta la cadena si supera el tamaño pasado como parámetro. Usa de marcador los 3 puntos suspensivos
	/// Ejemplo si ejecutamos: "Este es un texto demasiado largo".FitTextToLength(10) nos devolverá: "Este es..."
	/// </summary>
	/// <param name="source"></param>
	/// <param name="maxLength"></param>
	/// <param name="cutMarker"></param>
	/// <returns></returns>
	public static string FitTextToLength(this string source, int maxLength, string cutMarker)
	{
		if (source == null || (maxLength <= cutMarker.Length)) return source;
		return (source.Length > maxLength) ? source.Substring(0, maxLength - cutMarker.Length) + cutMarker : source;
	}

	/// <summary>
	/// Convierte El Primer Carácter De Cada Palabra De La Cadena A Mayúscula (Teniendo en cuenta palabras con todo mayúsculas)
	/// Ejemplo: "esTo eS UNA pruEBa".Capitalize(); -> "Esto Es UNA Prueba"
	/// </summary>
	/// <param name="source"></param>
	/// <returns></returns>
	public static string Capitalize(this string source)
	{
		return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(source);
	}

	/// <summary>
	/// Validacion de email
	/// </summary>
	/// <param name="email"></param>
	/// <returns></returns>
	public static bool IsValidEmail(this string email)
	{
		if (string.IsNullOrWhiteSpace(email))
			return false;

		try
		{
			// Normalize the domain
			email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

			// Examines the domain part of the email and normalizes it.
			string DomainMapper(Match match)
			{
				// Use IdnMapping class to convert Unicode domain names.
				var idn = new IdnMapping();

				// Pull out and process domain name (throws ArgumentException on invalid)
				var domainName = idn.GetAscii(match.Groups[2].Value);

				return match.Groups[1].Value + domainName;
			}
		}
		catch (RegexMatchTimeoutException)
		{
			return false;
		}
		catch (ArgumentException)
		{
			return false;
		}

		try
		{
			return Regex.IsMatch(email,
				@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
				@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
				RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
		}
		catch (RegexMatchTimeoutException)
		{
			return false;
		}
	}

	/// <summary>
	/// Indicates whether the specified string is null or an empty string ("").
	/// </summary>
	/// <param name="s">The string to test.</param>
	/// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
	public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

	/// <summary>
	/// Indicates whether a specified string is null, empty, or consists only of white-space characters.
	/// </summary>
	/// <param name="s">The string to test.</param>
	/// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
	public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s);

	/// <summary>
	/// Indicates whether a specified string is null, empty, and consists only of white-space characters.
	/// </summary>
	/// <param name="s">The string to test.</param>
	/// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
	public static bool IsNullOrEmptyAndWhiteSpace(this string s) => string.IsNullOrEmpty(s) && string.IsNullOrWhiteSpace(s);

	/// <summary>
	/// Indicates whether the specified string is not null and not empty string ("").
	/// </summary>
	/// <param name="s">The string to test.</param>
	/// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
	public static bool IsNotNullOrEmpty(this string s) => !string.IsNullOrEmpty(s);

	/// <summary>
	/// Indicates whether a specified string is not null, empty, and consists not only of white-space characters.
	/// </summary>
	/// <param name="s">The string to test.</param>
	/// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
	public static bool IsNotNullOrWhiteSpace(this string s) => !string.IsNullOrWhiteSpace(s);

	/// <summary>
	/// Indicates whether a specified string is not null, empty, and consists not only of white-space characters.
	/// </summary>
	/// <param name="s">The string to test.</param>
	/// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
	public static bool IsNotNullOrEmptyAndWhiteSpace(this string s) => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s);
}
