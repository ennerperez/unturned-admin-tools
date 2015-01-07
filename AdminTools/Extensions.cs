using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unturned
{
    public static class Extensions
    {

        public static int ToInt(this Enum e)
        {
            object val = Convert.ChangeType(e, e.GetTypeCode());
            return (int)(val);
        }

        public static int[] Find(this Inventory i, int id)
        {
            List<int> itemPos = new List<int>();
            Point2 obj = i.search(id);
            foreach (System.Reflection.FieldInfo item in obj.GetType().GetFields())
            {
                if (item.FieldType.IsPrimitive)
                {
                    itemPos.Add((int)item.GetValue(obj));
                }
            }
            return itemPos.ToArray();
        }

        public static bool IsXYValid(this int[] v)
        {
            if (v.Length > 1)
            {
                return v[0] >= 0 && v[1] >= 0;
            }

            return false;
            
        }

    }
}
