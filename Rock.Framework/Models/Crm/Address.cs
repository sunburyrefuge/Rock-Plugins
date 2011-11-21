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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Rock.Models;

namespace Rock.Models.Crm
{
    [Table( "crmAddress" )]
    public partial class Address : ModelWithAttributes<Address>, IAuditable
    {
		[MaxLength( 100 )]
		[DataMember]
		public string Street1 { get; set; }
		
		[MaxLength( 100 )]
		[DataMember]
		public string Street2 { get; set; }
		
		[MaxLength( 50 )]
		[DataMember]
		public string City { get; set; }
		
		[MaxLength( 50 )]
		[DataMember]
		public string State { get; set; }
		
		[MaxLength( 50 )]
		[DataMember]
		public string Country { get; set; }
		
		[MaxLength( 10 )]
		[DataMember]
		public string Zip { get; set; }
		
		[DataMember]
		public float latitude { get; set; }
		
		[DataMember]
		public float longitude { get; set; }
		
		[MaxLength( 50 )]
		[DataMember]
		public string StandardizeService { get; set; }
		
		[DataMember]
		public int? StandardizeResultCode { get; set; }
		
		[DataMember]
		public DateTime? StandardizeDate { get; set; }
		
		[MaxLength( 50 )]
		[DataMember]
		public string GeocodeService { get; set; }
		
		[DataMember]
		public int? GeocodeResultCode { get; set; }
		
		[DataMember]
		public DateTime? GeocodeDate { get; set; }
		
		[DataMember]
		public DateTime? CreatedDateTime { get; set; }
		
		[DataMember]
		public DateTime? ModifiedDateTime { get; set; }
		
		[DataMember]
		public int? CreatedByPersonId { get; set; }
		
		[DataMember]
		public int? ModifiedByPersonId { get; set; }
		
		[NotMapped]
		public override string AuthEntity { get { return "Crm.Address"; } }

		public virtual ICollection<AddressRaw> AddressRaws { get; set; }

		public virtual Person CreatedByPerson { get; set; }

		public virtual Person ModifiedByPerson { get; set; }

        public static Address Read(int id)
        {
            return new Rock.Services.Crm.AddressService().Get( id );
        }

    }

    public partial class AddressConfiguration : EntityTypeConfiguration<Address>
    {
        public AddressConfiguration()
        {
			this.HasOptional( p => p.CreatedByPerson ).WithMany().HasForeignKey( p => p.CreatedByPersonId );
			this.HasOptional( p => p.ModifiedByPerson ).WithMany().HasForeignKey( p => p.ModifiedByPersonId );
		}
    }
}
