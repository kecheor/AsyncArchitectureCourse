using Popug.Common.Enums;

namespace Popug.Accounts.Repository;
public record Account(int? Id, string ChipId, string Name, Roles Role, int BeakCurvature);