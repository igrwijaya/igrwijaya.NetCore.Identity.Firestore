using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;

namespace igrwijaya.NetCore.Identity.Firestore;

public partial class FirestoreUserStore<TUser, TRole> :
    IUserStore<TUser>,
    IUserEmailStore<TUser>,
    IUserPasswordStore<TUser>,
    IUserRoleStore<TUser> 
    where TUser : FirestoreIdentityUser
    where TRole : FirestoreIdentityRole
{
    #region Fields

    private readonly CollectionReference _userDataRef;
    private readonly CollectionReference _roleDataRef;

    #endregion
    
    public IdentityErrorDescriber ErrorDescriber { get; }

    public FirestoreUserStore(IdentityErrorDescriber errorDescriber = null)
    {
        ErrorDescriber = errorDescriber;
        var firestore = FirestoreDb.Create(Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID"));
        _userDataRef = firestore.Collection("users");
        _roleDataRef = firestore.Collection("roles");
    }

    #region IDisposable

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    private bool _disposed;

    public void Dispose()
    {
        _disposed = true;
    }

    #endregion

    #region User Store

    public async Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return user.Id;
    }

    public async Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return user.UserName;
    }

    public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (string.IsNullOrEmpty(userName))
        {
            throw new ArgumentNullException(nameof(userName));
        }
        
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        user.UserName = userName;
        
        return Task.CompletedTask;
    }

    public async Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return user.NormalizedUserName;
    }

    public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (string.IsNullOrEmpty(normalizedName))
        {
            throw new ArgumentNullException(nameof(normalizedName));
        }
        
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        user.NormalizedUserName = normalizedName;
        
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        cancellationToken.ThrowIfCancellationRequested();

        user.Id = Guid.NewGuid().ToString("N");
        
        await _userDataRef
            .Document(user.Id)
            .SetAsync(user, SetOptions.MergeAll, cancellationToken);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        await _userDataRef
            .Document(user.Id)
            .SetAsync(user, SetOptions.MergeAll, cancellationToken);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        await _userDataRef
            .Document(user.Id)
            .DeleteAsync(Precondition.MustExist, cancellationToken);

        return IdentityResult.Success;
    }

    public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        cancellationToken.ThrowIfCancellationRequested();
        var user = await ReadUserAsync(userId, cancellationToken);
        
        return user;
    }

    public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        var query = await _userDataRef
            .WhereEqualTo(nameof(FirestoreIdentityUser.NormalizedUserName), normalizedUserName)
            .Limit(1)
            .GetSnapshotAsync(cancellationToken);

        if (query.Count <= 0)
        {
            return null;
        }
        
        var userDoc = query.Documents.FirstOrDefault();

        if (userDoc == null)
        {
            return null;
        }

        return userDoc.ConvertTo<TUser>();
    }
    
    private async Task<TUser> ReadUserAsync(string userId, CancellationToken cancellationToken)
    {
        var docRef = await _userDataRef
            .Document(userId)
            .GetSnapshotAsync(cancellationToken);
        
        if (docRef == null)
        {
            return null;
        }
        
        var user = docRef.ConvertTo<TUser>();

        if (user == null)
        {
            return null;
        }
                
        user.Id = docRef.Id;

        return user;
    }

    #endregion

    #region User Password

    public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (string.IsNullOrEmpty(passwordHash) || user == null)
        {
            throw new ArgumentNullException(nameof(passwordHash));
        }

        user.PasswordHash = passwordHash;

        return Task.FromResult(0);
    }

    public async Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return user.PasswordHash;
    }

    public async Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return string.IsNullOrEmpty(user.PasswordHash);
    }

    #endregion

    #region User Role

    public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new ArgumentNullException(nameof(roleName));
        }
        
        var query = await _roleDataRef
            .WhereEqualTo(nameof(FirestoreIdentityRole.Name), roleName)
            .Limit(1)
            .GetSnapshotAsync(cancellationToken);

        if (query.Count <= 0)
        {
            throw new ArgumentNullException(nameof(query));
        }
        
        var roleDoc = query.Documents.FirstOrDefault();

        if (roleDoc == null)
        {
            throw new ArgumentNullException(nameof(roleDoc));
        }

        await _userDataRef
            .Document(user.Id)
            .Collection("user-roles")
            .AddAsync(new FirestoreUserRole
            {
                UserId = user.Id,
                RoleId = roleDoc.Id
            }, cancellationToken);

        await _roleDataRef
            .Document(roleDoc.Id)
            .Collection("users")
            .AddAsync(new FirestoreUserRole
            {
                UserId = user.Id,
                RoleId = roleDoc.Id
            }, cancellationToken);
    }

    public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new ArgumentNullException(nameof(roleName));
        }
        
        var query = await _roleDataRef
            .WhereEqualTo(nameof(FirestoreIdentityRole.Name), roleName)
            .Limit(1)
            .GetSnapshotAsync(cancellationToken);

        if (query.Count <= 0)
        {
            throw new ArgumentNullException(nameof(query));
        }
        
        var roleDoc = query.Documents.FirstOrDefault();

        if (roleDoc == null)
        {
            throw new ArgumentNullException(nameof(roleDoc));
        }

        var userRoleSnapshot = await _userDataRef
            .Document(user.Id)
            .Collection("user-roles")
            .WhereEqualTo(nameof(FirestoreUserRole.RoleId), roleDoc.Id)
            .GetSnapshotAsync(cancellationToken);

        foreach (var userRole in userRoleSnapshot)
        {
            await _userDataRef
                .Document(user.Id)
                .Collection("user-roles")
                .Document(userRole.Id)
                .DeleteAsync(Precondition.MustExist, cancellationToken);
        }
        
        
        var roleSnapshot = await _roleDataRef
            .Document(roleDoc.Id)
            .Collection("users")
            .WhereEqualTo(nameof(user.Id), user.Id)
            .GetSnapshotAsync(cancellationToken);

        foreach (var role in roleSnapshot)
        {
            await _roleDataRef
                .Document(roleDoc.Id)
                .Collection("users")
                .Document(role.Id)
                .DeleteAsync(Precondition.MustExist, cancellationToken);
        }
    }

    public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var userRoleSnapshot = await _userDataRef
            .Document(user.Id)
            .Collection("user-roles")
            .GetSnapshotAsync(cancellationToken);

        var roleIds = userRoleSnapshot
            .Documents
            .Select(item => item.ConvertTo<FirestoreUserRole>().RoleId)
            .ToList();

        if (!roleIds.Any())
        {
            return new List<string>();
        }

        var roleSnapshot = await _roleDataRef
            .WhereIn(nameof(FirestoreIdentityRole.Id), roleIds)
            .GetSnapshotAsync(cancellationToken);

        return roleSnapshot
            .Documents
            .Select(item => item.ConvertTo<TRole>().Name)
            .ToList();
    }

    public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new ArgumentNullException(nameof(roleName));
        }
        
        var query = await _roleDataRef
            .WhereEqualTo(nameof(FirestoreIdentityRole.Name), roleName)
            .Limit(1)
            .GetSnapshotAsync(cancellationToken);

        if (query.Count <= 0)
        {
            throw new ArgumentNullException(nameof(query));
        }
        
        var roleDoc = query.Documents.FirstOrDefault();

        if (roleDoc == null)
        {
            throw new ArgumentNullException(nameof(roleDoc));
        }
        
        var userRoleSnapshot = await _userDataRef
            .Document(user.Id)
            .Collection("user-roles")
            .WhereEqualTo(nameof(FirestoreUserRole.RoleId), roleDoc.Id)
            .GetSnapshotAsync(cancellationToken);

        return userRoleSnapshot.Count > 0;
    }

    public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new ArgumentNullException(nameof(roleName));
        }
        
        var query = await _roleDataRef
            .WhereEqualTo(nameof(FirestoreIdentityRole.Name), roleName)
            .Limit(1)
            .GetSnapshotAsync(cancellationToken);

        if (query.Count <= 0)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var roleDoc = query.Documents.FirstOrDefault();

        if (roleDoc == null)
        {
            throw new ArgumentNullException(nameof(roleDoc));
        }

        var userInRoleSnapshot = await _roleDataRef
            .Document(roleDoc.Id)
            .Collection("users")
            .GetSnapshotAsync(cancellationToken);

        var userIds = userInRoleSnapshot
            .Documents
            .Select(item => item.ConvertTo<FirestoreUserRole>().UserId)
            .ToList();

        if (!userIds.Any())
        {
            return new List<TUser>();
        }
        
        var userSnapshot = await _userDataRef
            .WhereIn(nameof(FirestoreIdentityUser.Id), userIds)
            .GetSnapshotAsync(cancellationToken);

        return userSnapshot
            .Documents
            .Select(item => item.ConvertTo<TUser>())
            .ToList();
    }

    #endregion

    #region User Email

    public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (string.IsNullOrEmpty(email) || user == null)
        {
            throw new ArgumentNullException(nameof(email));
        }

        user.PasswordHash = email;

        return Task.FromResult(0);
    }

    public async Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return user.Email;
    }

    public async Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return user.EmailConfirmed;
    }

    public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        user.EmailConfirmed = confirmed;

        return Task.FromResult(0);
    }

    public async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        var query = await _userDataRef
            .WhereEqualTo(nameof(FirestoreIdentityUser.NormalizeEmail), normalizedEmail)
            .Limit(1)
            .GetSnapshotAsync(cancellationToken);

        if (query.Count <= 0)
        {
            return null;
        }
        
        var userDoc = query.Documents.FirstOrDefault();

        if (userDoc == null)
        {
            return null;
        }

        return userDoc.ConvertTo<TUser>();
    }

    public async Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return user.NormalizeEmail;
    }

    public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        user.NormalizeEmail = normalizedEmail;

        return Task.FromResult(0);
    }

    #endregion
}