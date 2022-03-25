using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;

namespace igrwijaya.NetCore.Identity.Firestore;

public partial class FirestoreRoleStore<TRole>: IRoleStore<TRole> where TRole : FirestoreIdentityRole
{
    #region Private Methods

    private readonly CollectionReference _roleRef;

    #endregion
    
    public IdentityErrorDescriber ErrorDescriber { get; }

    public FirestoreRoleStore(IdentityErrorDescriber errorDescriber = null)
    {
        ErrorDescriber = errorDescriber;
        var firestore = FirestoreDb.Create(Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID"));
        _roleRef = firestore.Collection("roles");
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

    public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        role.Id = Guid.NewGuid().ToString("N");
        
        await _roleRef.AddAsync(role, cancellationToken);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        await _roleRef
            .Document(role.Id)
            .SetAsync(role, SetOptions.MergeAll, cancellationToken);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        await _roleRef
            .Document(role.Id)
            .DeleteAsync(Precondition.MustExist, cancellationToken);

        return IdentityResult.Success;
    }

    public async Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        return role.Id;
    }

    public async Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        return role.Name;
    }

    public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (string.IsNullOrEmpty(roleName))
        {
            throw new ArgumentNullException(nameof(roleName));
        }
        
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        role.Name = roleName;
        
        return Task.CompletedTask;
    }

    public async Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        return role.NormalizedName;
    }

    public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (string.IsNullOrEmpty(normalizedName))
        {
            throw new ArgumentNullException(nameof(normalizedName));
        }
        
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        role.NormalizedName = normalizedName;
        
        return Task.CompletedTask;
    }

    public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        cancellationToken.ThrowIfCancellationRequested();
        var identityRole = await ReadRoleAsync(roleId, cancellationToken);
        
        return identityRole;
    }

    public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        var query = await _roleRef
            .WhereEqualTo(nameof(FirestoreIdentityRole.NormalizedName), normalizedRoleName)
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

        return userDoc.ConvertTo<TRole>();
    }
    
    private async Task<TRole> ReadRoleAsync(string roleId, CancellationToken cancellationToken)
    {
        var docRef = await _roleRef
            .Document(roleId)
            .GetSnapshotAsync(cancellationToken);
        
        if (docRef == null)
        {
            return null;
        }
        
        var role = docRef.ConvertTo<TRole>();

        if (role == null)
        {
            return null;
        }
                
        role.Id = docRef.Id;

        return role;
    }
}