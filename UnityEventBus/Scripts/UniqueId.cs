using System;

public record UniqueId
{
    public string Id => _id ??= Guid.NewGuid().ToString();
    private string _id;

    public static implicit operator string(UniqueId uniqueId) => uniqueId.Id;
}