using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;

namespace igrwijaya.Identity.Firestore;

public partial class FirestoreUserStore<TUser> :
    IUserStore<TUser>,
    IUserPasswordStore<TUser>,
    IUserRoleStore<TUser>
    where TUser : FirestoreIdentityUser
{
    #region Private Methods

    private readonly CollectionReference _userDataRef;

    #endregion
    
    public IdentityErrorDescriber ErrorDescriber { get; }

    public FirestoreUserStore(IdentityErrorDescriber errorDescriber = null)
    {
        ErrorDescriber = errorDescriber;
        var firestore = FirestoreDb.Create(Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID"));
        _userDataRef = firestore.Collection("users");
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
        
        var userResult = await ReadUserAsync(user.Id, cancellationToken);

        return userResult.NormalizedUserName;
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
        
        await _userDataRef.Document(user.Id).SetAsync(user, SetOptions.MergeAll, cancellationToken);

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
            throw new ArgumentNullException(nameof(docRef));
        }
        
        var user = docRef.ConvertTo<TUser>();

        if (user == null)
        {
            return null;
        }
                
        user.Id = docRef.Id;

        return user;
    }

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
        throw new NotImplementedException();
    }

    public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}