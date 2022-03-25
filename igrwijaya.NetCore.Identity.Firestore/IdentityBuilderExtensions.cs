using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace igrwijaya.NetCore.Identity.Firestore;

public static class IdentityBuilderExtensions
{
    public static IdentityBuilder UseFirestore(this IdentityBuilder builder)
        => builder
            .AddFirestoreUserStore()
            .AddFirestoreRoleStore();
        
    private static IdentityBuilder AddFirestoreUserStore(this IdentityBuilder builder)
    {
        var userStoreType = typeof(FirestoreUserStore<,>).MakeGenericType(builder.UserType, builder.RoleType);

        builder.Services.AddScoped(
            typeof(IUserStore<>).MakeGenericType(builder.UserType),
            userStoreType
        );

        return builder;
    }

    private static IdentityBuilder AddFirestoreRoleStore(
        this IdentityBuilder builder
    )
    {
        var roleStoreType = typeof(FirestoreRoleStore<>).MakeGenericType(builder.RoleType);

        builder.Services.AddScoped(
            typeof(IRoleStore<>).MakeGenericType(builder.RoleType),
            roleStoreType
        );

        return builder;
    }
}