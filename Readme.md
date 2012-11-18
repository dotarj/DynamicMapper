DynamicMapper
========================================

DynamicMapper allows you to map a subset of an object's properties to an ExpandoObject.

Example:

```csharp
var entity = CreateEntity();

var properties = new List<PropertyInfo>()
{
    typeof(Entity).GetProperty("Int1"),
    typeof(Entity).GetProperty("String2"),
    typeof(Entity).GetProperty("String4"),
    typeof(Entity).GetProperty("Double1"),
    typeof(Entity).GetProperty("Double2")
};

var result = Mapper<Entity>.Map(entity, properties);
```