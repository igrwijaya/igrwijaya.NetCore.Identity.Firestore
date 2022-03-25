using Google.Cloud.Firestore;

namespace igrwijaya.NetCore.Identity.Firestore
{
    [FirestoreData]
    public class FirestoreUserRole
    {
        [FirestoreProperty] public virtual string UserId { get; set; }

        [FirestoreProperty] public virtual string RoleId { get; set; }
    }
}
