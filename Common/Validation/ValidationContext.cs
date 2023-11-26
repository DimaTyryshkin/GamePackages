using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Object = UnityEngine.Object;

namespace GamePackages.Core.Validation
{
	public class ValidationContext
	{
		public Object currentRoot;
		public FieldInfo currentFieldInfo; 
		public List<ValidationProblem> validationProblems = new List<ValidationProblem>(0);

		public void AddProblem(string header, ValidationProblem.Type type = ValidationProblem.Type.Error, string msg = null, Object overrideRoot = null)
		{
			ValidationProblem problem = new ValidationProblem();
			problem.type = type;
			problem.root = currentRoot;
			problem.fieldInfo = currentFieldInfo; 

			problem.header = header;
			problem.msg = msg;

			if (overrideRoot)
				problem.root = overrideRoot;

			problem.CacheToString();
			validationProblems.Add(problem);
		}

		public void NoDuplicate<T>(string problemHeader, ICollection<T> collection, ValidationProblem.Type problemType = ValidationProblem.Type.Error)
		{
			NoDuplicate(problemHeader, collection, x => x, problemType);
		}

		public void NoDuplicate<T, T2>(string problemHeader, ICollection<T> collection, Func<T, T2> func, ValidationProblem.Type problemType = ValidationProblem.Type.Error)
		{
			if (collection == null)
				return;
			HashSet<T2> set = new HashSet<T2>(32);

			foreach (var item in collection)
			{
				if (!set.Add(func(item)))
					AddProblem(problemHeader, problemType, "Duplicates found", item as Object ?? null);
			}
		}

		public void AllEquals<T>(string problemHeader, ICollection<T> collection, ValidationProblem.Type problemType = ValidationProblem.Type.Error)
		{
			AllEquals(problemHeader, collection, x => x, problemType);
		}

		public void AllEquals<T, T2>(string problemHeader, ICollection<T> collection, Func<T, T2> func, ValidationProblem.Type problemType = ValidationProblem.Type.Error)
		{
			if (collection == null)
				return;


			if (!collection.Any())
				return;

			HashSet<T2> set = new HashSet<T2>(32);

			set.Add(func(collection.First()));

			foreach (var item in collection)
			{
				if (set.Add(func(item)))
					AddProblem(problemHeader, problemType, "Elements in collection must be equals", item as Object ?? null);
			}
		}
	}
}