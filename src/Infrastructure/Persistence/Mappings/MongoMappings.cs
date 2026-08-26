using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Infrastructure.Persistence.Mappings;

internal static class MongoMappings
{
    public static void Register()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Product)))
            return;

        BsonClassMap.RegisterClassMap<Product>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(p => p.Id)
              .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
        });
    }
}
