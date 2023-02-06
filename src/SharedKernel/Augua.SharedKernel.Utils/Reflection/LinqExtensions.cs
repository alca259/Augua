using System.Linq.Expressions;

namespace System.Linq;

/// <summary>
/// Extensiones de LINQ
/// </summary>
public static class LinqExtensions
{
	/// <summary>
	/// Ejecuta el where si se cumple la condicion
	/// </summary>
	/// <typeparam name="TSource"></typeparam>
	/// <param name="source"></param>
	/// <param name="condition"></param>
	/// <param name="predicate"></param>
	/// <returns></returns>
	public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
	{
		return condition ? source.Where(predicate) : source;
	}

	/// <summary>
	/// Ejecuta el where si se cumple la condicion
	/// </summary>
	/// <typeparam name="TSource"></typeparam>
	/// <param name="source"></param>
	/// <param name="condition"></param>
	/// <param name="predicate"></param>
	/// <returns></returns>
	public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
	{
		return condition ? source.Where(predicate) : source;
	}

	/// <summary>
	/// Ordenación custom por reflexión
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <param name="source"></param>
	/// <param name="orderByProperty"></param>
	/// <param name="desc"></param>
	/// <returns></returns>
	public static IOrderedQueryable<TEntity> OrderByCustom<TEntity>(this IQueryable<TEntity> source, string orderByProperty, bool desc = false)
	{
		string command = desc ? "OrderByDescending" : "OrderBy";
		var type = typeof(TEntity);
		var property = type.GetProperty(orderByProperty);
		var parameter = Expression.Parameter(type, "p");
		var propertyAccess = Expression.MakeMemberAccess(parameter, property);
		var orderByExpression = Expression.Lambda(propertyAccess, parameter);
		var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
			source.Expression, Expression.Quote(orderByExpression));
		return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(resultExpression);
	}

	#region Func Extensions
	/// <summary>
	/// Concat two filters with AND condition
	/// </summary>
	/// <typeparam name="TIn"></typeparam>
	/// <param name="filter"></param>
	/// <param name="andFilter"></param>
	/// <returns></returns>
	public static Func<TIn, bool> And<TIn>(this Func<TIn, bool> filter, Func<TIn, bool> andFilter)
		where TIn : class
	{
		return newFilter => filter(newFilter) && andFilter(newFilter);
	}

	/// <summary>
	/// Concat two filters with OR condition
	/// </summary>
	/// <typeparam name="TIn"></typeparam>
	/// <param name="filter"></param>
	/// <param name="andFilter"></param>
	/// <returns></returns>
	public static Func<TIn, bool> Or<TIn>(this Func<TIn, bool> filter, Func<TIn, bool> andFilter)
		where TIn : class
	{
		return newFilter => filter(newFilter) || andFilter(newFilter);
	}
	#endregion
}
