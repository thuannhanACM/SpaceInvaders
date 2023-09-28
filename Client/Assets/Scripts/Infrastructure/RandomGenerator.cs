
using System;
using System.Collections.Generic;

namespace Core.Infrastructure
{
    public class RandomGenerator
    {
        public static int Choose(float[] probs)
        {
            float total = 0;
            var rand = new Random();

            foreach (float elem in probs)
                total += elem;

            float randomPoint = (float)(Math.Round(rand.NextDouble(), 2) * total);

            for (int i = 0; i < probs.Length; i++)
            {
                if (randomPoint < probs[i])
                    return i;
                else
                    randomPoint -= probs[i];
            }
            return probs.Length - 1;
        }

        public static int[] ChooseSet(
            int numRequired,
            int[] pool)
        {
            var rand = new Random();
            int[] result = new int[numRequired];
            int numToChoose = numRequired;

            for (int numLeft = numRequired; numLeft > 0; numLeft--)
            {
                numToChoose--;
                int index = rand.Next(pool.Length);
                result[numToChoose] = pool[index];

                if (numToChoose == 0)
                {
                    break;
                }
            }
            return result;
        }

        public static int[] ChooseUniqueSetWithProbs(
            int numRequired,
            Dictionary<int, float> poolProbDic)
        {
            int[] result = new int[numRequired];
            int numToChoose = numRequired;
            List<int> poolTmp = new List<int>(poolProbDic.Keys);
            List<float> probs = new List<float>(poolProbDic.Values);

            for (int numLeft = numRequired; numLeft > 0; numLeft--)
            {
                numToChoose--;
                int index = Choose(probs.ToArray());
                result[numToChoose] = poolTmp[index];
                poolTmp.RemoveAt(index);
                probs.RemoveAt(index);

                if (numToChoose == 0)
                {
                    break;
                }

                if (poolTmp.Count == 0)
                {
                    poolTmp = new List<int>(poolProbDic.Keys);
                }
            }
            return result;
        }

        public static T[] ChooseSetWithProbs<T>(
            int numRequired,
            Dictionary<T, float> poolProbDic)
        {
            T[] result = new T[numRequired];
            int numToChoose = numRequired;
            List<T> poolTmp = new List<T>(poolProbDic.Keys);

            float[] probs = new float[poolProbDic.Values.Count];
            poolProbDic.Values.CopyTo(probs, 0);

            for (int numLeft = numRequired; numLeft > 0; numLeft--)
            {
                numToChoose--;
                int index = Choose(probs);
                result[numToChoose] = poolTmp[index];

                if (numToChoose == 0)
                {
                    break;
                }

                if (poolTmp.Count == 0)
                {
                    poolTmp = new List<T>(poolProbDic.Keys);
                }
            }
            return result;
        }

        public static int[] ChooseSetWithProbsByCDF(int numRequired, Dictionary<int, float> poolRatioDic)
        {
            int[] result = new int[numRequired];
            List<float> inputRatio = new List<float>(poolRatioDic.Values);
            List<int> poolId = new List<int>(poolRatioDic.Keys);

            float totalDistribution;
            float[] CDF = CalculateDistributionTable(inputRatio.ToArray(), out totalDistribution);

            for (int i = 0; i < numRequired; i++)
            {
                Random r = new Random();
                float ratio = (float)(Math.Round(r.NextDouble(), 2) * totalDistribution);

                for(int j = 0; j < CDF.Length; j++)
                {
                    if(ratio <= CDF[j])
                        result[i] = poolId[j];
                }
            }

            return result;
        }

        public static float[] CalculateDistributionTable(float[] inputRatio, out float totalDistribution)
        {
            float[] distributionTable = new float[inputRatio.Length];
            distributionTable[0] = inputRatio[0];
            totalDistribution = distributionTable[0];

            for (int i = 1; i < distributionTable.Length; i++)
            {
                distributionTable[i] = distributionTable[i - 1] + inputRatio[i];
                totalDistribution += distributionTable[i];
            }

            return distributionTable;
        }
    }
}
