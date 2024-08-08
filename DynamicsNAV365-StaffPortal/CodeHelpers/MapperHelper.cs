using System;
using System.Reflection;

namespace DynamicsNAV365_StaffPortal.CodeHelpers
{
    public static class MapperHelper
    {
        public static TData Map<TData>(object oldObject) where TData : new()
        {
            // Create a new object of type TDATA
            TData newObject = new TData();
            try
            {
                // If the old object is null, just return the new object
                if (oldObject == null) return newObject;
                // Get the type of the new object and the type of the old object passed in
                Type newObjType = typeof(TData);
                Type oldObjType = oldObject.GetType();
                // Get a list of all the properties in the new object
                var propertyList = newObjType.GetProperties();
                // If the new object has properties
                if (propertyList.Length > 0)
                {
                    // Loop through each property in the new object
                    foreach (var newObjProp in propertyList)
                    {
                        // Get the corresponding property in the old object
                        var oldProp = oldObjType.GetProperty(newObjProp.Name,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                            BindingFlags.IgnoreCase | BindingFlags.ExactBinding);
                        // If there is a corresponding property in the old object and it can be read and the new object's property can be written to
                        if (oldProp != null && oldProp.CanRead && newObjProp.CanWrite)
                        {
                            // assign property type of both object to new variables
                            var oldPropertyType = oldProp.PropertyType;
                            var newPropertyType = newObjProp.PropertyType;
                            //check if property is nullable or not. if property is nullable then get it's original data type from generic argument
                            if (oldPropertyType.IsGenericType &&
                                oldPropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                oldPropertyType = oldPropertyType.GetGenericArguments()[0];
                            if (newPropertyType.IsGenericType &&
                                newPropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                newPropertyType = newPropertyType.GetGenericArguments()[0];
                            //check type of both property if match then set value
                            if (newPropertyType == oldPropertyType)
                            {
                                // Get the value of the property in the old object
                                var value = oldProp.GetValue(oldObject);
                                // Set the value of the property in the new object
                                newObjProp.SetValue(newObject, value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // If there is an exception, log it
            }

            // Return the new object
            return newObject;
        }
    }
}