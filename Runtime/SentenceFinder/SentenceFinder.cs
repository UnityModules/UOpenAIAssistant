using System.Collections.Generic;

public static class SentenceFinder
{
        public static string[] CreateSentences(string[] data)
        {
            List<string> list = new();
            string str = "";

            for(int i=0;i<data.Length;i++)
            {
                str += data[i];
                if (CheckSentenceEnd(data[i]) && str.Length > 250)
                    {
                        list.Add(str);
                        str = "";
                    }
            }

            if(!string.IsNullOrEmpty(str))
                list.Add(str);

            return list.ToArray();
        }

        public static bool CheckSentenceEnd(string sentence)
        {
            string[] endConditions = {"."};
            foreach (string endCondition in endConditions)
                if(sentence.Contains(endCondition))
                    return true;

            return false;
        }
}
