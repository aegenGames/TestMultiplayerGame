using UnityEngine;

public static class GeneralAlgoritms
{
	public static void ShuffleArray<T>(T[] list)
	{
		int countElements = list.Length;
		for (int i = 0; i < countElements; ++i)
		{
			int j = Random.Range(0, countElements - 1);
			var tmp = list[i];
			list[i] = list[j];
			list[j] = tmp;
		}
	}
}
