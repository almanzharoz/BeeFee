using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.ElasticSearch.Serialization
{
	public struct ActivatorData<T>
	{
		public ObjectActivator<T> Creator { get; }
		public KeyValuePair<string, Type>[] Parameters { get; }

		public ActivatorData(ObjectActivator<T> func, KeyValuePair<string, Type>[] parameters)
		{
			Creator = func;
			Parameters = parameters;
		}
	}
	public delegate T ObjectActivator<T>(params object[] args);
	public static class ObjectActivator
	{
		public static ActivatorData<T> GetActivator<T>(ConstructorInfo ctor)
		{
			ParameterInfo[] paramsInfo = ctor.GetParameters();

			//create a single param of type object[]
			ParameterExpression param =
				Expression.Parameter(typeof(object[]), "args");

			Expression[] argsExp =
				new Expression[paramsInfo.Length];

			//pick each arg from the params array 
			//and create a typed expression of them
			for (int i = 0; i < paramsInfo.Length; i++)
			{
				Expression index = Expression.Constant(i);
				Type paramType = paramsInfo[i].ParameterType;

				Expression paramAccessorExp =
					Expression.ArrayIndex(param, index);

				Expression paramCastExp =
					Expression.Convert(paramAccessorExp, paramType);

				argsExp[i] = paramCastExp;
			}

			//make a NewExpression that calls the
			//ctor with the args we just created
			NewExpression newExp = Expression.New(ctor, argsExp);

			//create a lambda with the New
			//Expression as body and our param object[] as arg
			LambdaExpression lambda =
				Expression.Lambda<ObjectActivator<T>>(newExp, param);

			//compile it
			ObjectActivator<T> compiled = (ObjectActivator<T>)lambda.Compile();
			return new ActivatorData<T>(compiled, ctor.GetParameters().Select(x => new KeyValuePair<string,Type>(x.Name.ToLowerInvariant(), x.ParameterType)).ToArray());
		}

		public static Action<T, object> GetSetter<T>(PropertyInfo property)
		{
			ParameterExpression targetExp = Expression.Parameter(typeof(T), "obj");
			ParameterExpression param = Expression.Parameter(typeof(object), "param");

			var assign = Expression.Lambda<Action<T, object>>
			(
				Expression.Assign(Expression.Property(targetExp, property), Expression.Convert(param, property.PropertyType)),
				targetExp, param
			);
			return assign.Compile();
		}
	}
}