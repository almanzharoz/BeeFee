using System;
using Core.ElasticSearch.Domain;
using SharpFuncExt;

namespace BeeFee.Model.Helpers
{
	/// <summary>
	/// Расширения для проверки аргуметов, являющихся проекциями
	/// </summary>
	public static class Extensions
	{
		public static T HasNotNullEntity<T>(this T arg, string argName) where T : IEntity
			=> arg.ThrowIfNull(x => x.Id, () => new ArgumentNullException(argName));

		public static T HasNotNullEntityWithVersion<T>(this T arg, string argName) where T : IEntity, IWithVersion
			=> arg.ThrowIfNull(x => x.Id, x => x.Version, () => new ArgumentNullException(argName));

		public static T HasNotNullEntityWithParent<T, TParent>(this T arg, string argName)
			where T : IEntity, IWithParent<TParent>
			where TParent : IProjection
			=> arg.ThrowIfNull(x => x.Id, () => new ArgumentNullException(argName))
				.Fluent(x => x.Parent.HasNotNullEntity(argName));

		public static T HasNotNullEntityWithParentAndVersion<T, TParent>(this T arg, string argName)
			where T : IEntity, IWithParent<TParent>, IWithVersion
			where TParent : IProjection
			=> arg.ThrowIfNull(x => x.Id, x => x.Version, () => new ArgumentNullException(argName))
				.Fluent(x => x.Parent.HasNotNullEntity(argName));

		public static T HasNotNullEntity<T>(this T arg, ThrowCollection collection, string argName) where T : IEntity
			=> arg.ThrowIfNull(collection, x => x.Id, () => new ArgumentNullException(argName));

		public static T HasNotNullEntityWithVersion<T>(this T arg, ThrowCollection collection, string argName) where T : IEntity, IWithVersion
			=> arg.ThrowIfNull(collection, x => x.Id, x => x.Version, () => new ArgumentNullException(argName));

		public static T HasNotNullEntityWithParent<T, TParent>(this T arg, ThrowCollection collection, string argName)
			where T : IEntity, IWithParent<TParent>
			where TParent : IProjection
			=> arg.ThrowIfNull(collection, x => x.Id, () => new ArgumentNullException(argName))
				.Fluent(x => x.Parent.HasNotNullEntity(collection, argName));

		public static T HasNotNullEntityWithParentAndVersion<T, TParent>(this T arg, ThrowCollection collection, string argName)
			where T : IEntity, IWithParent<TParent>, IWithVersion
			where TParent : IProjection
			=> arg.ThrowIfNull(collection, x => x.Id, x => x.Version, () => new ArgumentNullException(argName))
				.Fluent(x => x.Parent.HasNotNullEntity(collection, argName));
	}
}