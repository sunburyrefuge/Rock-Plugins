//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//
// THIS WORK IS LICENSED UNDER A CREATIVE COMMONS ATTRIBUTION-NONCOMMERCIAL-
// SHAREALIKE 3.0 UNPORTED LICENSE:
// http://creativecommons.org/licenses/by-nc-sa/3.0/
//

using System;
using System.Linq;

using Rock.Data;

namespace Rock.Model
{
    /// <summary>
    /// GroupMember Service class
    /// </summary>
    public partial class GroupMemberService : Service<GroupMember>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupMemberService"/> class
        /// </summary>
        public GroupMemberService()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupMemberService"/> class
        /// </summary>
        /// <param name="repository">The repository.</param>
        public GroupMemberService(IRepository<GroupMember> repository) : base(repository)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupMemberService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public GroupMemberService(RockContext context) : base(context)
        {
        }

        /// <summary>
        /// Determines whether this instance can delete the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        ///   <c>true</c> if this instance can delete the specified item; otherwise, <c>false</c>.
        /// </returns>
        public bool CanDelete( GroupMember item, out string errorMessage )
        {
            errorMessage = string.Empty;
            return true;
        }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class GroupMemberExtensionMethods
    {
        /// <summary>
        /// Clones this GroupMember object to a new GroupMember object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static GroupMember Clone( this GroupMember source, bool deepCopy )
        {
            if (deepCopy)
            {
                return source.Clone() as GroupMember;
            }
            else
            {
                var target = new GroupMember();
                target.IsSystem = source.IsSystem;
                target.GroupId = source.GroupId;
                target.PersonId = source.PersonId;
                target.GroupRoleId = source.GroupRoleId;
                target.GroupMemberStatus = source.GroupMemberStatus;
                target.Id = source.Id;
                target.Guid = source.Guid;

            
                return target;
            }
        }
    }
}
