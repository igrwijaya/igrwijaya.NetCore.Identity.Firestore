using Google.Cloud.Firestore;
using igrwijaya.Identity.Firestore;

namespace igrwijaya.Test.Api;

[FirestoreData]
public class ApplicationUser : FirestoreIdentityUser
{
    public ApplicationUser()
    {
        
    }
    
    public ApplicationUser(string userName) : base(userName)
    {
    }
}