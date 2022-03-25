using Google.Cloud.Firestore;

namespace igrwijaya.NetCore.Identity.Firestore;

[FirestoreData]
public class FirestoreIdentityUser
{
    public FirestoreIdentityUser()
    {
        
    }
    
    public FirestoreIdentityUser(string userName, string email)
    {
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }

    [FirestoreProperty]
    public string Id { get; internal set; }
    
    [FirestoreProperty]
    public string UserName { get; internal set; }
    
    [FirestoreProperty]
    public string NormalizedUserName { get; internal set; }
    
    [FirestoreProperty]
    public string PasswordHash { get; internal set; }
    
    [FirestoreProperty]
    public string Email { get; internal set; }
    
    [FirestoreProperty]
    public bool EmailConfirmed { get; internal set; }
    
    [FirestoreProperty]
    public string NormalizeEmail { get; internal set; }
}