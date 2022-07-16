using GoldenBook.DataAccess.Repository.IRepository;
using GoldenBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBook.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public int DecrementCount(ShoppingCart shoppingCart, int Count)
        {
            shoppingCart.Count-=Count;
            return shoppingCart.Count;
        }

        public int IncrementCount(ShoppingCart shoppingCart, int Count)
        {
            shoppingCart.Count += Count;
            return shoppingCart.Count;
        }
    }
}
