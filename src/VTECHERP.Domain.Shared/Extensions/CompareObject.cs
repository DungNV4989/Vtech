using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace VTECHERP.Extensions
{
    public static class CompareObject
    { /// <summary>
      /// Compare same object
      /// </summary>
      /// <param name="obj"></param>
      /// <param name="another"></param>
      /// <returns></returns>
      /// <exception cref="Exception"></exception>
        public static (Dictionary<string, object>, Dictionary<string, object>) CompareObjectValues(this object obj, object another)
        {
             
            if ((obj == null) || (another == null)) throw new Exception("Object is not null");
            //Compare two object's class, return false if they are difference
            //if (obj.GetType() != another.GetType()) throw new Exception("Diffirent type");

            var oldField = new Dictionary<string, object>();
            var newField = new Dictionary<string, object>();

            //Get all properties of obj
            //And compare each other
            foreach (var property in obj.GetType().GetProperties())
            {
                if(property.Name == "Id" 
                    || property.Name == "ExtraProperties"
                    || property.Name == "IsDeleted"
                    || property.Name == "DeleterId"
                    || property.Name == "DeletionTime"
                    || property.Name == "LastModificationTime"
                    || property.Name == "LastModifierId"
                    || property.Name == "CreationTime"
                    || property.Name == "CreatorId"
                    || property.Name == "ConcurrencyStamp"
                    )
                {
                    continue;
                }

                var objValue = property.GetValue(obj);
               
                var anotherPropertys = another.GetType().GetProperties();
                var anotherProperty = anotherPropertys.FirstOrDefault(x =>x.Name.Equals(property.Name));
                if (anotherProperty == null) continue;
                var anotherValue = anotherProperty.GetValue(another);

                if (objValue == null && anotherValue == null) continue;
                if (objValue is string && anotherValue is string && string.IsNullOrEmpty(objValue.ToString()) && string.IsNullOrEmpty(anotherValue.ToString())) continue;

                if (objValue == null && anotherValue != null)
                {
                    oldField.Add(property.Name, objValue);
                    newField.Add(property.Name, anotherValue);

                }
                else if (objValue != null && anotherValue == null)
                {
                    oldField.Add(property.Name, objValue);
                    newField.Add(property.Name, anotherValue);
                }
                else
                if (!objValue.Equals(anotherValue))
                {
                    oldField.Add(property.Name, objValue);
                    newField.Add(property.Name, anotherValue);
                }
            }

            return (oldField, newField);
        }
        public static (Dictionary<string, object>, Dictionary<string, object>) CompareObjectValuesWithListProperties(this object obj, object another)
        {

            if ((obj == null) && (another == null)) throw new Exception("Object is not null");
            //Compare two object's class, return false if they are difference
            //if (obj.GetType() != another.GetType()) throw new Exception("Diffirent type");

            var listProperties = new List<PropertyInfo>();

            if (obj != null)
                listProperties = obj.GetType().GetProperties().ToList();
            else if (another != null)
                listProperties = another.GetType().GetProperties().ToList();

            var oldField = new Dictionary<string, object>();
            var newField = new Dictionary<string, object>();

            //Get all properties of obj
            //And compare each other
            foreach (var property in listProperties)
            {
                if (property.Name == "Id" || property.Name == "ExtraProperties")
                {
                    continue;
                }

                // Get value 
                var objValue = new object();
                if (obj != null)
                    objValue = property.GetValue(obj);
                else
                    objValue = null;

                var anotherPropertys = new List<PropertyInfo>();
                PropertyInfo anotherProperty = null;
                if (another != null)
                {
                    anotherPropertys = another.GetType().GetProperties().ToList();

                    if (anotherPropertys.Count > 0)
                        anotherProperty = anotherPropertys.FirstOrDefault(x => x.Name.Equals(property.Name));
                }


                var anotherValue = new object();
                if (anotherProperty != null)
                    anotherValue = anotherProperty.GetValue(another);
                else
                    anotherValue = null;

                if (objValue == null && anotherValue == null) continue;
                if (objValue is string && anotherValue is string && string.IsNullOrEmpty(objValue.ToString()) && string.IsNullOrEmpty(anotherValue.ToString())) continue;

                Type oType = null;
                if (objValue != null)
                    oType = objValue.GetType();
                else if (anotherValue != null)
                    oType = anotherValue.GetType();

                if (property.Name == "Products")
                {
                    var ab = objValue;
                    var c = anotherValue;
                }

                if (objValue == null && anotherValue != null)
                {
                    oldField.Add(property.Name, objValue);
                    newField.Add(property.Name, anotherValue);

                }
                else if (objValue != null && anotherValue == null)
                {
                    oldField.Add(property.Name, objValue);
                    newField.Add(property.Name, anotherValue);
                }
                else if (oType.IsGenericType && oType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    IList collections = new ArrayList();
                    IList anotherCollections = new ArrayList();

                    if (objValue != null)
                        collections = (IList)objValue;
                    if (anotherValue != null)
                        anotherCollections = (IList)anotherValue;

                    ArrayList oldFieldList = new ArrayList();
                    ArrayList newFieldList = new ArrayList();


                    int? count = Math.Max(collections.Count, anotherCollections.Count);
                    //var itemType = objValue.GetType().GetGenericArguments().Single();

                    //var parameters = new object[obj.GetType().GetProperties().Length];

                    for (int i = 0; i < count; i++)
                    {
                        var compare = CompareObjectValuesWithListProperties(collections.Count >= (i + 1) ? collections[i] : null, anotherCollections.Count >= (i + 1) ? anotherCollections[i] : null);

                        if (compare.Item1.Count > 0 || compare.Item2.Count > 0)
                        {
                            oldFieldList.Add(compare.Item1);
                            newFieldList.Add(compare.Item2);
                        }
                    }
                    oldField.Add(property.Name, oldFieldList);
                    newField.Add(property.Name, newFieldList);
                }
                else
                if (!objValue.Equals(anotherValue))
                {
                    oldField.Add(property.Name, objValue);
                    newField.Add(property.Name, anotherValue);
                }
            }

            return (oldField, newField);
        }
        public static T Clone<T>(this T source) where T : class
        {
            //if (!typeof(T).IsSerializable)
            //{
            //    throw new ArgumentException("The type must be serializable.", nameof(source));
            //}

            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null)) return default;
            var serialize = Newtonsoft.Json.JsonConvert.SerializeObject(source);
            var target = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(serialize);
            if(target == null)
            {
                return default;
            }
            return target;
        }

        private static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }
    }
}