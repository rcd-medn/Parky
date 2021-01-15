



using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;

namespace ParkyAPI.Repository
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ParkDbContext _context;

        public TrailRepository(ParkDbContext context)
        {
            _context = context;
        }
        
        public bool CreateTrail(Trail trail)
        {
            _context.Trails.Add(trail);
            return Save();
        }

        public bool UpdateTrail(Trail trail)
        {
            _context.Trails.Update(trail);
            return Save();
        }

        public bool DeleteTrail(Trail trail)
        {
            _context.Trails.Remove(trail);
            return Save();
        }

        public Trail GetTrail(int trailId)
        {
            return _context.Trails
                .Include(t => t.NationalPark)
                .FirstOrDefault(i => i.Id == trailId);
        }

        public ICollection<Trail> GetTrails()
        {
            return _context.Trails
                .Include(t => t.NationalPark)
                .OrderBy(n => n.Name)
                .ToList();
        }

        public bool TrailExist(string name)
        {
            bool value = _context.Trails
                .Any(n => n.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool TrailExist(int id)
        {
            return _context.Trails.Any(n => n.Id == id);
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0 ? true : false;
        }

        public ICollection<Trail> GetTrailsInNationalPark(int npId)
        {
            return _context.Trails
                .Include(t => t.NationalPark)
                .Where(t => t.NationalParkId == npId)
                .ToList();
        }
    }
}
