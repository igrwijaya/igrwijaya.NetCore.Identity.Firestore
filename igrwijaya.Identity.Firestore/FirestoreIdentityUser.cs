using Google.Cloud.Firestore;

namespace igrwijaya.Identity.Firestore;

[FirestoreData]
public class FirestoreIdentityUser
{
    public FirestoreIdentityUser()
    {
        
    }
    
    public FirestoreIdentityUser(string userName)
    {
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
    }

    [FirestoreProperty]
    public string Id { get; internal set; }
    
    [FirestoreProperty]
    public string UserName { get; internal set; }
    
    [FirestoreProperty]
    public string NormalizedUserName { get; internal set; }
    
    [FirestoreProperty]
    public string PasswordHash { get; internal set; }
}