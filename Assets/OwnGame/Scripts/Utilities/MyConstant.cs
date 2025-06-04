using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyConstant
{
	public static System.Random random = new System.Random();
    public static T[] ShallowCopy<T>(this T[] _originalMatrix)
	{
		T[] _dataClone = new T[_originalMatrix.Length];
		for (int i = 0; i < _originalMatrix.Length; i++)
		{
			_dataClone[i] = _originalMatrix[i];
		}
		return _dataClone;
	}
	public static List<T> ShallowCopy<T>(this List<T> _originalMatrix)
	{
		List<T> _dataClone = new List<T>();
		for (int i = 0; i < _originalMatrix.Count; i++)
		{
			_dataClone.Add(_originalMatrix[i]);
		}
		return _dataClone;
	}
	public static bool CheckIfItsTime(this DateTime _dateTime)
	{
		if (_dateTime == DateTime.MinValue || _dateTime == DateTime.MaxValue)
		{
			//_dateTime = DateTime.UtcNow;
			return true;
		}
		if (_dateTime <= DateTime.UtcNow)
		{
			return true;
		}
		return false;
	}
}
