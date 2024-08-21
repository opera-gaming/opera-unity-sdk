using System.Collections.Generic;
using System.Linq;

namespace Opera
{
    public sealed class GroupForNewGameFinder
    {
        public (bool success, GroupDataApi group) FindGroup(GroupDataApi currentGroup, IEnumerable<GroupDataApi> allGroups)
        {
            var currentGroupId = currentGroup?.studioId;
            var groupWithSameId = allGroups.FirstOrDefault(group => group.studioId == currentGroupId);
            var success = string.IsNullOrEmpty(currentGroupId) || groupWithSameId != null;

            return (success, groupWithSameId ?? allGroups.FirstOrDefault());
        }
    }
}
