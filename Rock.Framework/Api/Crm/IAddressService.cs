//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the T4\Model.tt template.
//
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//
// THIS WORK IS LICENSED UNDER A CREATIVE COMMONS ATTRIBUTION-NONCOMMERCIAL-
// SHAREALIKE 3.0 UNPORTED LICENSE:
// http://creativecommons.org/licenses/by-nc-sa/3.0/
//
using System.ServiceModel;

namespace Rock.Api.Crm
{
	[ServiceContract]
    public partial interface IAddressService
    {
		[OperationContract]
        Rock.Models.Crm.Address Get( string id );

        [OperationContract]
        void UpdateAddress( string id, Rock.Models.Crm.Address Address );

        [OperationContract]
        void CreateAddress( Rock.Models.Crm.Address Address );

        [OperationContract]
        void DeleteAddress( string id );
    }
}
