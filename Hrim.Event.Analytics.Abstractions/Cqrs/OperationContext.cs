using System.Security.Claims;
using Hrim.Event.Analytics.Abstractions.Enums;

namespace Hrim.Event.Analytics.Abstractions.Cqrs;

/// <summary></summary>
public record OperationContext
{
    private string? _externalId;

    private ExternalIdp? _idp;
    private string?      _operatorEmail;

    /// <summary> </summary>
    /// <param name="userClaims"></param>
    /// <param name="correlationId"></param>
    public OperationContext(IEnumerable<Claim> userClaims, Guid correlationId) {
        UserClaims    = userClaims;
        CorrelationId = correlationId;
    }

    /// <summary>
    ///     Claims from jwt api access token
    /// </summary>
    public IEnumerable<Claim> UserClaims { get; init; }

    /// <summary>
    ///     Id that will be passed through the whole sequence of commands, queries, jobs, etc
    /// </summary>
    public Guid CorrelationId { get; init; }

    /// <summary> Operator Email taken from jwt claims </summary>
    public string? Email => _operatorEmail ??= UserClaims.FirstOrDefault(x => x.Type.Contains(value: "email"))?.Value;

    /// <summary> External User Identifier </summary>
    public string ExternalId() {
        if (_externalId == null)
            ProcessSubjectClaim();
        return _externalId!;
    }

    /// <summary> External Identity Provider that authorised current operator </summary>
    public ExternalIdp Idp() {
        if (_idp == null)
            ProcessSubjectClaim();
        return _idp!.Value;
    }

    private void ProcessSubjectClaim() {
        var subjectClaim = UserClaims.FirstOrDefault(x => x.Type == "sub");
        if (subjectClaim == null)
            throw new ArgumentException("There is no subject claim", "SubjectClaim");
        var subjectParts = subjectClaim.Value.Split(separator: '|');
        _externalId ??= subjectParts[1];
        _idp = subjectParts[0].StartsWith(value: "google")
                   ? ExternalIdp.Google
                   : subjectParts[0].StartsWith(value: "facebook")
                       ? ExternalIdp.Facebook
                       : ExternalIdp.Auth0;
    }
}