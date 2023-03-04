using System;

namespace EndPoints.Model
{
    public class AddressModel : RestModelBase
    {
        public Guid Id { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public Guid ClientId { get; set; }
    }
}
