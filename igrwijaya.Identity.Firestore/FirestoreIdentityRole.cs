using Google.Cloud.Firestore;

namespace igrwijaya.Identity.Firestore;

[FirestoreData]
public class FirestoreIdentityRole
{
    public FirestoreIdentityRole()
    {
        
    }

    public FirestoreIdentityRole(string name)
    {
        Name = name;
    }
    
    [FirestoreProperty]
    public string Id { get; internal set; }
    
    [FirestoreProperty]
    public string Name { get; internal set; }
    
    [FirestoreProperty]
    public string NormalizedName { get; internal set; }
}