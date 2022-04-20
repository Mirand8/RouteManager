using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System;

namespace RouteManager.Models
{
    [CollectionName("Roles")]
    public class AppRole : MongoIdentityRole<Guid>
    {

    }
}
