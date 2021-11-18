using System;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

namespace Needle.Timeline.ExpressionTrees
{
	// https://tyrrrz.me/blog/expression-trees
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public static class MyFirstTree
	{
		static MyFirstTree()
		{
			// ConstructHelloExpression();
			// LogMessagesBlockExpression();
			// AssignValue();
		}

		private static void ConstructHelloExpression()
		{
			// build a parameter named "name"
			var name = Expression.Parameter(typeof(string), "name");

			// get null or whitespace method
			var isNullOrWhitespace = typeof(string).GetMethod(nameof(string.IsNullOrWhiteSpace));

			// get condition check method
			var condition = Expression.Not(Expression.Call(isNullOrWhitespace!, name));

			var concatMethod = typeof(string)
				.GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) });
			var trueClause = Expression.Call(concatMethod!, Expression.Constant("Hello "), name);

			var falseClause = Expression.Constant(null, typeof(string));

			var conditional = Expression.Condition(condition, trueClause, falseClause);

			var lambda = Expression.Lambda<Func<string, string>>(conditional, name);

			var func = lambda.Compile();

			var result = func("World");
			Debug.Log(result);
		}

		private static void LogMessagesBlockExpression()
		{
			var logMethod = typeof(Debug).GetMethod("Log", new[] { typeof(string) });
			if (logMethod == null) throw new Exception();
			var block = Expression.Block(
				Expression.Call(logMethod, Expression.Constant("Hello Line1")),
				Expression.Call(logMethod, Expression.Constant("Hello Line2"))
			);
			var action = Expression.Lambda<Action>(block).Compile();

			action();
		}


		private class MyType
		{
			public float Test;
		}

		private static void AssignValue()
		{
			var type = typeof(MyType);

			var field = type.GetField("Test");
			var target = Expression.Parameter(type, "instance");
			var member = Expression.Field(target, field);

			var value = Expression.Parameter(typeof(float), "value");

			var assign = Expression.Assign(member, value);

			var exp = Expression.Lambda(assign, target, value).Compile();
			// var setter = Expression.Lambda<Action<MyType, float>>(assign, target, value).Compile();

			var instance = new MyType();
			// setter(instance, 42);
			exp.DynamicInvoke(instance, 42);
			Debug.Log(instance.Test);
		}

		// private static void BuildGenericMethod(Type type)
		// {
		// 	var method = typeof(MyFirstTree).GetMethod(nameof(BuildAssignExpression), 
		// 		BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
		// 	method = method.MakeGenericMethod(type, typeof(float));
		// 	
		// 	method.Invoke(this, new Object[] { genericArgs, 
		// 				typeof(Program), "GetSomething" });
		// }

		private static Action<T, float> BuildAssignExpression<T>()
		{
			var type = typeof(T);

			var field = type.GetField("Test");
			var target = Expression.Parameter(type, "instance");
			var member = Expression.Field(target, field);

			var value = Expression.Parameter(typeof(float), "value");

			var assign = Expression.Assign(member, value);

			var setter = Expression.Lambda<Action<T, float>>(assign, target, value).Compile();
			return setter;
		}
	}
}