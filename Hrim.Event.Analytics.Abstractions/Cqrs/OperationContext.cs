using System.Security.Claims;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrimsoft.StringCases;

namespace Hrim.Event.Analytics.Abstractions.Cqrs;

/// <summary></summary>
public record OperationContext
{
    public OperationContext(IEnumerable<Claim> userClaims, Guid correlationId) {
        UserClaims    = userClaims;
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Claims from jwt api access token
    /// </summary>
    public IEnumerable<Claim> UserClaims { get; init; }

    /// <summary>
    /// Id that will be passed through the whole sequence of commands, queries, jobs, etc
    /// </summary>
    public Guid CorrelationId { get; init; }

    /// <summary> Operator Email taken from jwt claims </summary>
    public string Email => _operatorEmail ??= UserClaims.FirstOrDefault(x => x.Type.Contains("email"))?.Value;

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

    private ExternalIdp? _idp;
    private string?      _externalId;
    private string?      _operatorEmail;

    private void ProcessSubjectClaim() {
        var subjectParts = UserClaims.First(x => x.Type == "sub").Value.Split('|');
        _externalId ??= subjectParts[1];
        _idp        =   Enum.Parse<ExternalIdp>(subjectParts[0].ToPascalCase());
    }
}