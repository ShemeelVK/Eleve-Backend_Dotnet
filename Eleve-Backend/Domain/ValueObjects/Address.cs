namespace Eleve_Backend.Domain.ValueObjects
{
    public class Address
    {
        public string Name { get; set; }
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string ZipCode { get; private set; }
        public string PhoneNumber   { get; private set; }
        

        public Address(string name,string street, string city, string state, string zipCode,string phoneNumber)
        {
            Name = name;
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
            PhoneNumber = phoneNumber;
        }

        //Needed for EF CORE mapping
        private Address() { }
    }
}
