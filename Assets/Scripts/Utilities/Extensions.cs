using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utilities
{
    public static class Extensions
    {
        public static void DoForAll<T>(this IList<T> list, System.Action<T> action)
        {
            int count = list.Count;

            for (int n = 0; n < count; n++)
            {
                T item = list[n];
                action(item);
            }
        }

        public static void PutFront<T>(this Queue<T> q, IEnumerable<T> items)
        {
            List<T> temp = new List<T>(items);
            temp.AddRange(q);
            q.Clear();
            q.AddRange(temp);
        }
        
        public static void PutFront<T>(this Queue<T> q, T item)
        {
            List<T> temp = new List<T>(){item};
            temp.AddRange(q);
            q.Clear();
            q.AddRange(temp);
        }
        
        public static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount)
        {
            return list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
        }

        public static List<int> ConvertListValuesToInt<T>(this IList<T> list) where T: IConvertible
        {
            List<int> newList = new List<int>(list.Count);

            for (var i = 0; i < list.Count; i++)
            {
                newList.Add(Convert.ToInt32(list[i]));
            }

            return newList;
        }
        
        public static List<T> ConvertIntegerListValuesToEnum<T>(this IList<int> list) where T: Enum
        {
            List<T> newList = new List<T>(list.Count);

            for (var i = 0; i < list.Count; i++)
            {
                newList.Add((T)Enum.ToObject(typeof(T),list[i]));
            }

            return newList;
        }
        
        public static List<int> ParseStringListValuesToInts(this IList<string> list)
        {
            List<int> newList = new List<int>(list.Count);

            string str = "";

            for (var i = 0; i < list.Count; i++)
            {
                str = list[i];
                if(string.IsNullOrEmpty(str)) continue;
                newList.Add(int.Parse(str));
            }

            return newList;
        }
        
        public static List<float> ParseStringListValuesToFloats(this IList<string> list)
        {
            List<float> newList = new List<float>(list.Count);

            for (var i = 0; i < list.Count; i++)
            {
                newList.Add(float.Parse(list[i]));
            }

            return newList;
        }
        
        public static int CountOf(this string str, char target)
        {
            int count = 0;
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i].Equals(target)) count++;
            }
            
            return count;
        }

        public static string Reverse(this string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            for (var i = 0; i < str.Length; i++)
            {
                sb.Append(str[i]);
            }

            return sb.ToString();
        }
        
        public static T PickRandom<T>(this IList<T> itemList, bool shouldItemBeRemoved = false, System.Func<T, float> probabilityGetterFunction = null)
        {
            if (itemList.Count == 0)
            {
                Debug.LogError("PickRandom() --> There is no item in the list!");
                return default;
            }

            int randomIndex = 0;
            T item = default;

            randomIndex = probabilityGetterFunction == null
                ? Random.Range(0, itemList.Count) : PickRandomIndexWithProbability(itemList, probabilityGetterFunction);

            item = itemList[randomIndex];

            if (shouldItemBeRemoved)
                itemList.RemoveAt(randomIndex);

            return item;
        }

        public static int CheckCountOfItem<T>(this IList<T> itemList, T item) where T : class
        {
            int count = 0;
            for (int i = 0; i < itemList.Count; i++)
            {
                if (CompareTwo(itemList[i], item))
                    count++;
            }

            return count;
        }
        
        public static bool CompareTwo<T>(T x, T y) where T : class
        {
            return x == y;
        }

        public static T PickRandomNoRepeat<T>(this IList<T> originalList, List<T> tempList, params T[] excludedItemsForRefill)
        {
            originalList.RefillNoneRepeatingListIfItIsEmpty(tempList, excludedItemsForRefill);

            return PickRandom(tempList, shouldItemBeRemoved: true);
        }

        private static void RefillNoneRepeatingListIfItIsEmpty<T>(this IList<T> originalList, List<T> tempList, params T[] excludedItemsForRefill)
        {
            if (tempList.Count == 0)
            {
                tempList.AddRange(originalList);

                for (int n = 0; n < excludedItemsForRefill.Length; n++)
                {
                    if (excludedItemsForRefill != null)
                        tempList.Remove(excludedItemsForRefill[n]);
                }
            }
        }
    
        private static int PickRandomIndexWithProbability<T>(IList<T> itemList, System.Func<T, float> probabilityGetterFunction)

        {
            double probabilitySum = itemList.Sum(probabilityGetterFunction);

            if (probabilitySum == 0)
                return Random.Range(0, itemList.Count);

            double randomNumber = Random.value * probabilitySum;
            double totalProbabilityFactor = 0;

            int n;

            for (n = 0; n < itemList.Count; n++)
            {
                var item = itemList[n];

                totalProbabilityFactor += probabilityGetterFunction(item);

                if (randomNumber <= totalProbabilityFactor)
                    break;
            }

            return n;
        }

        public static T Min<T>(this IList<T> itemList, System.Func<T, float> filterFunction)
        {
            if (itemList.Count == 0)
            {
                Debug.LogError("Min --> There is no item in the list!");
                return default(T);
            }
        
            float minValue = float.MaxValue;
            T item = default;
        
            int count = itemList.Count;

            for (int n = 0; n < count; n++)
            {
                T currentItem = itemList[n];

                float value = filterFunction(currentItem);

                if (value < minValue)
                {
                    minValue = value;
                    item = currentItem;
                }
            }
            return item;
        }

        public static void ResetTransformExtension(this Transform transform)
        {
            transform.position = Vector3.zero;
        }

        public static string UpperCaseAlphabeticAddition(this string sourceString, int amount)
        {
            int index = sourceString.Length-1;
            int range = 'Z' - 'A' + 1;
            StringBuilder text = new StringBuilder(sourceString);

            while (amount > 0)
            {
                int newValue = text[index] + amount;

                if (newValue >= 'A' && newValue <= 'Z')
                {
                    text[index] = (char) newValue;
                    amount = 0;
                }
                else
                {
                    amount = (newValue-'A') / range;
                    newValue = ((newValue -'A') % range) + 'A';
                    text[index] = (char)newValue;
                
                    index--;
                    if (index < 0)
                    {
                        amount--;
                        text.Insert(0, 'A');
                        index = 0;
                    }
                }
            }

            return text.ToString();
        }

        public static void AddRange<T>(this Queue<T> queue, IEnumerable<T> enu)
        {
            foreach (T obj in enu)
                queue.Enqueue(obj);
        }

        public static List<string> SeparateCamelCaseWise(this string originalString)
        {
            Func<char, bool> currentCheckMethod;
            List<string> parts = new List<string>();

            if (string.IsNullOrEmpty(originalString))
                return parts;
            
            void ReAssignMethod(char c)
            {
                if (Char.IsUpper(c))
                    currentCheckMethod = Char.IsLower;
                else
                    currentCheckMethod = Char.IsUpper;
            }

            int index = 0;

            ReAssignMethod(originalString[index]);

            string part = "";
            
            for (; index < originalString.Length; index++)
            {
                char c = originalString[index];
                
                if (currentCheckMethod(c))
                {
                    parts.Add(part);
                    part = c.ToString();
                    ReAssignMethod(c);
                    break;
                }
                else
                {
                    part += c;
                }
            }

            parts.Add(part);

            return parts;
        }

        public static Vector3 XY_TO_XZ(this Vector2 xyVector)
        {
            return new Vector3(xyVector.x,0,xyVector.y);
        }
    }
}
