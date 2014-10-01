﻿// <copyright>
// Copyright 2013 by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rock.SystemGuid
{
    /// <summary>
    /// Group Role System Guids
    /// </summary>
    public static class GroupRole
    {
        #region Family Members

        /// <summary>
        /// Gets the adult family member role
        /// </summary>
        public const string GROUPROLE_FAMILY_MEMBER_ADULT= "2639F9A5-2AAE-4E48-A8C3-4FFE86681E42";
        
        /// <summary>
        /// Gets the child family member role
        /// </summary>
        public const string GROUPROLE_FAMILY_MEMBER_CHILD= "C8B1814F-6AA7-4055-B2D7-48FE20429CB9";

        #endregion

        #region Known Relationships

        /// <summary>
        /// Gets the Known Relationships owner role.
        /// </summary>
        /// <value>
        /// The role Guid
        /// </value>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_OWNER = "7BC6C12E-0CD1-4DFD-8D5B-1B35AE714C42";
        
        /// <summary>
        /// A person that can be checked in by the owner of this known relationship group
        /// </summary>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_CAN_CHECK_IN = "DC8E5C35-F37C-4B49-A5C6-DF3D94FC808F";

        /// <summary>
        /// A person that can check in the owner of this known relationship group
        /// </summary>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_ALLOW_CHECK_IN_BY = "FF9869F1-BC56-4410-8A12-CAFC32C62257";
        
        /// <summary>
        /// A grandparent of the owner of this known relationship group
        /// </summary>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_GRANDPARENT = "567DA89F-3C43-443D-A988-C05BC516EF28";

        /// <summary>
        /// A grandchild of the owner of this known relationship group
        /// </summary>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_GRANDCHILD = "C1A393B2-519D-4E46-A551-E48C36BCAC06";

        /// <summary>
        /// A brother or sister of the owner of this known relationship group
        /// </summary>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_SIBLING = "1D92F0E1-E161-4160-9C63-2D0A901D3C38";

        /// <summary>
        /// A person that was first invited by the owner of this known relationship group
        /// </summary>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_INVITED = "32E71DAC-B40E-467A-98C9-0AA92AA5025E";

        /// <summary>
        /// The person that first invited the owner of this known relationship group
        /// </summary>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_INVITED_BY = "03BE336C-CD3D-445C-86EC-0856A51DC926";

        /// <summary>
        /// A step child of the owner of this known relationship group
        /// </summary>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_STEP_CHILD = "EFD2D6D1-A407-4EFB-9086-5DF1F19B7D93";

        /// <summary>
        /// A step parent of the owner of this known relationship group
        /// </summary>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_STEP_PARENT = "D14827EF-5D43-442D-8134-DEB58AAC93C5";

        /// <summary>
        /// An adult child of the owner of this known relationship group
        /// </summary>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_CHILD = "F87DF00F-E86D-4771-A3AE-DBF79B78CF5D";

        /// <summary>
        /// The parent of the owner of this known relationship group
        /// </summary>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_PARENT = "6F3FADC4-6320-4B54-9CF6-02EF9586A660";

        /// <summary>
        /// Role to identify former spouses after divorce.
        /// </summary>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_PREVIOUS_SPOUSE = "60C6142E-8E00-4678-BC2F-983BB7BDE80B";

        /// <summary>
        /// Role to identify contacts related to a business.
        /// </summary>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_BUSINESS_CONTACT = "102E6AF5-62C2-4767-B473-C9C228D75FB6";

        /// <summary>
        /// A role to identify the business a person owns.
        /// </summary>
        public const string GROUPROLE_KNOWN_RELATIONSHIPS_BUSINESS = "7FC58BB2-7C1E-4C5C-B2B3-4738258A0BE0";

        #endregion

        #region Implied Relationships

        /// <summary>
        /// Gets the Implied Relationships owner role.
        /// </summary>
        /// <value>
        /// The role Guid.
        /// </value>
        public const string GROUPROLE_IMPLIED_RELATIONSHIPS_OWNER= "CB9A0E14-6FCF-4C07-A49A-D7873F45E196";

        /// <summary>
        /// Gets the Implied Relationships related role.
        /// </summary>
        /// <value>
        /// The role Guid.
        /// </value>
        public const string GROUPROLE_IMPLIED_RELATIONSHIPS_RELATED= "FEA75948-97CB-4DE9-8A0D-43FA2646F55B";

        #endregion

    }
}