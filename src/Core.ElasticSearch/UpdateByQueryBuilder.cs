using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Core.ElasticSearch.Domain;
using Nest;
using SharpFuncExt;

namespace Core.ElasticSearch
{
    public class UpdateByQueryBuilder<T> where T : IEntity, IUpdateProjection
	{
		private StringBuilder _script = new StringBuilder();
		private readonly IDictionary<string, object> _paramsDictionary = new Dictionary<string, object>();

		public FluentDictionary<string, object> GetParams(FluentDictionary<string, object> fluentDictionary)
			=> fluentDictionary.AddEach(_paramsDictionary);

		public override string ToString() => _script.ToString();

		public static implicit operator string(UpdateByQueryBuilder<T> update) => update._script.ToString();

		private string Path<TValue>(Expression<Func<T, TValue>> member) =>
			(member.Body is UnaryExpression
				? ((UnaryExpression) member.Body).Operand
				: member.Body).ToString()
			.Convert(x => x.Substring(x.IndexOf(".") + 1).ToLower());

		private string PathNested<TNested, TValue>(Expression<Func<TNested, TValue>> member) =>
			(member.Body is UnaryExpression
				? ((UnaryExpression)member.Body).Operand
				: member.Body).ToString()
			.Convert(x => x.Substring(x.IndexOf(".") + 1).ToLower());

		public UpdateByQueryBuilder<T> Inc<TValue>(Expression<Func<T, TValue>> member, TValue value)
		{
			var p = Path(member);
			_script.Append($"if (ctx._source.containsKey('{p}')) ctx._source.{p}+=params.param_{_paramsDictionary.Count}; else ctx._source.{p}=params.param_{_paramsDictionary.Count};");
			_paramsDictionary.Add("param_" + _paramsDictionary.Count, value);
			return this;
		}

		public UpdateByQueryBuilder<T> IncNested<TNested, TValue>(Expression<Func<T, TNested[]>> nested, Expression<Func<TNested, TValue>> member, Guid nestedId, TValue value)
		{
			var p_nested = Path(nested);
			var p = PathNested(member);
			_script.Append($"for (int i = 0; i < ctx._source.{p_nested}.size(); i++){{if(ctx._source.{p_nested}[i].id == params.param_{_paramsDictionary.Count}){{ctx._source.{p_nested}[i].{p}+=params.param_{_paramsDictionary.Count+1};}}}}");
			//_script.Append($"if (ctx._source.containsKey('{p}')) ctx._source.{p}+=params.param_{_paramsDictionary.Count}; else ctx._source.{p}=params.param_{_paramsDictionary.Count};");
			_paramsDictionary.Add("param_" + _paramsDictionary.Count, nestedId.ToString());
			_paramsDictionary.Add("param_" + _paramsDictionary.Count, value);
			return this;
		}

		public UpdateByQueryBuilder<T> Set<TValue>(Expression<Func<T, TValue>> member, TValue value)
		{
			if (value.IsNull())
				return Unset(member);
			_script.Append($"ctx._source.{Path(member)}=params.param_{_paramsDictionary.Count};");
			_paramsDictionary.Add("param_" + _paramsDictionary.Count, value is IEntity ? ((IEntity)value).Id : (object)value);
			return this;
		}

		public UpdateByQueryBuilder<T> Unset<TValue>(Expression<Func<T, TValue>> member)
		{
			_script.Append($"ctx._source.remove('{Path(member)}');");
			return this;
		}

		public UpdateByQueryBuilder<T> Add<TValue>(Expression<Func<T, IEnumerable<TValue>>> member, TValue value)
		{
			value.ThrowIfNull(() => new ArgumentNullException());
			var p = Path(member);
			_script.Append($"if (ctx._source.containsKey('{p}')){{ if(!ctx._source.{p}.contains(params.param_{_paramsDictionary.Count})) ctx._source.{p}.add(params.param_{_paramsDictionary.Count}); }} else ctx._source.{p}=[params.param_{_paramsDictionary.Count}];");
			_paramsDictionary.Add("param_" + _paramsDictionary.Count, value is IEntity ? ((IEntity)value).Id : (object)value);
			return this;
		}

		public UpdateByQueryBuilder<T> Remove<TValue>(Expression<Func<T, IEnumerable<TValue>>> member, TValue value)
		{
			value.ThrowIfNull(() => new ArgumentNullException());
			var p = Path(member);
			//_script.Append($"if (ctx._source.containsKey('{p}')){{ if (ctx._source.{p}.size() > 1) Arrays.stream(array).filter(e-> !e.equals(foo)).toArray(String[]::new); else ctx._source.remove('{p}');}}");
			_script.Append($"if (ctx._source.containsKey('{p}')){{ if (ctx._source.{p}.size() > 1) {{ArrayList t = new ArrayList();t.add(params.param_{_paramsDictionary.Count}); ctx._source.{p}.removeAll(t);}} else ctx._source.remove('{p}');}}");
			_paramsDictionary.Add("param_" + _paramsDictionary.Count, value is IEntity ? ((IEntity)value).Id : (object)value);
			return this;
		}
	}
}
