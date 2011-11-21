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
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

using Rock.Cms.Security;

namespace Rock.Api.Crm
{
    [Export(typeof(IService))]
    [ExportMetadata("RouteName", "api/Crm/Address")]
	[AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public partial class AddressService : IAddressService, IService
    {
		[WebGet( UriTemplate = "{id}" )]
        public Rock.Models.Crm.Address Get( string id )
        {
            var currentUser = System.Web.Security.Membership.GetUser();
            if ( currentUser == null )
                throw new FaultException( "Must be logged in" );

            using (Rock.Helpers.UnitOfWorkScope uow = new Rock.Helpers.UnitOfWorkScope())
            {
                uow.objectContext.Configuration.ProxyCreationEnabled = false;
				Rock.Services.Crm.AddressService AddressService = new Rock.Services.Crm.AddressService();
                Rock.Models.Crm.Address Address = AddressService.Get( int.Parse( id ) );
                if ( Address.Authorized( "View", currentUser ) )
                    return Address;
                else
                    throw new FaultException( "Unauthorized" );
            }
        }
		
		[WebInvoke( Method = "PUT", UriTemplate = "{id}" )]
        public void UpdateAddress( string id, Rock.Models.Crm.Address Address )
        {
            var currentUser = System.Web.Security.Membership.GetUser();
            if ( currentUser == null )
                throw new FaultException( "Must be logged in" );

            using ( Rock.Helpers.UnitOfWorkScope uow = new Rock.Helpers.UnitOfWorkScope() )
            {
                uow.objectContext.Configuration.ProxyCreationEnabled = false;

                Rock.Services.Crm.AddressService AddressService = new Rock.Services.Crm.AddressService();
                Rock.Models.Crm.Address existingAddress = AddressService.Get( int.Parse( id ) );
                if ( existingAddress.Authorized( "Edit", currentUser ) )
                {
                    uow.objectContext.Entry(existingAddress).CurrentValues.SetValues(Address);
                    AddressService.Save( existingAddress, currentUser.PersonId() );
                }
                else
                    throw new FaultException( "Unauthorized" );
            }
        }

		[WebInvoke( Method = "POST", UriTemplate = "" )]
        public void CreateAddress( Rock.Models.Crm.Address Address )
        {
            var currentUser = System.Web.Security.Membership.GetUser();
            if ( currentUser == null )
                throw new FaultException( "Must be logged in" );

            using ( Rock.Helpers.UnitOfWorkScope uow = new Rock.Helpers.UnitOfWorkScope() )
            {
                uow.objectContext.Configuration.ProxyCreationEnabled = false;

                Rock.Services.Crm.AddressService AddressService = new Rock.Services.Crm.AddressService();
                AddressService.Add( Address, currentUser.PersonId() );
                AddressService.Save( Address, currentUser.PersonId() );
            }
        }

		[WebInvoke( Method = "DELETE", UriTemplate = "{id}" )]
        public void DeleteAddress( string id )
        {
            var currentUser = System.Web.Security.Membership.GetUser();
            if ( currentUser == null )
                throw new FaultException( "Must be logged in" );

            using ( Rock.Helpers.UnitOfWorkScope uow = new Rock.Helpers.UnitOfWorkScope() )
            {
                uow.objectContext.Configuration.ProxyCreationEnabled = false;

                Rock.Services.Crm.AddressService AddressService = new Rock.Services.Crm.AddressService();
                Rock.Models.Crm.Address Address = AddressService.Get( int.Parse( id ) );
                if ( Address.Authorized( "Edit", currentUser ) )
                {
                    AddressService.Delete( Address, currentUser.PersonId() );
                }
                else
                    throw new FaultException( "Unauthorized" );
            }
        }

    }
}
