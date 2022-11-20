using System.Globalization;

namespace System
{
	public static class DateTimeExtensions
	{
		#region Getters
		/// <summary>
		/// Obtiene el día de inicio de la semana actual
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="startOfWeek">Día en el que empieza la semana</param>
		/// <returns></returns>
		public static DateTime BeginOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
		{
			int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
			return dt.AddDays(-1 * diff).Date;
		}

		/// <summary>
		/// Obtiene el día de fin de la semana actual
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static DateTime EndOfWeek(this DateTime dt) => dt.BeginOfWeek().AddDays(6).Date;

		/// <summary>
		/// Returns datetime corresponding to first day of the month
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime BeginOfMonth(this DateTime date) => new(date.Year, date.Month, 1);
		/// <summary>
		/// Returns datetime corresponding to first day of the month
		/// </summary>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <returns></returns>
		public static DateTime BeginOfMonth(int year, int month) => new(year, month, 1);

		/// <summary>
		/// Returns datetime corresponding to last day of the month
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime EndOfMonth(this DateTime date) => new(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
		/// <summary>
		/// Returns datetime corresponding to last day of the month
		/// </summary>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <returns></returns>
		public static DateTime EndOfMonth(int year, int month) => new(year, month, DateTime.DaysInMonth(year, month));

		/// <summary>
		/// Returns datetime corresponding to first day of year
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime BeginOfYear(this DateTime date) => new(date.Year, 1, 1);

		/// <summary>
		/// Returns datetime corresponding to end of year
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime EndOfYear(this DateTime date) => new(date.Year, 12, 31);

		/// <summary>
		/// Obtiene el número de días para este año dependiendo de si es o no bisiesto
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static int NaturalDaysInYear(this DateTime date) => DateTime.IsLeapYear(date.Year) ? 366 : 365;

		/// <summary>
		/// Convierte una fecha a TimeSpan desde 1 de enero de 1970
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static long ToTimeSpan(this DateTime date)
		{
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
			var timespan = (date.Ticks - epoch.Ticks) / 10000000;
			return timespan;
		}

		/// <summary>
		/// Obtiene el siguiente día que sea el día de la semana especificado
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="day"></param>
		/// <returns></returns>
		public static DateTime GetNextWeekday(this DateTime dt, DayOfWeek day = DayOfWeek.Monday)
		{
			int daysToAdd = ((int)day - (int)dt.DayOfWeek + 7) % 7;
			return dt.AddDays(daysToAdd);
		}

		/// <summary>
		/// This presumes that weeks start with Monday.
		/// Week 1 is the 1st week of the year with a Thursday in it.
		/// </summary>
		public static int GetIso8601WeekOfYear(this DateTime time, CalendarWeekRule calendarWeekRule = CalendarWeekRule.FirstFourDayWeek, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
		{
			// Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
			// be the same week# as whatever Thursday, Friday or Saturday are,
			// and we always get those right
			DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
			if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
			{
				time = time.AddDays(3);
			}

			// Return the week of our adjusted day
			return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, calendarWeekRule, firstDayOfWeek);
		}
		#endregion

		#region Setters
		/// <summary>
		/// Establece el año para esta fecha
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public static DateTime SetYear(this DateTime dt, int newValue = 0)
		{
			var current = dt.Year;
			return dt.AddYears(-current).AddYears(newValue);
		}

		/// <summary>
		/// Establece el mes para esta fecha
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public static DateTime SetMonth(this DateTime dt, int newValue = 0)
		{
			var current = dt.Month;
			return dt.AddMonths(-current).AddMonths(newValue);
		}

		/// <summary>
		/// Establece el día para esta fecha
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public static DateTime SetDay(this DateTime dt, int newValue = 0)
		{
			var current = dt.Day;
			return dt.AddDays(-current).AddDays(newValue);
		}

		/// <summary>
		/// Establece la hora para esta fecha
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public static DateTime SetHour(this DateTime dt, int newValue = 0)
		{
			var current = dt.Hour;
			return dt.AddHours(-current).AddHours(newValue);
		}

		/// <summary>
		/// Establece el minuto para esta fecha
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public static DateTime SetMinute(this DateTime dt, int newValue = 0)
		{
			var current = dt.Minute;
			return dt.AddMinutes(-current).AddMinutes(newValue);
		}

		/// <summary>
		/// Establece el segundo para esta fecha
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public static DateTime SetSecond(this DateTime dt, int newValue = 0)
		{
			var current = dt.Second;
			return dt.AddSeconds(-current).AddSeconds(newValue);
		}

		/// <summary>
		/// Establece el milisegundo para esta fecha
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public static DateTime SetMillisecond(this DateTime dt, int newValue = 0)
		{
			var current = dt.Millisecond;
			return dt.AddMilliseconds(-current).AddMilliseconds(newValue);
		}
		#endregion

	}
}
