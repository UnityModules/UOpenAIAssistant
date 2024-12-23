using System.Collections.Generic;

namespace OpenAI.InterfaceLayer
{
    public static class InterfaceLayerHandler
    {
        public static List<InterfaceId> Ids = new List<InterfaceId>();

        public static void SetLayer(InterfaceId id)
        {
            Ids.Clear();
            Ids.Add(id);
        }

        public static void AddLayer(InterfaceId id) =>
            Ids.Add(id);

        public static void RemoveLayer(InterfaceId id) =>
            Ids.Remove(id);

        public static string CurrentId
        {
            get
            {
                string str = "";

                for(int i=0;i<Ids.Count;i++)
                    str += Ids[i].Id;

                return str;
            }
        }

        public static string Message =>
            "I'm in " + CurrentId +" Layer \n";
    }
}
