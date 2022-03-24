using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;

namespace igrwijaya.Identity.Firestore;

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

        cancellationToken.ThrowIfCancellationRequested();
        
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
        
        var identityRole = await ReadRoleAsync(role.Id, cancellationToken);

        return identityRole.Id;
    }

    public async Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        var identityRole = await ReadRoleAsync(role.Id, cancellationToken);

        return identityRole.Name;
    }

    public async Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (string.IsNullOrEmpty(roleName))
        {
            throw new ArgumentNullException(nameof(roleName));
        }
        
        var identityRole = await ReadRoleAsync(role.Id, cancellationToken);

        identityRole.Name = roleName;

        await _roleRef
            .Document(role.Id)
            .SetAsync(identityRole, SetOptions.MergeAll, cancellationToken);
    }

    public async Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        var identityRole = await ReadRoleAsync(role.Id, cancellationToken);

        return identityRole.NormalizedName;
    }

    public async Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        
        if (string.IsNullOrEmpty(normalizedName))
        {
            throw new ArgumentNullException(nameof(normalizedName));
        }
        
        var identityRole = await ReadRoleAsync(role.Id, cancellationToken);

        identityRole.NormalizedName = normalizedName;

        await _roleRef
            .Document(role.Id)
            .SetAsync(identityRole, SetOptions.MergeAll, cancellationToken);
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
            .WhereEqualTo(nameof(FirestoreIdentityUser.NormalizedUserName), normalizedRoleName)
            .Limit(1)
            .GetSnapshotAsync(cancellationToken);

        if (query.Count <= 0)
        {
            throw new ArgumentNullException(nameof(query));
        }
        
        var userDoc = query.Documents.FirstOrDefault();

        if (userDoc == null)
        {
            throw new ArgumentNullException(nameof(userDoc));
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
            throw new ArgumentNullException(nameof(docRef));
        }
        
        var role = docRef.ConvertTo<TRole>();

        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }
                
        role.Id = docRef.Id;

        return role;
    }
}