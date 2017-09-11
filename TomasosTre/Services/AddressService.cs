using System;
using System.Linq;
using TomasosTre.Data;
using TomasosTre.Models;

namespace TomasosTre.Services
{
    public class AddressService
    {
        #region Setup
        private readonly ApplicationDbContext _context;
        public AddressService(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion

        public void Create(string street, int zip, string city, string customerId = null)
        {
            Address address = new Address
            {
                Street = street,
                Zip = zip,
                City = city,
                CustomerId = customerId ?? null,
                StartDateTime = DateTime.Now,
                EndDateTime = null,
            };
            address.Customer = customerId != null ? _context.ApplicationUsers.First(x => x.Id == customerId) : null;

            _context.Addresses.Add(address);
            _context.SaveChanges();
        }

        /// <summary>
        /// Reads the Address based on an Address id
        /// </summary>
        /// <param name="id">The Address ID to search with</param>
        /// <returns>The first Address of that ID found</returns>
        public Address Read(int id)
        {
            return _context.Addresses.FirstOrDefault(x => x.AddressId == id);
        }

        /// <summary>
        /// Reads the Address based on a Customer id
        /// </summary>
        /// <param name="customerId">The Customer id to search with</param>
        /// <param name="atDate">If filled, search the log for which address was relevant at a provided date. Looks for the current Address if left null.</param>
        /// <returns>The current, or relevant, Address of provided Customer</returns>
        public Address Read(string customerId, DateTime? atDate = null)
        {
            //if (atDate == null)
            //{
            //    var address = _context.Addresses.FirstOrDefault(x => x.CustomerId == customerId && x.EndDateTime == null);
            //    return address;
            //}
            //else
            //{
            //    var address = _context.Addresses.FirstOrDefault(x => x.CustomerId == customerId &&
            //        x.StartDateTime < atDate && x.EndDateTime > atDate);
            //    return address;
            //}
            return atDate == null ?
                _context.Addresses.FirstOrDefault(x => x.CustomerId == customerId && x.EndDateTime == null) :
                _context.Addresses.FirstOrDefault(x => x.CustomerId == customerId &&
                    x.StartDateTime < atDate && x.EndDateTime > atDate);
        }

        /// <summary>
        /// Archives the current address, and creates a new, up-to-date adress for the customer.
        /// </summary>
        /// <param name="id">The id of the Address to update</param>
        /// <param name="data">The new Address. Requires non-null Street, Zip, City and CustomerId properties</param>
        public void Update(int id, Address data)
        {
            //ApplicationUser customer = _context.ApplicationUsers.FirstOrDefault(x => x.Id == data.CustomerId);
            Address current = Read(id);

            current.EndDateTime = DateTime.Now;

            _context.Update(current);
            _context.SaveChanges();

            Create(data.Street, data.Zip, data.City, data.CustomerId);
        }

        public void Delete(int id)
        {
            Address data = Read(id);
            _context.Remove(data);
            _context.SaveChanges();
        }
    }
}
