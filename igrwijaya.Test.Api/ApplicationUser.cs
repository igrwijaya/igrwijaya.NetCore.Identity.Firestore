using Google.Cloud.Firestore;
using igrwijaya.NetCore.Identity.Firestore;

namespace igrwijaya.Test.Api;

[FirestoreData]
public class ApplicationUser : FirestoreIdentityUser
{
    public ApplicationUser()
    {
        
    }

    public ApplicationUser(string userName, string email) : base(userName, email)
    {
    }
}