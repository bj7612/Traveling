using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Traveling.Database;
using Traveling.Models;

namespace Traveling.Services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        private readonly AppDbContext _context;

        public TouristRouteRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<bool> TouristRouteExistsAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.AnyAsync(n => n.Id == touristRouteId);
        }
        public async Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId)
        {
            // firstOrDefault() will triger the query in database
            return await _context.TouristRoutes.Include(t => t.TouristRoutePictures).FirstOrDefaultAsync(n => n.Id == touristRouteId);
        }

        public async Task<IEnumerable<TouristRoute>> GetTouristRoutesAsync(string? keyword, string? ratingOperator, int? ratingValue)
        {
            // using inlcude() to add the touristRoutePicutre model to touristRoutes or using join to add the model which without haveing forient key relationship to each other
            // IQueryable provide the possibility of dynamic expression, result is just a queryable statement, not a real query action.
            IQueryable<TouristRoute> result = _context.TouristRoutes.Include(t => t.TouristRoutePictures);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                result = result.Where(t => t.Title.Contains(keyword));
            }

            if (ratingValue >= 0) 
            {
                result = ratingOperator switch
                {
                    "largerThan" => result.Where(t => t.Rating >= ratingValue),
                    "lessThan" => result.Where(t => t.Rating <= ratingValue),
                    _=> result.Where(t => t.Rating == ratingValue),
                };
            }

            return await result.ToListAsync(); // ToList() will triger the query action in database
        }

        public async Task<TouristRoutePicture> GetPictureAsync(int pictureId)
        {
           return await _context.TouristRoutePictures.Where(p => p.Id == pictureId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutePictures.Where(n => n.TouristRouteId == touristRouteId).ToListAsync();
        }

        public void AddTouristRoute(TouristRoute touristRoute)
        {
            if (touristRoute == null)
                throw new ArgumentNullException(nameof(touristRoute));

            _context.TouristRoutes.Add(touristRoute);
        }

        public void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture)
        {
            if (touristRouteId == Guid.Empty)
                throw new ArgumentNullException(nameof(touristRouteId));
            if (touristRoutePicture == null)
                throw new ArgumentNullException(nameof(touristRoutePicture));

            touristRoutePicture.TouristRouteId = touristRouteId;
            _context.TouristRoutePictures.Add(touristRoutePicture);
        }

        public async Task<IEnumerable<TouristRoute>> GetTouristRoutesByIDListAsync(IEnumerable<Guid> ids)
        {
            return await _context.TouristRoutes.Where(t => ids.Contains(t.Id)).ToListAsync();   
        }

        public void DeleteTouristRoute(TouristRoute touristRoute)
        {
            _context.TouristRoutes.Remove(touristRoute);
        }

        public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.TouristRoutes.RemoveRange(touristRoutes);
        }

        public void DeleteTouristRoutePicture(TouristRoutePicture picture)
        {
            _context.TouristRoutePictures.Remove(picture);
        }

        public async Task<ShoppingCart> GetShoppingCartByUserId(string userId) 
        {
            return await _context.ShoppingCarts
                .Include(x => x.User)
                .Include(x => x.ShoppingCartItems).ThenInclude(li => li.TouristRoute)
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();      
        }

        public async Task CreateShoppingCart(ShoppingCart shoppingCart)
        {
            await _context.ShoppingCarts.AddAsync(shoppingCart);
        }

        public async Task<bool> SaveAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

    }
}
