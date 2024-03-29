﻿namespace System;

/// <summary>
/// Rangos de fechas
/// </summary>
public sealed class DateRange
{
    private DateTime UseStart => UseHours ? Start : Start.Date;
    private DateTime UseEnd => UseHours ? End : End.Date;

    public DateRange()
    {
        // Json endless constructor
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public DateRange(DateTime start, DateTime end)
    {
        if (start > end)
        {
            Diagnostics.Debug.WriteLine($"[WARN] End date is lower than start, matching end with start date...");
            end = start;
        }

        if (end < start)
        {
            Diagnostics.Debug.WriteLine($"[WARN] Start date is lower than end, matching start with end date...");
            start = end;
        }

        Start = start;
        End = end;
    }

    /// <summary>
    /// Constructor ampliado
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="useHours"></param>
    /// <param name="inclusiveStart">True por defecto</param>
    /// <param name="inclusiveEnd">True por defecto</param>
    public DateRange(DateTime start, DateTime end, bool useHours, bool inclusiveStart = true, bool inclusiveEnd = true) : this(start, end)
    {
        UseHours = useHours;
        InclusiveStart = inclusiveStart;
        InclusiveEnd = inclusiveEnd;
    }

    /// <summary>
    /// Fecha de inicio
    /// </summary>
    public DateTime Start { get; set; }

    /// <summary>
    /// Fecha de fin
    /// </summary>
    public DateTime End { get; set; }

    /// <summary>
    /// Si la fecha de inicio será tomada en cuenta
    /// </summary>
    public bool InclusiveStart { get; set; } = true;

    /// <summary>
    /// Si la fecha de fin será tomada en cuenta
    /// </summary>
    public bool InclusiveEnd { get; set; } = true;

    /// <summary>
    /// Si las horas serán tenidas en cuenta
    /// </summary>
    public bool UseHours { get; set; } = false;

    /// <summary>
    /// Timespan del rango
    /// </summary>
    public TimeSpan RangeSpan => End - Start;

    /// <summary>
    /// Devuelve los enumerados de fechas, en función a como se le ha especificado
    /// </summary>
    /// <param name="step"></param>
    /// <returns></returns>
    public IEnumerable<DateTime> Step(Func<DateTime, DateTime> step)
    {
        for (var dt = UseStart; dt <= UseEnd; dt = step(dt))
        {
            yield return dt;
        }
    }

    /// <summary>
    /// El rango del parámetro choca de cualquier forma con éste
    /// </summary>
    /// <param name="checkRange"></param>
    /// <returns></returns>
    public bool Collision(DateRange checkRange)
    {
        return Inside(checkRange)
            || StartInside(checkRange)
            || EndsInside(checkRange)
            || FullOverlap(checkRange)
            || StartInsideEndsOutside(checkRange)
            || StartOutsideEndsInside(checkRange);
    }

    /// <summary>
    /// El rango del parámetro está dentro de éste
    /// </summary>
    /// <param name="checkRange"></param>
    /// <returns></returns>
    public bool Inside(DateRange checkRange)
    {
        return Inside(checkRange.Start) && Inside(checkRange.End);
    }

    /// <summary>
    /// La fecha suministrada está dentro de éste rango
    /// </summary>
    /// <param name="checkDate"></param>
    /// <returns></returns>
    public bool Inside(DateTime checkDate)
    {
        var useCheckDate = UseHours ? checkDate : checkDate.Date;
        var condStart = InclusiveStart ? UseStart <= useCheckDate : UseStart < useCheckDate;
        var condEnd = InclusiveEnd ? useCheckDate <= UseEnd : useCheckDate < UseEnd;
        return condStart && condEnd;
    }

    /// <summary>
    /// El rango del parámetro empieza dentro de éste
    /// </summary>
    /// <param name="checkRange"></param>
    /// <returns></returns>
    public bool StartInside(DateRange checkRange)
    {
        return Inside(checkRange.Start);
    }

    /// <summary>
    /// El rango del parámetro termina dentro de éste
    /// </summary>
    /// <param name="checkRange"></param>
    /// <returns></returns>
    public bool EndsInside(DateRange checkRange)
    {
        return Inside(checkRange.End);
    }

    /// <summary>
    /// El rango del parámetro empieza dentro de éste y termina fuera
    /// </summary>
    /// <param name="checkRange"></param>
    /// <returns></returns>
    public bool StartInsideEndsOutside(DateRange checkRange)
    {
        if (!InclusiveStart)
        {
            var useCheckDate = UseHours ? checkRange.Start : checkRange.Start.Date;
            if (useCheckDate == UseStart) return true;
        }

        return Inside(checkRange.Start) && !Inside(checkRange.End);
    }

    /// <summary>
    /// El rango del parámetro empieza fuera y termina dentro de éste
    /// </summary>
    /// <param name="checkRange"></param>
    /// <returns></returns>
    public bool StartOutsideEndsInside(DateRange checkRange)
    {
        if (!InclusiveEnd)
        {
            var useCheckDate = UseHours ? checkRange.End : checkRange.End.Date;
            if (useCheckDate == UseEnd) return true;
        }

        return !Inside(checkRange.Start) && Inside(checkRange.End);
    }

    /// <summary>
    /// El rango del parámetro cubre por completo a éste
    /// </summary>
    /// <param name="checkRange"></param>
    /// <returns></returns>
    public bool FullOverlap(DateRange checkRange)
    {
        // Si cualquiera de las fechas a comprobar está dentro de rango, ya no lo está solapando de forma total
        if (Inside(checkRange.Start) || Inside(checkRange.End)) return false;

        var useCheckDateStart = UseHours ? checkRange.Start : checkRange.Start.Date;
        var useCheckDateEnd = UseHours ? checkRange.End : checkRange.End.Date;

        var condStart = InclusiveStart ? useCheckDateStart <= UseStart : useCheckDateStart < UseStart;
        var condEnd = InclusiveEnd ? useCheckDateEnd >= UseEnd : useCheckDateEnd > UseEnd;

        if (!InclusiveStart && !InclusiveEnd && Equals(checkRange)) return true;

        return condStart && condEnd;
    }

    public override string ToString()
    {
        return $"{Start:yyyy-MM-dd HH:mm:ss} - {End:yyyy-MM-dd HH:mm:ss} ({base.ToString()})";
    }

    public override bool Equals(object obj)
    {
        if (obj is not DateRange checkRange) return false;

        var useCheckDateStart = UseHours ? checkRange.Start : checkRange.Start.Date;
        var useCheckDateEnd = UseHours ? checkRange.End : checkRange.End.Date;

        return useCheckDateStart == UseStart && useCheckDateEnd == UseEnd;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
