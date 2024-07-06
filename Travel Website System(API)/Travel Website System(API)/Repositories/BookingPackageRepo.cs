﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel_Website_System_API.Models;

namespace Travel_Website_System_API_.Repositories
{
    // to get all booking relative data
    public class BookingPackageRepo:IBookingPackageRepo
    {
        ApplicationDBContext db;
        public BookingPackageRepo(ApplicationDBContext db)
        {
            this.db = db;
        }
        public bool GetAllBokking(int id)
        {
            return db.BookingPackages.Any(bp => bp.packageId == id);
        }

        public List<BookingPackage> selectAll()
        {
            return db.BookingPackages.Include(b=>b.Payment).Include(b=>b.client).Include(b=>b.package).ToList();
        }

        public BookingPackage GetById(int id)
        {
          return  db.BookingPackages.Include(b => b.Payment).Include(b => b.client).ThenInclude(c=>c.ApplicationUser).Include(b => b.package).SingleOrDefault(b =>b.Id == id);
        }
        // this to get relative data and allbooking for a client with  no payment 
        public List<BookingPackage> SelectAllBookingforClient(string clientId)
        {
            return db.BookingPackages.Include(b => b.client).ThenInclude(c => c.ApplicationUser).Include(b => b.package).
                Where(b => b.clientId.Equals(clientId) && b.Payment == null)
                .ToList();
        }

        // this to get relative data and allbooking for a client with  those has paid 
        public List<BookingPackage> GetAllPaidBookingsForClient(string clientId)
        {
            return db.BookingPackages.Include(b => b.client).ThenInclude(c => c.ApplicationUser).Include(b => b.package).
                Include(b=>b.Payment)
                            .Where(b => b.clientId.Equals(clientId) && b.Payment!=null)
                            .ToList();
        }
   

    }
}
